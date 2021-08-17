using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ParentConstraint))]
public class RadioHandler : MonoBehaviour
{
    public const int FREQUENCY = 44100;

    public float BeltRadius;
    public float TimeToReturnAfterThrow = 4f;
    public AudioSource AudioLocation;
    
    // Attached Components::
    private ParentConstraint _parentConstraint;
    private Rigidbody _rigidbody;
    // ---------------------

    /// <summary>
    /// Used to prevent double calling the relese after the wait time. Only called once, when this count is 1. This value is negative while the ipad is being held.
    /// </summary>
    private int releseCalls = 0;

    // Start is called before the first frame update
    private void Start()
    {
        MicrophoneSettings.OnMicChanged += TryUpdateMicrophoneSource;

        if (AudioLocation == null)
        {
            Debug.LogError("RadioHandeler must have an assigned AudioSource", this);

        }
        else
            UpdateMicrophoneSource();

        _parentConstraint = GetComponent<ParentConstraint>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void TryUpdateMicrophoneSource(string _)
    {
            this.UpdateMicrophoneSource();
    }

    public void UpdateMicrophoneSource()
    {
        StartCoroutine(UpdateMicrophoneSourceCoroutine());
    }

    private IEnumerator UpdateMicrophoneSourceCoroutine()
    {
        AudioLocation.clip = Microphone.Start(MicrophoneSettings.Mic, true, 10, FREQUENCY);

        if (AudioLocation.clip != null)
        {
            AudioLocation.loop = true;
            while (!(Microphone.GetPosition(null) > 0))
                yield return null;

            AudioLocation.Play();
        }
        else
        {
            Debug.LogError("Microphone.Start() failed to create an audio clip.");
        }
    }


    public void RadioPickedUp()
    {
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _parentConstraint.enabled = false;

        releseCalls = -releseCalls;
    }

    public void RadioRelesed()
    {
        //Debug.Log("IPad Distance: " + Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position));

        if (Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position) < BeltRadius)
            ReturnRadio();
        else
        {
            if (releseCalls < 0) releseCalls = (-releseCalls) + 1;
            else releseCalls++;

            StartCoroutine(ReturnToStartAfterTime(TimeToReturnAfterThrow));
        }
    }

    public void ReturnRadio()
    {
        _parentConstraint.enabled = true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    private IEnumerator ReturnToStartAfterTime(float seconds)
    {
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        yield return new WaitForSeconds(seconds);

        if (releseCalls == 1)
        {
            ReturnRadio();

        }

        if (releseCalls < 0) releseCalls++;
        else releseCalls--;
    }

    void OnDestroy()
    {
        Microphone.End(MicrophoneSettings.Mic);
        MicrophoneSettings.OnMicChanged -= TryUpdateMicrophoneSource;
    }
}
