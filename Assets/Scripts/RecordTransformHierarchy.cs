using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

/// <summary>
/// Copied from the Unity Website under "GameObjectRecorder".
/// Used to bake simulations to animations
/// </summary>

public class RecordTransformHierarchy : MonoBehaviour
{
#if UNITY_EDITOR
    public AnimationClip clip;
    public Vector3 triggerVelocity = Vector3.zero;

    private GameObjectRecorder m_Recorder;
    
    public void TriggerVelocity()
    {
        Rigidbody _rb;
        if((_rb = GetComponent<Rigidbody>()) != null)
            _rb.AddForce(triggerVelocity, ForceMode.VelocityChange);
    }

    void Start()
    {
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        // Take a snapshot and record all the bindings values for this frame.
        m_Recorder.TakeSnapshot(Time.deltaTime);
    }

    void OnDisable()
    {
        if (clip == null)
            return;

        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the clip.
            m_Recorder.SaveToClip(clip);
        }
    }

#endif
}