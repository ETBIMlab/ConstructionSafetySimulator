using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ParentConstraint))]
[RequireComponent(typeof(Interactable))]
public class IPadHandeler : MonoBehaviour
{
    public float BeltRadius;
    public float TimeToReturnAfterThrow = 4f;
    public SteamVR_Skeleton_Poser otherHandPoser;

    // Attached Components::
    private ParentConstraint _parentConstraint;
    private Rigidbody _rigidbody;
    private Interactable _interactable;
    // ---------------------

    private Action<Hand.AttachedObject> overrideAttachedChecker;

    /// <summary>
    /// Used to prevent double calling the relese after the wait time. Only called once, when this count is 1. This value is negative while the ipad is being held.
    /// 
    /// Developement note: There is a better way to implement this. This should be implemented as a coroutine which is stopped depending on if the ipad is picked up.
    /// </summary>
    private int releseCalls = 0;

    // Start is called before the first frame update
    void Start()
    {
        _parentConstraint = GetComponent<ParentConstraint>();
        _rigidbody = GetComponent<Rigidbody>();
        _interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///<summary>
    /// Method is called by the SteamVr Hand object when a grab is started. Used to handle specific wanted changes not available using UnityEvents
    /// </summary> 
    protected virtual void OnAttachedToHand(Hand hand)
    {
        // Set secondary hand to pointing only when the hand is empty
        // The following is to add listeners for when the secondary hand is changing attached objects.
        if (IsHandPoseValid(hand.otherHand))
        {
            /* :::Check for when the other hand is holding another object::: */
            overrideAttachedChecker = (attachedObject) =>
            {
                // If the other hand is not holding anything,
                // OR if the other hand was holding only one thing which it is currently dropping,
                // Set the other hand to point pose.
                if (hand.otherHand.AttachedObjects.Count <= 0 || 
                (hand.otherHand.AttachedObjects.Count == 1 && hand.otherHand.AttachedObjects.Contains(attachedObject)))
                {
                    hand.otherHand.skeleton.BlendToPoser(otherHandPoser);
                }
            };
            hand.otherHand.onObjectDetachedToHand += overrideAttachedChecker;

            /* :::Set point pose now::: */
            if (hand.otherHand.AttachedObjects.Count <= 0)
                    hand.otherHand.skeleton.BlendToPoser(otherHandPoser);
        }
        // end of secondary hand posing.

        // Enable IPad to follow hand.
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _parentConstraint.enabled = false;

        releseCalls = -releseCalls;
    }

    public bool IsHandPoseValid(Hand hand)
    {
        return hand != null && hand.skeleton != null && otherHandPoser != null;
    }

    ///<summary>
    /// Method is called by the SteamVr Hand object when a grab is ended. Used to handle specific wanted changes not available using UnityEvents
    /// </summary> 
    protected virtual void OnDetachedFromHand(Hand hand)
    {
        //Debug.Log("IPad Distance: " + Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position));

        hand.otherHand.onObjectDetachedToHand -= overrideAttachedChecker;

        // Return the secondary hand to open hand if hand is open
        if (hand.otherHand.AttachedObjects.Count <= 0 && IsHandPoseValid(hand.otherHand))
        {
            // 0.2f is the standard release time in the interactable.cs script.
            hand.otherHand.skeleton.BlendToSkeleton(0.2f);
        }

        // Checks to see if the IPad is being handed off
        foreach (ForemanHandeler fh in ForemanHandeler.instances)
        {
            if (fh.IsIPadHandOff())
            {
                fh.IPadHandedOff();

                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;

                int count = _parentConstraint.sourceCount;
                GameObject go = new GameObject("Constraint fade");
                go.hideFlags = HideFlags.HideAndDontSave;
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;

                ConstraintSource c = new ConstraintSource();
                c.sourceTransform = go.transform;
                _parentConstraint.AddSource(c);
                _parentConstraint.translationOffsets[count] = transform.position;
                _parentConstraint.rotationOffsets[count] = transform.rotation.eulerAngles;

                ChangeConstraintToIndex(2); // Essentailly sets partent to the last location the ipad is at
                FadeConstraintChange(1, 2); // fades the partent to the formans left hand

                _parentConstraint.enabled = true;

                return;
            }
        }

        if (Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position) < BeltRadius)
            ReturnIPad();
        else
        {
            if (releseCalls < 0) releseCalls = (-releseCalls) + 1;
            else releseCalls++;

            StartCoroutine(ReturnToStartAfterTime(TimeToReturnAfterThrow));
        }
    }

    public void ReturnIPad()
    {
        _parentConstraint.enabled = true;
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
    }

    public void ChangeConstraintToIndex(int index)
    {
        ConstraintSource source;
        for (int i = 0; i < _parentConstraint.sourceCount; i++)
        {

            if (i == index)
            {
                source = _parentConstraint.GetSource(i);
                source.weight = 1;
                _parentConstraint.SetSource(i, source);
            }
            else
            {
                source = _parentConstraint.GetSource(i);
                source.weight = 0;
                _parentConstraint.SetSource(i, source);
            }
        }
    }

    public void FadeConstraintChange(int newindex, int fadeindex)
    {
        StartCoroutine(FadeConstraintChange_coroutine(newindex, fadeindex));
    }

    private IEnumerator FadeConstraintChange_coroutine(int newindex, int fadeindex)
    {
        float time = 0;
        float alpha = 0;
        ConstraintSource source;
        while (alpha < 1)
        {
            yield return null;
            time += Time.deltaTime;
            alpha = Mathf.Clamp01(time / 0.5f);
            for (int i = 0; i < _parentConstraint.sourceCount; i++)
            {

                if (i == newindex)
                {
                    source = _parentConstraint.GetSource(i);
                    source.weight = alpha;
                    _parentConstraint.SetSource(i, source);
                }
                else if (i == fadeindex)
                {
                    source = _parentConstraint.GetSource(i);
                    source.weight = 1 - alpha;
                    _parentConstraint.SetSource(i, source);
                }
            }
        }
    }

    private IEnumerator ReturnToStartAfterTime(float seconds)
    {
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        yield return new WaitForSeconds(seconds);

        if (releseCalls == 1)
        {
            ReturnIPad();
        }

        if (releseCalls < 0) releseCalls++;
        else releseCalls--;
    }
}
