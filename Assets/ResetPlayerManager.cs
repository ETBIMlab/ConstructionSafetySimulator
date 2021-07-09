using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ResetPlayerManager : MonoBehaviour
{
    public void EnablePlayerHands(bool enabled)
    {
        if (Player.instance == null)
            Debug.LogWarning("SteamVR player is not set up. Cannot access hands.", this);
        else
            foreach(Hand hand in Player.instance.hands)
                hand.SetVisibility(enabled);
    }

}
