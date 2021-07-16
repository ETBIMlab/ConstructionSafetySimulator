using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoLibraryDisplay : MonoBehaviour
{
    public RenderTextureCapture RenderTextureCapture;
    public Text EmptyWarning;
    public Text[] textPage;
    public GameObject textNextPage;
    public GameObject textPrevPage;
    public RawImage[] imagePage;
    public GameObject imageNextPage;
    public GameObject imagePrevPage;
    public RawImage EnhancedImaged;

    private int groupNumber;
    private string library;
    private List<string> valids;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(RenderTextureCapture != null, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a RenderTextureCapture to reference.", this);
        Debug.Assert(textPage.Length > 0, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a lest one to Text to reference.", this);
        Debug.Assert(imagePage.Length > 0, "The PhotoLibraryDisplay is a specified script designed for one task. There should always be a lest one to Image to reference.", this);
    }

    private void OnDisable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void UpdateTextPage()
    {
        // Disable all text boxes. We will turn them on as needed
        EmptyWarning.gameObject.SetActive(false);
        foreach (var item in textPage)
        {
            item.gameObject.SetActive(false);
        }

        // Find the list of valid libraries (not null/empty).
        valids = new List<string>();
        foreach (string library in RenderTextureCapture.PhotoLibraries.Keys)
            if (RenderTextureCapture.PhotoLibraries[library] != null && RenderTextureCapture.PhotoLibraries[library].Count != 0)
                valids.Add(library);

        //
        // Display page content
        //

        // There is nothing to display, but we should still
        if (valids.Count == 0)
        {
            
            EmptyWarning.gameObject.SetActive(true);
            EmptyWarning.text = "There are no Photos taken yet. Open the camera app to take some photos!";
        }

        // Change placeholders to library names:
        int i = groupNumber * textPage.Length;
        int index = 0;
        foreach (string library in valids)
        {
            if (i >= groupNumber * textPage.Length)
            {
                if (i >= (1 + groupNumber) * textPage.Length || i >= valids.Count)
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

        // Update the next/previous page buttons
        if (i < valids.Count)
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
        EnhancedImaged.gameObject.SetActive(false);
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
        foreach (string library in valids)
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

    public void EnhanceImage(int index)
    {
        EnhancedImaged.gameObject.SetActive(true);
        EnhancedImaged.texture = imagePage[index].texture;
    }

}
