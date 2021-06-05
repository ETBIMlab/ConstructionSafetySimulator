using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IPadHandeler : MonoBehaviour
{
    public float BeltRadius;
    public float TimeToReturnAfterThrow = 4f;
    private ParentConstraint _parentConstraint;
    private bool throwMutex = false;

    // Start is called before the first frame update
    void Start()
    {
        _parentConstraint = GetComponent<ParentConstraint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IPadRelesed()
    {
        if (Vector3.Distance(_parentConstraint.GetTranslationOffset(0) + _parentConstraint.GetSource(0).sourceTransform.position, transform.position) < BeltRadius)
            _parentConstraint.enabled = true;
        else
            StartCoroutine(ReturnToStartAfterTime(TimeToReturnAfterThrow));
    }

    private IEnumerator ReturnToStartAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        _parentConstraint.enabled = true;
    }
}
