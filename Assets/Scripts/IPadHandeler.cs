using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ParentConstraint))]
public class IPadHandeler : MonoBehaviour
{
    public float BeltRadius;
    public float TimeToReturnAfterThrow = 4f;

    // Attached Components::
    private ParentConstraint _parentConstraint;
    private Rigidbody _rigidbody;
    // ---------------------

    /// <summary>
    /// Used to prevent double calling the relese after the wait time. Only called once, when this count is 1. This value is negative while the ipad is being held.
    /// </summary>
    private int releseCalls = 0;

    // Start is called before the first frame update
    void Start()
    {
        _parentConstraint = GetComponent<ParentConstraint>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IPadPickedUp()
    {
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _parentConstraint.enabled = false;

        releseCalls = -releseCalls;
    }

    public void IPadRelesed()
    {
        //Debug.Log("IPad Distance: " + Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position));

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
