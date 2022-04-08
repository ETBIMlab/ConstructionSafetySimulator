/*
 * Controls when the shards are animated.
 * 
 * Author: Cristion Dominguez
 * Date: 3 Feb. 2022
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardAnimController : MonoBehaviour
{
    [SerializeField, Tooltip("Delay between animating the next shard")]
    private float delayBetweenShardAnimations;

    [SerializeField, Tooltip("Animators of shards")]
    private Animator[] shardAnimators;

    private WaitForSeconds waitForShardAnim;

    /// <summary>
    /// Sets the suspension for the coroutine that animates the shards and starts the coroutine.
    /// </summary>
    private void Start()
    {
        waitForShardAnim = new WaitForSeconds(delayBetweenShardAnimations);
        StartCoroutine(DelayShardAnimations());
    }

    /// <summary>
    /// Commences the animation for each shard with delays in-between.
    /// </summary>
    private IEnumerator DelayShardAnimations()
    {
        foreach (Animator animator in shardAnimators)
        {
            animator.SetTrigger("Wiggle");
            yield return waitForShardAnim;
        }
    }
}
