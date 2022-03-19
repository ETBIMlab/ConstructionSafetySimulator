using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    private Animation anim_bars;
    public List<GameObject> disableObjects;

    private void Awake()
    {
        anim_bars = GetComponent<Animation>();
    }

    public void PlayAnimationBars()
    {
        anim_bars.Play();
        foreach(GameObject obj in disableObjects)
        {
            obj.SetActive(false);
        }
    }

    public void StopAnimationBars()
    {
        //anim_bars[anim_bars.clip.name].normalizedTime = 1f;
        //anim_bars[anim_bars.clip.name].speed = -1;
        anim_bars.Rewind();
        anim_bars.Play();
        anim_bars.Sample();
        anim_bars.Stop();
        foreach (GameObject obj in disableObjects)
        {
            obj.SetActive(true);
        }
    }
}
