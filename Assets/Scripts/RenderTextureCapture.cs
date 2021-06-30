using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCapture : MonoBehaviour
{
    public const string DEFAULT_CAPTURE_NAME = "RenderTextureCapture-Capture";
    public const string DEFAULT_PHOTO_NAME = "RenderTextureCapture-Photo";

    public RenderTexture renderTexture;
    [HideInInspector]
    public List<Texture2D> PhotoCaptures;

    private void Start()
    {
        if (renderTexture == null)
            Debug.LogWarning("Render Texture must be set! Component not set up for creating and rendering it's own textures.", this);

        PhotoCaptures = new List<Texture2D>();
    }


    public void CapturePhoto()
    {
        PhotoCaptures.Add(RenderToTexture2D(renderTexture));
    }

    public void SaveCaptures(string path = "", string name = DEFAULT_CAPTURE_NAME)
    {
        for (int i = 0; i < PhotoCaptures.Count; i++)
            SaveCapture(i, path, name);
    }

    public void SaveCapture(int index, string path = "", string name = DEFAULT_CAPTURE_NAME)
    {
        SaveTexture(PhotoCaptures[index], path, name);
    }


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
