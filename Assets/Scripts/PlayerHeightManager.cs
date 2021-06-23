using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeightManager : MonoBehaviour
{
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
    [Tooltip("The right arm bone structure of the player. This is scaled up to match the players arm length.")]
    private Transform rightArmRoot;

    public float GetPlayerHeight()
    {
        return playerHeight;
    }

    public void SetPlayerHeight()
    {
        avatarRoot.localScale = (Vector3.one * playerHeight) / robotKyleHeight;
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
    }

    public void SetPlayerArmLength(float armLength)
    {
        this.playerArmScale = armLength;

        SetPlayerHeight();
    }
}
