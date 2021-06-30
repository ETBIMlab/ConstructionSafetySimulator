using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCapture : MonoBehaviour
{
    public const string DEFAULT_CAPTURE_NAME = "RenderTextureCapture-Capture";
    public const string DEFAULT_PHOTO_NAME = "RenderTextureCapture-Photo";
    public const string DEFAULT_Library_NAME = "Default";

    public RenderTexture renderTexture;
    [HideInInspector]
    public Dictionary<string, List<Texture2D>> PhotoLibraries;

    private void Start()
    {
        if (renderTexture == null)
            Debug.LogWarning("Render Texture must be set! Component not set up for creating and rendering it's own textures.", this);

        PhotoLibraries = new Dictionary<string, List<Texture2D>>() {
            { DEFAULT_Library_NAME, new List<Texture2D>() }
        };
    }

    #region CapturePhotos
    public void CapturePhoto()
    {
        CapturePhoto(DEFAULT_Library_NAME);
    }

    public void CapturePhoto(string library)
    {
        if (!PhotoLibraries.ContainsKey(library) || PhotoLibraries[library] == null)
            PhotoLibraries[library] = new List<Texture2D>();
         
        PhotoLibraries[library].Add(RenderToTexture2D(renderTexture));
    }
    #endregion

    #region SavePhotos

    public void SaveLibraries(string path = "", string name = DEFAULT_CAPTURE_NAME)
    {
        foreach (string library in PhotoLibraries.Keys)
        {
            SaveLibrary(library, path, name);
        }
    }

    public void SaveLibrary(string library, string path = "", string name = DEFAULT_CAPTURE_NAME)
    {
        Debug.Assert(!PhotoLibraries.ContainsKey(library), "RenderTextureCaptuer is trying to save a library that does not exist: " + library, this);

        name = library + "_" + name;

        for (int i = 0; i < PhotoLibraries[library].Count; i++)
            SaveCapture(library, i, path, name);
    }

    public void SaveCapture(string library, int index, string path = "", string name = DEFAULT_CAPTURE_NAME)
    {
        SaveTexture(PhotoLibraries[library][index], path, name);
    }

    #endregion

    #region TakePhotos
    public void TakeAndSavePhoto()
    {
        TakeAndSavePhoto("");
    }

    public void TakeAndSavePhoto(string path)
    {
        TakeAndSavePhoto(path, DEFAULT_PHOTO_NAME);
    }

    public void TakeAndSavePhoto(string path, string name)
    {
        SaveTexture(RenderToTexture2D(renderTexture), path, name);
    }
    #endregion

    private static Texture2D RenderToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    private static void SaveTexture(Texture2D texture, string path, string name)
    {
        if (path == "")
            path = Application.dataPath;

        if (!(path.EndsWith("\\") || path.EndsWith("/")))
            path += "\\";

        name += DateTime.Now.Ticks.GetHashCode().ToString("x").ToUpper();

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path + name + ".png", bytes);
    }
}
