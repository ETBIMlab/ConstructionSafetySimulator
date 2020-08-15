using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    public bool hide;
    public GameObject target;
    private Animator _animator;

    //Start
    private void Start()
    {
        _animator = target.GetComponent<Animator>();
    }

    // Causes the target object to show if hidden. Check "hide" to hide object on trigger 
    void OnTriggerEnter()
    {
        //target.SetActive(!hide);
        _animator.Play("animatetractor",0, 0.25f);
    }

}
