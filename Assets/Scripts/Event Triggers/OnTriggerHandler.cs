using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
* Author: Nevin Foster
* Adds UnityEvents to object triggers
*/
public class OnTriggerHandler : MonoBehaviour
{
	public bool useTag;
	public string tagName;
	public UnityEvent triggerEnter;
	public UnityEvent triggerEnterDoOnce;
	public UnityEvent triggerExit;
	public UnityEvent triggerStay;
	public UnityEvent firstTriggerEnter;
	public UnityEvent lastTriggerExit;

	private bool doOnce = false;


	[HideInInspector]
	[System.NonSerialized]
	public int collisionCount;

	private void OnEnable()
	{
		collisionCount = 0;
	}

	private void OnDisable()
	{
		collisionCount = 0;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (useTag)
		{
			Transform node = other.transform;
			while (!node.gameObject.CompareTag(tagName))
			{
				if (node.parent == null) return;

				node = node.parent;
			}
		}

		collisionCount++;
		if (collisionCount == 1)
		{
			firstTriggerEnter.Invoke();
		}

		if (!doOnce)
		{
			triggerEnterDoOnce.Invoke();
			doOnce = true;
			//Debug.Log("Do once trigger on '" + gameObject.name + "' happend. Other: '" + other.name + "'", other);
		}

		triggerEnter.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		if (useTag)
		{
			Transform node = other.transform;
			while (!node.gameObject.CompareTag(tagName))
			{
				if (node.parent == null) return;

				node = node.parent;
			}
				
		}

		collisionCount--;
		if (collisionCount == 0)
			lastTriggerExit.Invoke();
		triggerExit.Invoke();
	}

	private void OnTriggerStay(Collider other)
	{
		if(!useTag || (useTag && other.gameObject.CompareTag(tagName))){
			triggerStay.Invoke();
		}
	}

	public void OnTriggerEnter()
	{
		collisionCount++;
		if (collisionCount == 1)
			firstTriggerEnter.Invoke();
		triggerEnter.Invoke();
	}

	public void OnTriggerExit()
	{
		collisionCount--;
		if (collisionCount < 0)
			collisionCount = 0;
		if (collisionCount == 0)
			lastTriggerExit.Invoke();
		triggerExit.Invoke();
	}

	public void OnTriggerStay()
	{
		triggerStay.Invoke();
	}
}
