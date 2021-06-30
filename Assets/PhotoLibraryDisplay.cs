using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoLibraryDisplay : MonoBehaviour
{
    public RenderTextureCapture RenderTextureCapture;
    public Text[] textPage;
    public GameObject textNextPage;
    public GameObject textPrevPage;
    public RawImage[] imagePage;
    public GameObject imageNextPage;
    public GameObject imagePrevPage;

    private int groupNumber;
    private string library;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(RenderTextureCapture != null, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a RenderTextureCapture to reference.", this);
        Debug.Assert(textPage.Length > 0, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a lest one to Text to reference.", this);
        Debug.Assert(imagePage.Length > 0, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a lest one to Image to reference.", this);
    }

    public void UpdateTextPage()
    {
        foreach (var item in textPage)
        {
            item.gameObject.SetActive(false);
        }

        int i = groupNumber * textPage.Length;
        int index = 0;
        foreach (string library in RenderTextureCapture.PhotoLibraries.Keys)
        {
            if (i >= groupNumber * textPage.Length)
            {
                if (i >= (1 + groupNumber) * textPage.Length || i >= RenderTextureCapture.PhotoLibraries.Count)
                    break;
                else
                {
                    textPage[index].gameObject.SetActive(true);
                    textPage[index].text = library;
                    index++;
                }

            }
            i++;
        }

        if (i < RenderTextureCapture.PhotoLibraries.Count)
        {
            textNextPage.SetActive(true);
        }
        else
            textNextPage.SetActive(false);

        if (groupNumber == 0)
            textPrevPage.SetActive(false);
        else
            textPrevPage.SetActive(true);
    }

    public void UpdateImagePage()
    {
        foreach (var item in imagePage)
        {
            item.gameObject.SetActive(false);
        }

        int i = 0;
        int index = 0;
        foreach (Texture2D image in RenderTextureCapture.PhotoLibraries[library])
        {
            if (i >= groupNumber * imagePage.Length)
            {
                if (index >= imagePage.Length || i >= RenderTextureCapture.PhotoLibraries[library].Count)
                    break;
                else
                {
                    imagePage[index].gameObject.SetActive(true);
                    imagePage[index].texture = image;
                    index++;
                }

            }
            i++;
        }

        if (i < RenderTextureCapture.PhotoLibraries[library].Count)
        {
            imageNextPage.SetActive(true);
        }
        else
            imageNextPage.SetActive(false);

        if (groupNumber == 0)
            imagePrevPage.SetActive(false);
        else
            imagePrevPage.SetActive(true);
    }

    public void SetLibraryFromIndex(int index)
    {
        index += groupNumber * textPage.Length;
        int i = 0;
        foreach (string library in RenderTextureCapture.PhotoLibraries.Keys)
        {
            if (i == index)
            {
                this.library = library;
                return;
            }
            i++;
        }

        Debug.LogError("Index out of bounds.");
    }

    public void SetGroupNumber(int number)
    {
        groupNumber = number;
    }

    public void IncrementGroupNumber()
    {
        groupNumber++;
    }

    public void DecrementGroupNumber()
    {
        groupNumber--;
    }

    public void SaveCurrentLib()
    {
        RenderTextureCapture.SaveLibrary(library);
    }

}
