using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerHeightManager : MonoBehaviour
{
    public PlayerHeightManager instance;

    private const float robotKyleHeight = 1.6f;
    private const float normalArmScale = 1.0f;

    [SerializeField]
    [Tooltip("This is the players height in meters.")]
    private float playerHeight;
    [SerializeField]
    [Tooltip("This is the players proportional arm length compared to bone structure.")]
    private float playerArmScale;
    [SerializeField]
    [Tooltip("The bone structure of the player. This is scaled up to match the players height.")]
    private Transform avatarRoot;
    [SerializeField]
    [Tooltip("The left arm bone structure of the player. This is scaled up to match the players arm length.")]
    private Transform leftArmRoot;
    [SerializeField]
    [Tooltip("The left wrist of the player. This is used to align the player hands.")]
    private Transform leftArmWrist;
    [SerializeField]
    [Tooltip("The left wrist's target. This is used to align the player hands.")]
    private Transform leftArmTarget;
    [SerializeField]
    [Tooltip("The right arm bone structure of the player. This is scaled up to match the players arm length.")]
    private Transform rightArmRoot;
    [SerializeField]
    [Tooltip("The right wrist of the player. This is used to align the player hands.")]
    private Transform rightArmWrist;
    [SerializeField]
    [Tooltip("The right wrist's target. This is used to align the player hands.")]
    private Transform rightArmTarget;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Player.instance.headsetOnHead.active);
        AlignHands();
    }

    private void OnEnable()
    {
        if (instance != null)
        {
            Debug.LogError("There should only be one PlayerHeightManager active in the scene!");
            this.enabled = false;
        }
        else instance = this;
        
    }

    private void OnDisable()
    {
        if (instance == this)
            instance = null;
    }

    private void AlignHands()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        if (rightArmWrist == null || leftArmWrist == null || leftArmTarget == null || rightArmTarget == null)
        {
            Debug.LogWarning("Both wrists and targets should be set in the PlayerHeightManager when adjusting player size. Expect the hands to not align.", this);
            return;
        }

        Debug.Log("Hand Offset: " + (leftArmWrist.position - leftArmTarget.position));
    }

    public float GetPlayerHeight()
    {
        return playerHeight;
    }

    public void SetPlayerHeight()
    {
        avatarRoot.localScale = (Vector3.one * playerHeight) / robotKyleHeight;
        AlignHands();
    }

    public void SetPlayerHeight(float playerHeight)
    {
        this.playerHeight = playerHeight;

        SetPlayerHeight();
    }

    public float GetPlayerArmLength()
    {
        return playerArmScale;
    }

    public void SetPlayerArmLength()
    {
        leftArmRoot.localScale = (Vector3.one * playerArmScale) / normalArmScale;
        rightArmRoot.localScale = (Vector3.one * playerArmScale) / normalArmScale;
        AlignHands();
    }

    public void SetPlayerArmLength(float armLength)
    {
        this.playerArmScale = armLength;

        SetPlayerHeight();
    }
}
