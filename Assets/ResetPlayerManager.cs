using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ResetPlayerManager : MonoBehaviour
{
    [Tooltip("If track calls is enabled, there will be counters for how many times an object is enabled and disabled, and the state will only change if one number of calls out numbers the other.")]
    public bool TrackCalls = false;

    public float FadeTime = 0;

    private int enabledHandCalls = 0;
    private int enabledBodyCalls = 0;
    private bool fading = false;

    public void EnablePlayerHands(bool enabled)
    {
        if (Player.instance == null)
            Debug.LogWarning("SteamVR player is not set up. Cannot access hands.", this);

        else if (TrackCalls)
        {
            enabledHandCalls += (enabled) ? 1 : -1;

            if (enabledHandCalls < 0)
                enabled = false;
            else enabled = true;

        }

        foreach (Hand hand in Player.instance.hands)
        {
            if (enabled != hand.gameObject.activeSelf)
            {
                if (fading)
                {
                    fading = false;
                    StopAllCoroutines();
                }

                if (enabled)
                    StartCoroutine(StartFadeIn(hand.gameObject));
                else
                    StartCoroutine(StartFadeOut(hand.gameObject));
            }
        }
    }

    public void EnablePlayerBody(bool enabled)
    {
        if (Player.instance == null)
            Debug.LogWarning("SteamVR player is not set up. Cannot access Body Visuals.", this);

        else if (Player.instance.bodyVisuals == null)
            Debug.LogWarning("SteamVR player does not have a reference to a player body. Cannot access Body Visuals.", this);

        else if (TrackCalls)
        {
            enabledBodyCalls += (enabled) ? 1 : -1;

            if (enabledBodyCalls < 0)
                enabled = false;
            else enabled = true;
        }

        foreach (GameObject go in Player.instance.bodyVisuals)
        {
            if (enabled != go.activeSelf)
            {
                if (fading)
                {
                    fading = false;
                    StopAllCoroutines();
                }

                if (enabled)
                    StartCoroutine(StartFadeIn(go));
                else
                    StartCoroutine(StartFadeOut(go));
            }
        }
    }

    private IEnumerator StartFadeOut(GameObject go)
    {
        yield return new WaitForEndOfFrame();
        fading = true;
        float counter = 0;
        Vector3 initScale = go.transform.localScale;

        while (counter < FadeTime)
        {
            counter += Time.unscaledDeltaTime;

            if (Player.instance == null)
            {
                Debug.LogWarning("SteamVR player is not set up. Cannot access Body Visuals.", this);
                break;
            } else if (Player.instance.bodyVisuals == null)
            {
                Debug.LogWarning("SteamVR player does not have a reference to a player body. Cannot access Body Visuals.", this);
                break;
            }

            go.transform.localScale = initScale * (1 - (counter/FadeTime));
            Debug.Log("Loop new scale: " + go.transform.localScale);
            yield return null;
        }

        go.transform.localScale = Vector3.zero;
        go.SetActive(false);
    }

    private IEnumerator StartFadeIn(GameObject go)
    {
        yield return new WaitForEndOfFrame();

        fading = true;
        float counter = 0;
        Vector3 initScale = go.transform.localScale;
        go.SetActive(true);

        while (counter < FadeTime)
        {
            counter += Time.unscaledDeltaTime;

            if (Player.instance == null)
            {
                Debug.LogWarning("SteamVR player is not set up. Cannot access Body Visuals.", this);
                break;
            }
            else if (Player.instance.bodyVisuals == null)
            {
                Debug.LogWarning("SteamVR player does not have a reference to a player body. Cannot access Body Visuals.", this);
                break;
            }

            go.transform.localScale = initScale + (Vector3.one - initScale) * ((counter / FadeTime));
            yield return null;
        }

        go.transform.localScale = Vector3.one;
        
    }

    public void EnablePlayerBodyAndHands(bool enabled)
    {
        EnablePlayerBody(enabled);
        EnablePlayerHands(enabled);
    }

}
