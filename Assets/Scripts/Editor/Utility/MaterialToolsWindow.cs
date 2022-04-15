using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MaterialToolsWindow : EditorWindow
{
	[Flags]
	public enum IncludedAssemblies { None = 0, Universal = 1, HighDefinition = 2 }

	private static IncludedAssemblies includedAssemblies = IncludedAssemblies.None;
	private static string[] validTabs;
	private static int mainTab;
	private static int packTextureTab;

	[MenuItem("Window/URP->HDRP Wizard")]
	[MenuItem("Edit/Render Pipeline/URP->HDRP Wizard")]
	static void Init()
	{
		ValidateIncludedAssemblies();
		GetWindow<MaterialToolsWindow>(false, "Material Tools").Show();
	}

	void OnGUI()
	{
		mainTab = GUILayout.Toolbar(mainTab, validTabs);

		RenderMainTab(mainTab);

		//AssetDatabase.GetMainAssetTypeAtPath("").Name
		
	}

	private void RenderMainTab(int tabindex)
	{
		switch (tabindex)
		{
		case 1:
			GUILayout.Label("Base Settings", EditorStyles.boldLabel);
			Upgrade_URPToHDRP.ValidateDiffuseAsset(EditorGUILayout.ObjectField("Defusion Value: ", Upgrade_URPToHDRP.defaultDiffusionAsset, Upgrade_URPToHDRP.DiffusionType, false));
				if (GUILayout.Button("Upgrade Seleted Materials (auto pack)")) Upgrade_URPToHDRP.Selected_URPToHDRP_WithPacked();
			if (GUILayout.Button("Upgrade Seleted Materials")) Upgrade_URPToHDRP.Selected_URPToHDRP();
			break;
		case 0:
			packTextureTab = GUILayout.Toolbar(packTextureTab, new string[] {
				"Material Textures", "Mask Map", "Detail Map", "Custom Map"
			});
			RenderPackTextureTab(packTextureTab);
			break;
		}
	}


	private static Texture2D[] textures = new Texture2D[4];
	private void RenderPackTextureTab(int tabindex)
	{
		switch (tabindex)
		{
			case 0:
				if (GUILayout.Button(new GUIContent("Pack Selected Materials", "This packs BOTH Mask and Detail if available"))) Upgrade_URPToHDRP.Selected_UPR_PackedTextures();
				break;
			case 1:
				textures[0] = EditorGUILayout.ObjectField("Metallic: ", textures[0], typeof(Texture2D), false) as Texture2D;
				textures[1] = EditorGUILayout.ObjectField("Occlusion: ", textures[1], typeof(Texture2D), false) as Texture2D;
				textures[2] = EditorGUILayout.ObjectField("Detail Mask: ", textures[2], typeof(Texture2D), false) as Texture2D;
				textures[3] = EditorGUILayout.ObjectField("Smoothness: ", textures[3], typeof(Texture2D), false) as Texture2D;
				if (GUILayout.Button(new GUIContent("Pack Textures", "This packs textures into a mask map"))) Upgrade_URPToHDRP.CreatePackedMaskMap(textures[0], textures[1], textures[2], textures[3]);
				break;
			case 2:
				textures[0] = EditorGUILayout.ObjectField("Albedo: ", textures[0], typeof(Texture2D), false) as Texture2D;
				textures[1] = EditorGUILayout.ObjectField("Normal: ", textures[1], typeof(Texture2D), false) as Texture2D;
				textures[2] = EditorGUILayout.ObjectField("Smoothness: ", textures[2], typeof(Texture2D), false) as Texture2D;
				if (GUILayout.Button(new GUIContent("Pack Textures", "This packs textures into a detail map"))) Upgrade_URPToHDRP.CreatePackedDetailMap(textures[0], textures[1], textures[2]);
				break;
			case 3:
				GUILayout.Label("Not implemented!", EditorStyles.boldLabel);
				break;
		}
	}

	private static void ValidateIncludedAssemblies()
	{
		IncludedAssemblies includes = IncludedAssemblies.None;
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.GetName().Name == "Unity.RenderPipelines.HighDefinition.Editor") includes |= IncludedAssemblies.HighDefinition;
			else if (assembly.GetName().Name == "Unity.RenderPipelines.Universal.Editor") includes |= IncludedAssemblies.Universal;
		}
		includedAssemblies = includes;

		List<string> tabs = new List<string>()
		{
			"Pack Textures",
		};

		if (includedAssemblies.HasFlag(IncludedAssemblies.HighDefinition) && includedAssemblies.HasFlag(IncludedAssemblies.Universal))
			tabs.Add("URP->HDRP");

		validTabs = tabs.ToArray();
	}
}
