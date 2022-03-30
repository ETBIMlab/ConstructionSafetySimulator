using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class Upgrade_URPToHDRP : EditorWindow
{
	private const TextureFormat DEFAULT_MASKMAP_FORMAT = TextureFormat.RGBA32;

	[MenuItem("Edit/Render Pipeline/Selected URP Lit --> HDRP Lit")]
	static void Selected_URPToHDRP()
	{
		Undo.RegisterCompleteObjectUndo(Selection.objects, "UndoDowngrade");

		UnityEngine.Object[] selectedObjects = Selection.objects;

		List<Material> urpMaterials = new List<Material>();

		for (int i = 0; i < selectedObjects.Length; i++)
		{
			if (selectedObjects[i] is Material)
			{
				urpMaterials.Add(selectedObjects[i] as Material);
			}
		}

		URPToHDRP_Materials(urpMaterials.ToArray());
	}


	private static void URPToHDRP_Materials(Material[] urpMaterials)
	{
		Shader URP_LIT = Shader.Find("Universal Render Pipeline/Lit");
		Shader HDRP_LIT = Shader.Find("HDRP/Lit");

		for (int i = 0; i < urpMaterials.Length; i++)
		{
			EditorUtility.DisplayProgressBar("Fixing materials", urpMaterials[i].name, i / (float)urpMaterials.Length);

			if (urpMaterials[i].shader == URP_LIT)
			{
				//try
				//{
					URPToHDRP_LitMaterial(urpMaterials[i], HDRP_LIT);
				//}
				//catch(Exception e)
				//{
				//	Debug.LogError("Material asset '" + urpMaterials[i].name + "' (URP Lit) failed to convert to HDRP: " + e.Message, urpMaterials[i]);
				//}
			}
			// Default...
			else
			{
				Debug.LogWarning("Material asset '" + urpMaterials[i].name + "' does not have a defined conversion for its shader: '" + urpMaterials[i].shader.name + "'", urpMaterials[i]);
			}
		}

		EditorUtility.ClearProgressBar();
	}

	private static void URPToHDRP_LitMaterial(Material litMaterial, Shader HDRP_LIT)
	{
		// WorkflowMode: Specular = 0, Metallic = 1
		int _WorkflowMode = (int)litMaterial.GetFloat("_WorkflowMode");
		//Get render type Opaque = 0, Transparent = 1
		int _Surface = (int)litMaterial.GetFloat("_Surface");
		// Blending Mode: Alpha = 0, Premult = 1, Add = 2, Mult = 3
		int _Blend = (int)litMaterial.GetFloat("_Blend");
		// Render Face: Front = 2, Back = 1, Both = 0
		int _Cull = (int)litMaterial.GetFloat("_Cull");
		// Alpha Clip: Boolean
		bool _AlphaClip = litMaterial.GetFloat("_AlphaClip") != 0;
		// (Alpha) Threshold: Boolean
		float _Cutoff = litMaterial.GetFloat("_Cutoff");
		// Receive Shadows: Boolean
		bool _ReceiveShadows = litMaterial.GetFloat("_ReceiveShadows") != 0;

		// Albedo
		Texture2D _BaseMap = (Texture2D)litMaterial.GetTexture("_BaseMap");
		// (Base) Color
		Color _BaseColor = litMaterial.GetColor("_BaseColor");
		// N/A
		Vector2 _BaseScale = litMaterial.GetTextureScale("_BaseMap");
		// N/A
		Vector2 _BaseOffset = litMaterial.GetTextureOffset("_BaseMap");

		// Smoothness: Range (0.0, 1.0)
		float _Smoothness = litMaterial.GetFloat("_Smoothness");
		// Smoothness texture channel: Specular/Metallic Alpha = 0, Albedo Alpha = 1
		float _SmoothnessTextureChannel = litMaterial.GetFloat("_SmoothnessTextureChannel");

		// (Strength) Metallic: Range (0.0, 1.0)
		float _Metallic = litMaterial.GetFloat("_Metallic");
		// (Map) Metallic
		Texture2D _MetallicGlossMap = (Texture2D)litMaterial.GetTexture("_MetallicGlossMap");

		// (Color) Specular
		Color _SpecColor = litMaterial.GetColor("_SpecColor");
		// (Map) Specular
		Texture2D _SpecGlossMap = (Texture2D)litMaterial.GetTexture("_SpecGlossMap");

		// Specular Highlights: Boolean
		bool _SpecularHighlights = litMaterial.GetFloat("_SpecularHighlights") != 0;
		// Environment Reflections: Boolean
		float _EnvironmentReflections = litMaterial.GetFloat("_EnvironmentReflections");

		// (Normal) Scale
		float _BumpScale = litMaterial.GetFloat("_BumpScale");
		// Normal Map
		Texture2D _BumpMap = (Texture2D)litMaterial.GetTexture("_BumpMap");

		// (Hight) Scale: Range(0.005, 0.08)
		float _Parallax = litMaterial.GetFloat("_Parallax");
		// Height Map
		Texture2D _ParallaxMap = (Texture2D)litMaterial.GetTexture("_ParallaxMap");

		// (Occlusion) Strength: Range(0.0, 1.0)
		float _OcclusionStrength = litMaterial.GetFloat("_OcclusionStrength");
		// Occlusion
		Texture2D _OcclusionMap = (Texture2D)litMaterial.GetTexture("_OcclusionMap");

		// (Emission) Color: HDR
		Color _EmissionColor = litMaterial.GetColor("_EmissionColor");
		// (Map) Emission
		Texture2D _EmissionMap = (Texture2D)litMaterial.GetTexture("_EmissionMap");

		// Detail Mask
		Texture2D _DetailMask = (Texture2D)litMaterial.GetTexture("_DetailMask");

		// (Detail Albedo) Scale: Range(0.0, 2.0)
		float _DetailAlbedoMapScale = litMaterial.GetFloat("_DetailAlbedoMapScale");
		// Detail Albedo x2
		Texture2D _DetailAlbedoMap = (Texture2D)litMaterial.GetTexture("_DetailAlbedoMap");
		// N/A
		Vector2 _DetailScale = litMaterial.GetTextureScale("_DetailAlbedoMap");
		// N/A
		Vector2 _DetailOffset = litMaterial.GetTextureOffset("_DetailAlbedoMap");

		// (Detail Normal) Scale: Range(0.0, 2.0)
		float _DetailNormalMapScale = litMaterial.GetFloat("_DetailNormalMapScale");
		// (Detail) Normal Map
		Texture2D _DetailNormalMap = (Texture2D)litMaterial.GetTexture("_DetailNormalMap");
		
		// Priority?: Editmode props
		float _QueueOffset = litMaterial.GetFloat("_QueueOffset");

		Texture2D _MaskMap = CreatePackedMaskMap((_WorkflowMode == 0) ? null : _MetallicGlossMap, _OcclusionMap, _DetailMask, (_SmoothnessTextureChannel == 0) ? _MetallicGlossMap : _BaseMap);

		Texture2D _DetailMap = CreatePackedDetailMap(_DetailAlbedoMap, _DetailNormalMap, null);


		litMaterial.shader = HDRP_LIT;

		litMaterial.SetColor("_BaseColor", _BaseColor);
		litMaterial.SetTexture("_BaseColorMap", _BaseMap);
		litMaterial.SetTextureScale("_BaseColorMap", _BaseScale);
		litMaterial.SetTextureOffset("_BaseColorMap", _BaseOffset);
		

		litMaterial.SetFloat("_Metallic", _Metallic);
		litMaterial.SetFloat("_Smoothness", _Smoothness);
		litMaterial.SetTexture("_MaskMap", _MaskMap);
		litMaterial.SetTextureScale("_MaskMap", _BaseScale);
		litMaterial.SetTextureOffset("_MaskMap", _BaseOffset);
		litMaterial.SetFloat("_AORemapMax", _OcclusionStrength);


		litMaterial.SetTexture("_NormalMap", _BumpMap);
		litMaterial.SetTextureScale("_NormalMap", _BaseScale);
		litMaterial.SetTextureOffset("_NormalMap", _BaseOffset);
		litMaterial.SetFloat("_NormalScale", _BumpScale);

		//if (_ParallaxMap) 
		//	litMaterial.SetFloat("_HeightMap", _DisplacementMode, 1);
		litMaterial.SetTexture("_HeightMap", _ParallaxMap);
		litMaterial.SetTextureScale("_HeightMap", _BaseScale);
		litMaterial.SetTextureOffset("_HeightMap", _BaseOffset);
		litMaterial.SetFloat("_HeightTessAmplitude", _Parallax * 100);

		litMaterial.SetTexture("_DetailMap", _DetailMap);
		litMaterial.SetTextureScale("_DetailMap", _DetailScale);
		litMaterial.SetTextureOffset("_DetailMap", _DetailOffset);
		litMaterial.SetFloat("_DetailAlbedoScale", _DetailAlbedoMapScale);
		litMaterial.SetFloat("_DetailNormalScale", _DetailNormalMapScale);

		litMaterial.SetColor("_SpecularColor", _SpecColor);
		litMaterial.SetTexture("_SpecularColorMap", _SpecGlossMap);
		litMaterial.SetTextureScale("_SpecularColorMap", _BaseScale);
		litMaterial.SetTextureOffset("_SpecularColorMap", _BaseOffset);

		litMaterial.SetColor("_EmissiveColor", _EmissionColor);
		litMaterial.SetTexture("_EmissiveColorMap", _EmissionMap);
		litMaterial.SetTextureScale("_EmissiveColorMap", _BaseScale);
		litMaterial.SetTextureOffset("_EmissiveColorMap", _BaseOffset);

		litMaterial.SetFloat("_AlphaCutoffEnable", _AlphaClip ? 1 : 0);
		litMaterial.SetFloat("_AlphaCutoff", _Cutoff);

		litMaterial.SetFloat("_SurfaceType", _Surface);
		litMaterial.SetFloat("_BlendMode", _Blend);


		litMaterial.SetFloat("_EnableBlendModePreserveSpecularLighting", (_SpecularHighlights) ? 1 : 0);
		litMaterial.SetFloat("_EnableBlendModePreserveSpecularLighting", (_SpecularHighlights) ? 1 : 0);


		if (_Cull != 2)
		{
			litMaterial.SetFloat("_DoubleSidedEnable", 1);
			litMaterial.SetFloat("_DoubleSidedNormalMode", (_Cull == 1) ? 0 : 1);
		}

		// TODO::
		if (_QueueOffset != 0) Debug.LogWarning("Material '" + litMaterial.name + "' has a _QueueOffset (_QueueOffset is not compatible for HDRP)");

		if (!_ReceiveShadows) Debug.LogWarning("Material '" + litMaterial.name + "' is set to NOT receive shadows, which HDRP does not support. Consider using Light Layers instead.");
	}


	private static Texture2D CreatePackedMaskMap(Texture2D metallic, Texture2D occlusion, Texture2D height, Texture2D smoothness)
	{
		if (!(metallic || occlusion || height)) return null;

		TextureWithImport[] textures = new TextureWithImport[]
		{
			new TextureWithImport(metallic),
			new TextureWithImport(occlusion),
			new TextureWithImport(height),
			new TextureWithImport(smoothness)
		};

		Vector2Int maxSize = Vector2Int.zero;

		foreach (TextureWithImport twi in textures)
		{
			if (twi.texture.width > maxSize.x)
				maxSize.x = twi.texture.width;
			if (twi.texture.height > maxSize.y)
				maxSize.y = twi.texture.height;
		}

		Texture2D maskMap = new Texture2D(maxSize.x, maxSize.y, DEFAULT_MASKMAP_FORMAT, false);

		Color temp = Color.white;
		for (int i = 0; i < maskMap.height; ++i)
		{
			for (int j = 0; j < maskMap.width; ++j)
			{
				float normx = (float)j / (float)maskMap.width;
				float normy = (float)i / (float)maskMap.height;
				temp.r = textures[0].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.g = textures[1].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.b = textures[2].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.a = textures[3].texture.GetPixelBilinear(normx, normy).a;
				maskMap.SetPixel(j, i, temp);
			}
		}
		maskMap.Apply();

		string path = "";
		foreach (TextureWithImport twi in textures) if (twi.tImporter)
		{
			path = twi.tImporter.assetPath;
			break;
		}

		path = path.Split('.')[0] + "-MASK_MAP.png";
		
		File.WriteAllBytes(Application.dataPath + "/../" + path, maskMap.EncodeToPNG());
		UnityEngine.Object.DestroyImmediate(maskMap);

		AssetDatabase.ImportAsset(path);
		AssetDatabase.Refresh();
		return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
	}

	private static Texture2D CreatePackedDetailMap(Texture2D albedo, Texture2D normal, Texture2D smoothness)
	{
		if (!(albedo || normal || smoothness)) return null;

		TextureWithImport[] textures = new TextureWithImport[]
		{
			new TextureWithImport(albedo),
			new TextureWithImport(normal),
			new TextureWithImport(smoothness)
		};

		Vector2Int maxSize = Vector2Int.zero;
		foreach (TextureWithImport twi in textures)
		{
			if (twi.texture.width > maxSize.x)
				maxSize.x = twi.texture.width;
			if (twi.texture.height > maxSize.y) 
				maxSize.y = twi.texture.height;
		}

		Texture2D detailmap = new Texture2D(maxSize.x, maxSize.y, DEFAULT_MASKMAP_FORMAT, false);

		Color temp = Color.white;
		for (int i = 0; i < detailmap.height; ++i)
		{
			for (int j = 0; j < detailmap.width; ++j)
			{
				float normx = (float)j / (float)detailmap.width;
				float normy = (float)i / (float)detailmap.height;

				temp.r = textures[0].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.g = textures[1].texture.GetPixelBilinear(normx, normy).g;
				temp.b = textures[2].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.a = textures[1].texture.GetPixelBilinear(normx, normy).r; detailmap.SetPixel(j, i, temp);
			}
		}
		detailmap.Apply();

		string path = "";
		foreach (TextureWithImport twi in textures) if (twi.tImporter)
		{
			path = twi.tImporter.assetPath;
			break;
		}

		path = path.Split('.')[0] + "-DETAIL_MAP.png";

		File.WriteAllBytes(Application.dataPath + "/../" + path, detailmap.EncodeToPNG());
		UnityEngine.Object.DestroyImmediate(detailmap);

		AssetDatabase.ImportAsset(path);
		AssetDatabase.Refresh();
		return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
	}

	public class TextureWithImport
	{
		public TextureWithImport(Texture2D texture, bool whiteFallback = true)
		{
			if (!texture)
			{
				this.texture = whiteFallback ? Texture2D.whiteTexture : Texture2D.blackTexture;
			}
			else
			{
				this.texture = texture;

				this.tImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
				this.readable = texture.isReadable;
			}

			SetRead(true);
		}

		~TextureWithImport()
		{
			SetRead(readable);
		}

		public Texture2D texture;
		public TextureImporter tImporter;
		private bool readable;
		
		private void SetRead(bool readable)
		{
			if (!tImporter) return;
			if (tImporter.isReadable != readable)
			{
				tImporter.isReadable = readable;

				AssetDatabase.ImportAsset(tImporter.assetPath);
				AssetDatabase.Refresh();
			}
		}

	}

	
}