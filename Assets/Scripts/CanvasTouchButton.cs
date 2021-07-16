using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A script that acts as a button trigger enter and exit and only activates after a given time has passed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CanvasTouchButton : MonoBehaviour
{
    public enum ScaleAxis { None, X, Y, Z, XY, XZ, YZ, XYZ }

    [Header("Button Properties")]
    [Tooltip("The hover time required to activate the button")]
    [Min(0)]
    public float selectTime = 1f;
    [Tooltip("The Image that is scaled up to display time remaining. Original Color on image is used for incomplete.")]
    public Image ProgressImage;
    [Tooltip("Once the Image has scaled up all the way, the image color will change to match the active color to indicate once the finger moves, the button will be pressed.")]
    public Color ActiveColor = Color.green;
    [Tooltip("The Axis that the image should scale up with. Pair with an offset for different effects like a progress bar.")]
    public ScaleAxis TargetScaleAxis = ScaleAxis.None;

    [Header("Events")]
    public UnityEvent ButtonEnter;
    public UnityEvent ButtonStay;
    public UnityEvent ButtonExit;
    [Tooltip("Event for when the button is activated")]
    public UnityEvent OnSelected;


    // Private variables

    /// <summary>
    /// Counts the time that has pasted since the 
    /// </summary>
    private float timeEnterTime;

    /// <summary>
    /// A save state of the original scale of the image. 
    /// </summary>
    private Vector3 MaxScale;

    /// <summary>
    /// A save state of the original image color. This represents the progress is not complete.
    /// </summary>
    private Color startColor;

    /// <summary>
    /// Trigger enter and trigger exit should only care about when the last and first colliders enter this gameobject. This keeps track of the number of colliders colliding with the trigger.
    /// </summary>
    private int numberEnter;

    private void Awake()
    {
        if (ProgressImage == null)
        {
            if (selectTime == 0)
                Debug.LogWarning("Progress Image on CanvasTouchButton is null AND time is set to 0. If this is intended, consider using the 'OnTriggerHandler' instead.");
        }
        else
        {
            MaxScale = ProgressImage.transform.localScale;
            ProgressImage.transform.localScale = Vector3.zero;
            startColor = ProgressImage.color;
        }
    }

    public void OnEnable()
    {
        numberEnter = 0;
    }

    public void OnDisable()
    {
        numberEnter = 0;

        if (ProgressImage != null)
        {
            ProgressImage.gameObject.SetActive(false);
            ProgressImage.transform.localScale = MaxScale;
            ProgressImage.color = startColor;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        numberEnter++;
        if (numberEnter == 1)
        {
            timeEnterTime = Time.time;

            if (ProgressImage != null)
            {
                ProgressImage.transform.localScale = Vector3.zero;
                ProgressImage.gameObject.SetActive(true);
            }

            

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (ProgressImage == null)
            return;

        float progress = Mathf.Clamp01((Time.time - timeEnterTime) / selectTime);

        if (progress == 1)
        {
            ProgressImage.color = ActiveColor;
        }

        Vector3 scl = MaxScale;
        switch (TargetScaleAxis)
        {
            case ScaleAxis.None:
                break;
            case ScaleAxis.X:
                scl.x *= progress;
                break;
            case ScaleAxis.Y:
                scl.y *= progress;
                break;
            case ScaleAxis.Z:
                scl.z *= progress;
                break;
            case ScaleAxis.XY:
                scl.x *= progress;
                scl.y *= progress;
                break;
            case ScaleAxis.XZ:
                scl.x *= progress;
                scl.z *= progress;
                break;
            case ScaleAxis.YZ:
                scl.y *= progress;
                scl.z *= progress;
                break;
            case ScaleAxis.XYZ:
                scl.x *= progress;
                scl.y *= progress;
                scl.z *= progress;
                break;
        }

        ProgressImage.transform.localScale = scl;
    }

    private void OnTriggerExit(Collider other)
    {
        numberEnter--;
        if (numberEnter == 0)
        {
            if (ProgressImage != null)
            {
                ProgressImage.gameObject.SetActive(false);
                ProgressImage.transform.localScale = MaxScale;
                ProgressImage.color = startColor;
            }

            if ((Time.time - timeEnterTime) / selectTime >= 1)
            {
                OnSelected.Invoke();
            }
        }
        
    }

    private void OnValidate()
    {
        if (ProgressImage != null && ProgressImage.gameObject == gameObject)
            Debug.LogWarning("The 'CanvasTouchButton' script scales and deacitvates the gameobject attatched to the Selector [Image]. The image gameobject should not be on the same gameobject or a parent of the the 'CanvasTouchButton' component.");

        if (ProgressImage == null && selectTime == 0)
                Debug.LogWarning("Progress Image on CanvasTouchButton is null AND time is set to 0. If this is intended, consider using the 'OnTriggerHandler' instead.");
    }
}
