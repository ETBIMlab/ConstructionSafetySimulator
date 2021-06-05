using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnLate : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("DONL");
    }


    public Behaviour comp;

    private IEnumerator DONL()
    {
        yield return new WaitForEndOfFrame();
        if (comp != null)
            comp.enabled = false;
        this.enabled = false;
    }
}
