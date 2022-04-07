using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class Upgrade_URPToHDRP : EditorWindow
{
	private const string MASK_MAP_NAME = "MaskMap";
	private const string DETAIL_MAP_NAME = "DetailMap";
	private const TextureFormat DEFAULT_MASKMAP_FORMAT = TextureFormat.RGBA32;

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
	
	[MenuItem("Edit/Render Pipeline/Selected With-Packed URP-->HDRP")]
	static void Selected_URPToHDRP_WithPacked()
	{
		Selected_UPR_PackedTextures();
		Selected_URPToHDRP();
	}


	[MenuItem("Edit/Render Pipeline/Selected NO-Packed URP-->HDRP")]
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

				if (mat.shader == URP_LIT)
				{
					try { CreatePackedTextures_URPLit(mat); }
					catch (Exception e)
					{
						Debug.LogError("Material asset '" + urpMaterials[i].name +
						  "' (" + mat.shader.name + ") failed to create packed texture: " + e.Message + "\n" + e.StackTrace, urpMaterials[i]);
					}
				}
				// Default...
				else
				{
					Debug.LogWarning("Material asset '" + mat.name + "' does not have a defined conversion for its shader: '" + mat.shader.name + "'", mat);
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

			/****************************/
			/* Get URP and HDRP shaders */
			/****************************/
			// This will ensure that all shaders are included in the project while only retrieving
			// each shader once (slower for single conversion, faster for batch)
			Shader URP_LIT = TryShaderFind("Universal Render Pipeline/Lit");
			Shader HDRP_LIT = TryShaderFind("HDRP/Lit");

			/***************************/
			/*   Convert Each Shader   */
			/***************************/
			for (int i = 0; i < urpMaterials.Count; i++)
			{
				Material mat = urpMaterials[i];
				EditorUtility.DisplayProgressBar("Fixing materials", "Converting materail " + mat.name, i / (float)urpMaterials.Count);

				if (mat.shader.name.Split('/')[0] != "Universal Render Pipeline")
					continue; // Selected Asset is not URP

				if (mat.shader == URP_LIT)
				{
					try { URPToHDRP_LitMaterial(mat, HDRP_LIT); }
					catch (Exception e) 
						{ Debug.LogError("Material asset '" + urpMaterials[i].name + 
							"' (URP Lit) failed to convert to HDRP: " + e.Message + "\n" + e.StackTrace, urpMaterials[i]); }
				}
				// Default...
				else
				{
					Debug.LogWarning("Material asset '" + mat.name + "' does not have a defined conversion for its shader: '" + mat.shader.name + "'", mat);
				}
			}
		}
		catch(InvalidDataException e)
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
			AssetDatabase.SaveAssets();

			EditorUtility.ClearProgressBar();
		}
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