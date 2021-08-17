using Bhaptics.Tact.Unity;
using UnityEngine;

[RequireComponent(typeof(HapticSource))]
public class HapticsOnWalk : MonoBehaviour
{
    [SerializeField]
    private VR_Animator_Controller m_VR_Animator_Controller;
    [SerializeField]
    private AnimationCurve SpeedIntensityCurve;
    [SerializeField]
    private float stepSpeed;

    private HapticSource[] hapticSources;
    private int index;
    private float counter;

    // Start is called before the first frame update
    void Start()
    {
        if (m_VR_Animator_Controller == null)
        {
            Debug.LogError("Cannon play HapticsOnWalk without a VR Animation Constroller. Disabling component...", this);
            this.enabled = false;
            return;
        }

        hapticSources = GetComponents<HapticSource>();
        foreach (var item in hapticSources)
        {
            item.duration = 0.3f;
        }
        index = 0;
        counter = stepSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (counter < 0)
        {
            if (Time.timeScale == 0)
                return;

            float intensity = SpeedIntensityCurve.Evaluate(m_VR_Animator_Controller.Speed);
            if (intensity > 0)
            {
                var source = hapticSources[index];

                source.intensity = intensity;
                source.Play();

                counter = stepSpeed;
                index++;
                if (index >= hapticSources.Length)
                    index = 0;
            }
        }
        else counter -= Time.deltaTime;
    }
}
