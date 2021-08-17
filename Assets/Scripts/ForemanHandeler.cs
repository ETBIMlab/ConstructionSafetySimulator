using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

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

    [Header("Locations")]
    public ForemanLocation[] locations;

    private bool m_inTransition;
    public bool InTransition
    {
        get => m_inTransition;
        set
        {
            m_inTransition = value;
        }
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
                    ForemanRig.weight = 0;
                }
            }
        }
    }

    private float blendWeight;
    private int locationIndex;
    private bool handOffHasHappend;
    

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

        Debug.Assert(index >= 0 && index < locations.Length, "Index on ForemanHandeler:SetLocationIndex() is out of bounds.", this);

        if (index != this.locationIndex)
        {
            this.locationIndex = index;

            InTransition = true;
            StopAllCoroutines();
            StartCoroutine(MoveToCoroutine());

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
    }

    public IEnumerator MoveToCoroutine()
    {
        // Bend animation to rotating
        ForemanController.CrossFadeInFixedTime(RotatingAnimation, FixedTransitionTime);

        while (true)
        {
            Vector3 targetDirection = locations[locationIndex].location.position - ForemanTrans.position;
            ForemanTrans.forward = Vector3.RotateTowards(ForemanTrans.forward, targetDirection, AngleSpeed * Time.deltaTime * 0.01f, 0.0f);

            if (Vector3.Angle(ForemanTrans.forward,
                targetDirection) < 0.001f)
                break;

            yield return null;
        }

        // blend Animation to walking
        ForemanController.CrossFadeInFixedTime(WalkingAnimation, FixedTransitionTime);


        while (Vector3.Distance(ForemanTrans.position, locations[locationIndex].location.position) > 0.001f)
        {
            ForemanTrans.position = Vector3.MoveTowards(
                ForemanTrans.position,
                locations[locationIndex].location.position,
                speed * Time.deltaTime
                );
            yield return null;
        }

        // Blend animation to rotating
        ForemanController.CrossFadeInFixedTime(RotatingAnimation, FixedTransitionTime);

        while (Quaternion.Angle(
            ForemanTrans.rotation,
            locations[locationIndex].location.rotation
            ) > 0.001f)
        {
            ForemanTrans.rotation = Quaternion.RotateTowards(
                ForemanTrans.rotation,
                locations[locationIndex].location.rotation,
                AngleSpeed * Time.deltaTime
                );
            yield return null;
        }

        // blend animation to idle
        ForemanController.CrossFadeInFixedTime(IdleAnimation, FixedTransitionTime);

        locations[locationIndex].location.gameObject.SetActive(true);
        InTransition = false;
    }

    public bool IsIPadHandOff()
    {
        Debug.LogFormat("!InTransition: {0}, locationIndex != -1 {1}, UpdateRig: {2}, !handOffHasHappend: {3}, HandOffArea.bounds.Intersects(IPad.bounds): {4}", !InTransition, locationIndex != -1,  UpdateRig, !handOffHasHappend, HandOffArea.bounds.Intersects(IPad.bounds));
        return !InTransition && locationIndex != -1 && UpdateRig && !handOffHasHappend && HandOffArea.bounds.Intersects(IPad.bounds);
    }

    public void IPadHandedOff()
    {
        handOffHasHappend = true;
        Debug.Log("Ipad has been handed off");
        StartCoroutine(IPadHandedOffDelayCoroutine());
    }

    private IEnumerator IPadHandedOffDelayCoroutine()
    {
        yield return new WaitForSeconds(locations[locationIndex].OnHandOffDelay);
        locations[locationIndex].OnHandoff.Invoke();
    }
}
