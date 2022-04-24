using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.AI;

public class ForemanHandeler : MonoBehaviour
{
	public const float FixedTransitionTime = 1f;
	public static List<ForemanHandeler> instances = new List<ForemanHandeler>();

	[System.Serializable]
	public class ForemanLocation
	{
		public Transform location;
		[Tooltip("Used if you want the unity event to be delayed")]
		public float OnHandOffDelay = 0;
		public UnityEvent OnHandoff;
	}

	[Header("Movement Values")]
	public Animator ForemanController;
	public Transform ForemanTrans;
	public float speed = 1.0f;
	public float AngleSpeed = 130.0f;

	public GameObject ForemanBody;

	[Header("HandOff Values")]
	public Collider IPad;
	public Transform ForemanRightHand;
	[Tooltip("The collider that determines if the IPad is in range of the foreman when handing off the ipad")]
	public Collider HandOffArea;

	[Header("Rig Values")]
	public Transform RATarget;
	public AnimationCurve DistanceWeightCurve = new AnimationCurve();
	public Rig ForemanRig;
	public float targetSpeed;

	[Header("Animation States")]
	public string IdleAnimation;
	public string RotatingAnimation;
	public string WalkingAnimation;

	[Header("Default Actions")]
	public UnityEvent OnHandoff;
	public UnityEvent OnReset;


	[Header("Locations")]
	public ForemanLocation[] locations;

	private bool m_inTransition;
	public bool InTransition
	{
		get => m_inTransition;
		set
		{
			m_inTransition = value;

			if (!value)
			{
				ActivateLocation();
				navAgent.updateRotation = false;
				navAgent.updatePosition = false;
				ForemanController.SetFloat("velocity", 0);
			}
			else
			{
				if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
				navAgent.updatePosition = true;
				navAgent.updateRotation = true;
			}
		}
	}

	private void ActivateLocation()
	{
		if (locationIndex != -1)
		{
			Debug.Log("Activating location " + locationIndex);
			locations[locationIndex].location.gameObject.SetActive(true);

			if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
			rotationCoroutine = StartCoroutine(RotateToLocation());
		}
	}

	private Coroutine rotationCoroutine;
	private IEnumerator RotateToLocation()
	{
		yield return null;

		while (Quaternion.Angle(ForemanBody.transform.rotation, locations[locationIndex].location.rotation) > 3.0f)
		{
			ForemanBody.transform.rotation = Quaternion.RotateTowards(ForemanBody.transform.rotation, locations[locationIndex].location.rotation, navAgent.angularSpeed * Time.deltaTime);
			yield return null;
		}

		rotationCoroutine = null;
	}

	public bool UseRig { get => ForemanRig != null; }

	private bool m_updateRig;
	public bool UpdateRig
	{
		get => m_updateRig;
		set
		{
			if (m_updateRig != value)
			{
				if (value)
				{
					m_updateRig = true;
					blendWeight = 0;
				}
				else
				{
					m_updateRig = false;
					if (ForemanRig)
						ForemanRig.weight = 0;
				}
			}
		}
	}

	private float blendWeight;
	private int locationIndex;
	private bool handOffHasHappend;
	private NavMeshAgent navAgent;
	

	private void OnEnable()
	{
		instances.Add(this);
	}

	private void OnDisable()
	{
		instances.Remove(this);
	}

	public void SetUpdateRig(bool enabled)
	{
		UpdateRig = enabled;
	}


	private void Awake()
	{
		navAgent = ForemanBody.GetComponent<NavMeshAgent>();
	}

	// Start is called before the first frame update
	void Start()
	{
		Debug.Assert(ForemanController, "ForemanController MUST be set inorder to use the ForemanHandeler.", this);
		if (IPad == null || ForemanRightHand == null || HandOffArea == null)
			Debug.LogWarning("IPad, RightHand, and HandOffArea should be set inorder to use the handoff in ForemanHandeler.", this);

		UpdateRig = false;
		handOffHasHappend = false;
		blendWeight = 0;

		if (UseRig)
			ForemanRig.weight = 0;

		locationIndex = -1;
		InTransition = false;

		foreach (ForemanLocation fl in locations)
		{
			fl.location.gameObject.SetActive(false);
		}
	}

	public void SetLocationIndex(int index)
	{
		if (handOffHasHappend)
			return;

		Debug.Assert(index >= -1 && index < locations.Length, "Index on ForemanHandeler:SetLocationIndex() is out of bounds.", this);

		if (index != this.locationIndex)
		{
			this.locationIndex = index;

			if (locationIndex == -1)
			{
				InTransition = false;
				navAgent.destination = transform.position;
			}
			else
			{
				InTransition = true;
				navAgent.destination = locations[locationIndex].location.position;
			}

			foreach (ForemanLocation fl in locations)
			{
				fl.location.gameObject.SetActive(false);
			}
		   
		}
	}

	private void Update()
	{
		if (!InTransition && UpdateRig && !handOffHasHappend)
		{
			blendWeight = Mathf.Clamp01(blendWeight + Time.deltaTime * targetSpeed);
		}
		else
			blendWeight = Mathf.Clamp01(blendWeight - Time.deltaTime * targetSpeed);

		if (UseRig)
		{
			// Update the position of the hand
			if (RATarget != null)
				RATarget.position = IPad.ClosestPointOnBounds(ForemanRightHand.position);
			// Update the rotation of the hand
			//if (ForemanRightHand.position - RATarget.position != Vector3.zero)
			//    RATarget.forward = ForemanRightHand.position - RATarget.position;
			ForemanRig.weight = blendWeight;
		}

		// mine
		if(InTransition && navAgent.velocity.magnitude < 0.05f && Vector3.Distance(navAgent.destination, ForemanBody.transform.position) <= navAgent.stoppingDistance)
		{
			InTransition = false;
			Debug.Log("Reached Goal!");
		}
		else
		{
			ForemanController.SetFloat("velocity", navAgent.velocity.magnitude);
		}
		// end mine
	}

	public bool IsIPadHandOff()
	{
		Debug.LogFormat("!InTransition: {0}, locationIndex != -1 {1}, UpdateRig: {2}, !handOffHasHappend: {3}, HandOffArea.bounds.Intersects(IPad.bounds): {4}", !InTransition, locationIndex != -1,  UpdateRig, !handOffHasHappend, HandOffArea.bounds.Intersects(IPad.bounds));
		return !InTransition && locationIndex != -1 && UpdateRig && !handOffHasHappend && HandOffArea.bounds.Intersects(IPad.bounds);
	}

	public void IPadHandedOff()
	{
		if (locationIndex < 0)
		{
			Debug.LogWarning("IPad is being handed off at an invalid location");
			return;
		
		}
		handOffHasHappend = true;
		Debug.Log("Ipad has been handed off");
		StartCoroutine(IPadHandedOffDelayCoroutine());
	}

	private IEnumerator IPadHandedOffDelayCoroutine()
	{
		OnHandoff.Invoke();
		yield return new WaitForSeconds(locations[locationIndex].OnHandOffDelay);
		locations[locationIndex].OnHandoff.Invoke();
	}
}
