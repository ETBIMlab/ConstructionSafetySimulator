using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class Upgrade_URPToHDRP : EditorWindow
{
	private const string MASK_MAP_NAME = "MaskMap";
	private const string DETAIL_MAP_NAME = "DetailMap";
	private const TextureFormat DEFAULT_MASKMAP_FORMAT = TextureFormat.RGBA32;

	private static Vector4 ConvertGUIDToVector4(string guid)
	{
		byte[] bytes = new byte[16];

		for (int i = 0; i < 16; i++)
			bytes[i] = byte.Parse(guid.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);

		return BytesAs<Vector4>(bytes);
	}

	private static float Asfloat(int hash)
	{
		return BytesAs<float>(BitConverter.GetBytes(hash));
	}

	private static T BytesAs<T>(params byte[] bytes)
	{
		BinaryFormatter bf = new BinaryFormatter();
		try
		{
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				object obj = bf.Deserialize(ms);
				return (T)obj;
			}
		}
		catch { return default(T); }
	}

	private static UnityEngine.Object defaultDiffusionAsset = null;
	private static Vector4 defaultDiffusionAsset_V4;
	private static float defaultDiffusionAsset_f;

	[MenuItem("Window/URP->HDRP Wizard")]
	[MenuItem("Edit/Render Pipeline/URP->HDRP Wizard")]
	static void Init()
	{
		GetWindow<Upgrade_URPToHDRP>(false, "URP->HDRP Wizard").Show();
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		UpdateDiffuseAsset((UnityEngine.Object)EditorGUILayout.ObjectField("Defusion Value: ", defaultDiffusionAsset, Type.GetType("DiffusionProfileSettings"), false));

		


		//AssetDatabase.GetMainAssetTypeAtPath("").Name
		if (GUILayout.Button("Upgrade Seleted Materials (auto pack)")) Selected_URPToHDRP_WithPacked();
		if (GUILayout.Button("Upgrade Seleted Materials")) Selected_URPToHDRP();
	}

	private void UpdateDiffuseAsset(UnityEngine.Object obj)
	{
		if (obj && obj != defaultDiffusionAsset)
		{
			defaultDiffusionAsset = obj;
			string diffuse_ap = AssetDatabase.GetAssetPath(defaultDiffusionAsset);
			Debug.Log(diffuse_ap + "  ::  " + AssetDatabase.GetMainAssetTypeAtPath(diffuse_ap).Name);
		}
		
	}

	private static Shader TryShaderFind(string name)
	{
		Shader result = Shader.Find(name);
		if (result) return result;

		throw new InvalidDataException("Shader '" + name + "' could not be found in project!");
	}

	static void Selected_UPR_PackedTextures()
	{
		Debug.Log("Creating packed...");
		UnityEngine.Object[] selectedObjects = Selection.objects;

		List<Material> urpMaterials = new List<Material>();

		for (int i = 0; i < selectedObjects.Length; i++)
		{
			if (selectedObjects[i] is Material)
			{
				string path = AssetDatabase.GetAssetPath(selectedObjects[i]);

				if (path != "")
					urpMaterials.Add(selectedObjects[i] as Material);
			}
		}

		URP_CreatePackedTextures(urpMaterials);
	}
	
	static void Selected_URPToHDRP_WithPacked()
	{
		Selected_UPR_PackedTextures();
		Selected_URPToHDRP();
	}


	static void Selected_URPToHDRP()
	{
		Undo.RegisterCompleteObjectUndo(Selection.objects, "UndoConversion");

		UnityEngine.Object[] selectedObjects = Selection.objects;

		List<Material> urpMaterials = new List<Material>();

		for (int i = 0; i < selectedObjects.Length; i++)
		{
			if (selectedObjects[i] is Material)
			{
				string path = AssetDatabase.GetAssetPath(selectedObjects[i]);

				if (path != "")
					urpMaterials.Add(selectedObjects[i] as Material);
			}
		}
		URPToHDRP_Materials(urpMaterials);
	}

	private static void URP_CreatePackedTextures(List<Material> urpMaterials)
	{
		/****************************/
		/* Filter Materials for URP */
		/****************************/
		for (int i = 0; i < urpMaterials.Count; i++)
		{
			if (urpMaterials[i].shader.name.Split('/')[0] != "Universal Render Pipeline")
			{
				urpMaterials.RemoveAt(i);
				i--; // This index is removed, thus repeat this index.
			}
		}
		try
		{
			AssetDatabase.StartAssetEditing();

			/*******************/
			/* Get URP shaders */
			/*******************/
			// Only includes shaders that need a packed texture.
			Shader URP_LIT = TryShaderFind("Universal Render Pipeline/Lit");

			/***************************/
			/*  Create Dense Textures  */
			/***************************/
			for (int i = 0; i < urpMaterials.Count; i++)
			{
				Material mat = urpMaterials[i];
				EditorUtility.DisplayProgressBar("Fixing materials", "Creating Packed Textures for " + mat.name, i / (float)urpMaterials.Count);

				if (mat.shader.name.Split('/')[0] != "Universal Render Pipeline")
					continue; // Selected Asset is not URP

				try
				{
					if (mat.shader == URP_LIT) CreatePackedTextures_URPLit(mat);
				}
				catch (Exception e)
				{
					Debug.LogErrorFormat("Material asset failed to pack:\n\t" +
						"Materail Name: {0}\n\t" +
						"  Shader Name: {1}\n\t" +
						"        Error: {2}\n{3}", mat.name, mat.shader, e.Message, e.StackTrace, urpMaterials[i]);
				}
			}
		}
		catch (InvalidDataException e)
		{
			Debug.LogError(e.Message + " Check to make sure URP and HDRP are included in the project.");
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + "\n" + e.StackTrace);
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();

			EditorUtility.ClearProgressBar();
		}
	}

	private static void URPToHDRP_Materials(List<Material> urpMaterials)
	{
		/****************************/
		/* Filter Materials for URP */
		/****************************/
		{
			int len = urpMaterials.Count;
			for (int i = 0; i < urpMaterials.Count; i++)
			{
				if (urpMaterials[i].shader.name.Split('/')[0] != "Universal Render Pipeline")
				{
					urpMaterials.RemoveAt(i);
					i--; // This index is removed, thus repeat this index.
				}
			}

			Debug.LogFormat("Converting Materails from URP to HDRP::\n\t" +
				"Total Targeted Material Count: {0}\n\t" +
				"           URP Material Count: {1}", len, urpMaterials.Count);
		}

		try
		{
			AssetDatabase.StartAssetEditing();

			/****************************/
			/* Get URP and HDRP shaders */
			/****************************/
			// This will ensure that all shaders are included in the project while only retrieving
			// each shader once (slower for single conversion, faster for batch)
			Shader URP_LIT = TryShaderFind("Universal Render Pipeline/Lit");
			Shader HDRP_LIT = TryShaderFind("HDRP/Lit");

			Shader URP_ST7 = TryShaderFind("Universal Render Pipeline/Nature/SpeedTree7");
			Shader HDRP_ST8 = TryShaderFind("HDRP/Nature/SpeedTree8");

			/***************************/
			/*  Convert Each Material  */
			/***************************/
			for (int i = 0; i < urpMaterials.Count; i++)
			{
				Material mat = urpMaterials[i];
				EditorUtility.DisplayProgressBar("Fixing materials", "Converting materail " + mat.name, i / (float)urpMaterials.Count);

				if (mat.shader.name.Split('/')[0] != "Universal Render Pipeline")
					continue; // Selected Asset is not URP
				try
				{
					// Switch statement for shader conversion
					if (mat.shader == URP_LIT) URPToHDRP_Lit(mat, HDRP_LIT);
					else if (mat.shader == URP_ST7) URPToHDRP_SpeedTree7(mat, HDRP_ST8);

					// Default...
					else
					{
						Debug.LogWarning("Material asset '" + mat.name + "' does not have a defined conversion for its shader: '" + mat.shader.name + "'", mat);
					}
				}
				catch (Exception e)
				{
					Debug.LogErrorFormat("Material asset failed to convert to HDRP:\n\t" +
						"Materail Name: {0}\n\t" +
						"  Shader Name: {1}\n\t" +
						"        Error: {2}\n{3}", mat.name, mat.shader, e.Message, e.StackTrace, urpMaterials[i]);
				}
			}
		}
		catch(InvalidDataException e)
		{
			Debug.LogError(e.Message + " Check to make sure URP and HDRP are included in the project. (Note, URP must be included in order to identify which shader materials start with)");
		}
		catch (Exception e)
		{
			Debug.LogError("Critical Error. Stopping conversion:\n" + e.Message + "\n" + e.StackTrace);
		}
		finally
		{
			AssetDatabase.StopAssetEditing();
			AssetDatabase.SaveAssets();

			EditorUtility.ClearProgressBar();
		}
	}

	/******************************/
	/* Shader Property Converters */
	/******************************/

	private static void URPToHDRP_Lit(Material litMaterial, Shader HDRP_LIT)
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

		Texture2D _MaskMap = LoadPackedTexure(MASK_MAP_NAME, 
			(_WorkflowMode == 0) ? null : _MetallicGlossMap,
			_OcclusionMap,
			_DetailMask,
			(_SmoothnessTextureChannel == 0) ? _MetallicGlossMap : _BaseMap
		);

		Texture2D _DetailMap = LoadPackedTexure(DETAIL_MAP_NAME, _DetailAlbedoMap, _DetailNormalMap, null);


		litMaterial.shader = HDRP_LIT;

		litMaterial.SetColor("_BaseColor", _BaseColor);
		litMaterial.SetTexture("_BaseColorMap", _BaseMap);
		litMaterial.SetTextureScale("_BaseColorMap", _BaseScale);
		litMaterial.SetTextureOffset("_BaseColorMap", _BaseOffset);

		litMaterial.SetFloat("_Metallic", _Metallic);
		litMaterial.SetFloat("_Smoothness", _Smoothness);
		// The packed textures that we create will have a default of White.
		// Thus, also set the remapping equal to the equal weight
		litMaterial.SetFloat("_MetallicRemapMax", _Metallic);
		litMaterial.SetFloat("_SmoothnessRemapMax", _Smoothness);

		litMaterial.SetTexture("_MaskMap", _MaskMap);
		litMaterial.SetTextureScale("_MaskMap", _BaseScale);
		litMaterial.SetTextureOffset("_MaskMap", _BaseOffset);
		litMaterial.SetFloat("_AORemapMax", _OcclusionStrength);

		litMaterial.SetTexture("_NormalMap", _BumpMap);
		litMaterial.SetTextureScale("_NormalMap", _BaseScale);
		litMaterial.SetTextureOffset("_NormalMap", _BaseOffset);
		litMaterial.SetFloat("_NormalScale", _BumpScale);

		if (_ParallaxMap)
		{
			litMaterial.SetTexture("_HeightMap", _ParallaxMap);
			litMaterial.SetTextureScale("_HeightMap", _BaseScale);
			litMaterial.SetTextureOffset("_HeightMap", _BaseOffset);
			litMaterial.SetFloat("_HeightPoMAmplitude", _Parallax);
			litMaterial.SetFloat("_HeightTessAmplitude", _Parallax);

			// Pixel Displacement
			litMaterial.SetInt("_DisplacementMode", 2);
		}

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

		litMaterial.SetFloat("_ReceivesSSR", _EnvironmentReflections);
		litMaterial.SetFloat("_ReceivesSSRTransparent", _EnvironmentReflections);

		litMaterial.SetFloat("_EnableBlendModePreserveSpecularLighting", (_SpecularHighlights) ? 1 : 0);
		litMaterial.SetFloat("_EnableBlendModePreserveSpecularLighting", (_SpecularHighlights) ? 1 : 0);

		if (_Cull == 0)
		{
			litMaterial.SetFloat("_DoubleSidedEnable", 1);
			litMaterial.SetFloat("_DoubleSidedNormalMode", 0);
		}
		else if (_Cull == 1)
		{
			litMaterial.SetInt("_OpaqueCullMode", 1);
			litMaterial.SetInt("_TransparentCullMode", 1);
		}

		// TODO::
		if (_QueueOffset != 0) Debug.LogWarning("Material '" + litMaterial.name + "' has a _QueueOffset (_QueueOffset is not compatible for HDRP)");

		if (!_ReceiveShadows) Debug.LogWarning("Material '" + litMaterial.name + "' is set to NOT receive shadows, which HDRP does not support. Consider using Light Layers instead.");
	}

	/// NOTES:
	/// - Detail is not supported
	/// - Face culling is forced to back. (Double sided mode is flipped normals)
	private static void URPToHDRP_SpeedTree7(Material st7Material, Shader HDRP_ST8)
	{
		bool EFFECT_BUMP = st7Material.IsKeywordEnabled("EFFECT_BUMP");
		bool ENABLE_WIND = st7Material.IsKeywordEnabled("ENABLE_WIND");
		bool EFFECT_HUE_VARIATION = st7Material.IsKeywordEnabled("EFFECT_HUE_VARIATION");
		bool GEOM_TYPE_BRANCH_DETAIL = st7Material.IsKeywordEnabled("GEOM_TYPE_BRANCH_DETAIL"); 
		bool GEOM_TYPE_BRANCH = st7Material.IsKeywordEnabled("GEOM_TYPE_BRANCH");
		bool SPEEDTREE_ALPHATEST = st7Material.IsKeywordEnabled("SPEEDTREE_ALPHATEST"); 

		 // (Base) Color
		 Color _Color = st7Material.GetColor("_Color");
		// (Base) Hue Variation
		Color _HueVariation = st7Material.GetColor("_HueVariation");
		// Albedo
		Texture2D _MainTex = (Texture2D)st7Material.GetTexture("_MainTex");
		// N/A
		Vector2 _MainScale = st7Material.GetTextureScale("_MainTex");
		// N/A
		Vector2 _MainOffset = st7Material.GetTextureOffset("_MainTex");

		if (GEOM_TYPE_BRANCH_DETAIL)
		{
			Debug.LogWarning("SpeedTree7 Material '" + st7Material.name + "' is set to 'Branch Detail'. Detail map will not be preserved.", st7Material);

			//NOTE:: Detail is not compatable with HDRP SpeedTree8
			//Texture2D _DetailTex = (Texture2D)st7Material.GetTexture("_DetailTex");
			//Vector2 _DetailScale = st7Material.GetTextureScale("_DetailTex");
			//Vector2 _DetailOffset = st7Material.GetTextureOffset("_DetailTex");
		}

		// Normal Map
		Texture2D _BumpMap = (Texture2D)st7Material.GetTexture("_BumpMap");
		// N/A
		Vector2 _BumpScale = st7Material.GetTextureScale("_BumpMap");
		// N/A
		Vector2 _BumpOffset = st7Material.GetTextureOffset("_BumpMap");

		// (Alpha) Threshold: Boolean
		float _Cutoff = st7Material.GetFloat("_Cutoff");
		// Render Face: Front = 2, Back = 1, Both = 0
		int _Cull = (int)st7Material.GetFloat("_Cull");
		// Wind Quality: None = 0, Fastest = 1, Fast = 2, Better = 3, Best = 4, Palm = 5
		int _WindQuality = (int)st7Material.GetFloat("_WindQuality");

		st7Material.shader = HDRP_ST8;

		// Base Map
		st7Material.SetTexture("_MainTex", _MainTex);
		st7Material.SetTextureScale("_MainTex", _MainScale);
		st7Material.SetTextureOffset("_MainTex", _MainOffset);

		// Color Tint
		st7Material.SetColor("_Color", _Color);
		// Enable Hue Variation (bool)
		st7Material.SetFloat("_HueVariationKwToggle", EFFECT_HUE_VARIATION ? 1 : 0); 
		// Hue Variation Color
		st7Material.SetColor("_HueVariationColor", _HueVariation);

		// Normal Mapping (bool)
		st7Material.SetFloat("_NormalMapKwToggle", EFFECT_BUMP ? 1 : 0);
		// Normal Map
		st7Material.SetTexture("_BumpMap", _BumpMap);
		st7Material.SetTextureScale("_BumpMap", _BumpScale);
		st7Material.SetTextureOffset("_BumpMap", _BumpScale);

		// Enable Extra Map (bool)
		st7Material.SetFloat("EFFECT_EXTRA_TEX", 0);
		// Smoothness (R), Met (G), AO (B)
		st7Material.SetTexture("_ExtraTex", null); 
		
		// Smoothness
		st7Material.SetFloat("_Glossiness", GEOM_TYPE_BRANCH ? 0 : 1);
		// Subsurface Map
		st7Material.SetTexture("_SubsurfaceTex", null);
		// Subsurface Scale
		st7Material.SetFloat("_SubsurfaceScale", 1);

		st7Material.SetVector("Diffusion_Profile_Asset", defaultDiffusionAsset_V4);
		st7Material.SetFloat("Diffusion_Profile", defaultDiffusionAsset_f);

		// Alpha Clip Threshold
		st7Material.SetFloat("_AlphaClipThreshold", SPEEDTREE_ALPHATEST ? _Cutoff : 0);
		// Enable Wind
		st7Material.SetFloat("_WindQuality", ENABLE_WIND ? 1 : 0);
		// Wind Quality 
		st7Material.SetFloat("_WINDQUALITY", _WindQuality);

		
	}

	/****************************/
	/* Creating Packed Textuers */
	/****************************/

	private static void CreatePackedTextures_URPLit(Material litMaterial)
	{
		int _WorkflowMode = (int)litMaterial.GetFloat("_WorkflowMode");
		float _SmoothnessTextureChannel = litMaterial.GetFloat("_SmoothnessTextureChannel");
		Texture2D _BaseMap = (Texture2D)litMaterial.GetTexture("_BaseMap");
		Texture2D _MetallicGlossMap = (Texture2D)litMaterial.GetTexture("_MetallicGlossMap");
		Texture2D _OcclusionMap = (Texture2D)litMaterial.GetTexture("_OcclusionMap");
		Texture2D _DetailMask = (Texture2D)litMaterial.GetTexture("_DetailMask");
		Texture2D _DetailAlbedoMap = (Texture2D)litMaterial.GetTexture("_DetailAlbedoMap");
		Texture2D _DetailNormalMap = (Texture2D)litMaterial.GetTexture("_DetailNormalMap");

		CreatePackedMaskMap((_WorkflowMode == 0) ? null : _MetallicGlossMap, _OcclusionMap, _DetailMask, (_SmoothnessTextureChannel == 0) ? _MetallicGlossMap : _BaseMap);
		CreatePackedDetailMap(_DetailAlbedoMap, _DetailNormalMap, null);
	}

	private static void CreatePackedMaskMap(Texture2D metallic, Texture2D occlusion, Texture2D height, Texture2D smoothness)
	{
		if (!(metallic || occlusion || height)) return;

		string filePath = MASK_MAP_NAME;
		bool exists = GetReadableTextures(out ReadableTexture[] twis, ref filePath, out Texture2D maskMap, metallic, occlusion, height, smoothness);

		if (exists) return;

		Color temp = Color.white;
		for (int i = 0; i < maskMap.height; ++i)
		{
			for (int j = 0; j < maskMap.width; ++j)
			{
				float normx = (float)j / (float)maskMap.width;
				float normy = (float)i / (float)maskMap.height;
				temp.r = twis[0].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.g = twis[1].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.b = twis[2].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.a = twis[3].texture.GetPixelBilinear(normx, normy).a;
				maskMap.SetPixel(j, i, temp);
			}
		}
		maskMap.Apply();

		SaveTexture(maskMap, filePath);
	}

	private static void CreatePackedDetailMap(Texture2D albedo, Texture2D normal, Texture2D smoothness)
	{
		if (!(albedo || normal || smoothness)) return;

		string filePath = DETAIL_MAP_NAME;
		bool exists = GetReadableTextures(out ReadableTexture[] twis, ref filePath, out Texture2D detailmap, albedo, normal, smoothness);

		if (exists) return;

		

		Color temp = Color.white;
		for (int i = 0; i < detailmap.height; ++i)
		{
			for (int j = 0; j < detailmap.width; ++j)
			{
				float normx = (float)j / (float)detailmap.width;
				float normy = (float)i / (float)detailmap.height;

				temp.r = twis[0].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.g = twis[1].texture.GetPixelBilinear(normx, normy).g;
				temp.b = twis[2].texture.GetPixelBilinear(normx, normy).grayscale;
				temp.a = twis[1].texture.GetPixelBilinear(normx, normy).r; detailmap.SetPixel(j, i, temp);
			}
		}
		detailmap.Apply();

		SaveTexture(detailmap, filePath);
	}

	private static void SaveTexture(Texture2D texture, string path)
	{
		File.WriteAllBytes(Application.dataPath + "/../" + path, texture.EncodeToPNG());
		UnityEngine.Object.DestroyImmediate(texture);
	}

	private static Texture2D LoadPackedTexure(string typeName, params Texture2D[] textures)
	{
		return AssetDatabase.LoadAssetAtPath<Texture2D>(
			GetPathFromReadableTextures(CreateReadableTextures(textures), typeName)
		);
	}

	private static ReadableTexture[] CreateReadableTextures(params Texture2D[] textures)
	{
		ReadableTexture[] iResult = new ReadableTexture[textures.Length];
		for (int i = 0; i < iResult.Length; i++)
		{
			iResult[i] = new ReadableTexture(textures[i]);
		}
		return iResult;
	}

	private static string GetPathFromReadableTextures(ReadableTexture[] readables, string typeName)
	{
		string combinedPath = "";
		string path = "";
		string textureName = typeName + "_";
		for (int i = 0; i < readables.Length; i++)
		{
			combinedPath += readables[i].path;

			if (!readables[i].isFile)
				textureName += "-";
			else
			{
				if (path == "") path = readables[i].path.Substring(0, readables[i].path.LastIndexOf("/") + 1);
				switch (i)
				{
					case 0: textureName += "r"; break;
					case 1: textureName += "g"; break;
					case 2: textureName += "b"; break;
					case 3: textureName += "a"; break;
					default: textureName += "X"; break;
				}
			}
		}

		return string.Format("{0}{1}_{2,00000000:X}.png", path, textureName, combinedPath.GetHashCode());
	}

	private static bool GetReadableTextures(out ReadableTexture[] iResult, ref string textureName, out Texture2D tResult, params Texture2D[] textures)
	{
		iResult = CreateReadableTextures(textures);
		textureName = GetPathFromReadableTextures(iResult, textureName);

		if (File.Exists(textureName))
		{
			tResult = null;
			return true;
		}


		Vector2Int maxSize = Vector2Int.zero;
		foreach (ReadableTexture t in iResult)
		{
			t.MakeReadable();
			if (t.texture.width > maxSize.x)
				maxSize.x = t.texture.width;
			if (t.texture.height > maxSize.y)
				maxSize.y = t.texture.height;
		}

		tResult = new Texture2D(maxSize.x, maxSize.y, DEFAULT_MASKMAP_FORMAT, false);

		return false;
	}

	public class ReadableTexture
	{
		public ReadableTexture(Texture2D texture, bool whiteFallback = true)
		{
			if (!texture)
			{
				this.texture = whiteFallback ? Texture2D.whiteTexture : Texture2D.blackTexture;
				path = this.texture.name;
				isFile = false;
			}
			else
			{
				this.texture = texture;
				path = AssetDatabase.GetAssetPath(texture);
				isFile = path != "";
			}
		}

		~ReadableTexture()
		{
			if (copied) UnityEngine.Object.DestroyImmediate(this.texture);
		}

		public Texture2D texture { get; private set; }
		public readonly string path;
		public readonly bool isFile;

		private bool copied;

		public void MakeReadable()
		{
			if (this.copied || !isFile) return;

			//byte[] pix = texture.GetRawTextureData();
			//Texture2D readableText = new Texture2D(texture.width, texture.height, texture.format, false);
			//readableText.LoadRawTextureData(pix);
			//readableText.Apply();
			//this.texture = readableText;
			//this.copied = true;

			// Using render texture to convert texture formats
			// (if texture is compressed, some pixel functions won't work)
			RenderTexture renderTex = RenderTexture.GetTemporary(
				texture.width,
				texture.height,
				0,
				RenderTextureFormat.Default,
				RenderTextureReadWrite.Linear);

			Graphics.Blit(texture, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(texture.width, texture.height);
			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			this.texture = readableText;
			this.copied = true;
		}
	}

	
}