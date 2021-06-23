using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    /// <summary>
    /// This class is used for creating haptic feedback in response to collision events between colliders.
    /// The haptic clips are taken from the HapticSender class that should be attached to the other collider. The
    /// HapticSender class has different playable clips depending on a hit PositionTag (view HapticSender.Play()) (collision
    /// only plays haptics if the trigger/collision collider has a HapticSender). This class is intended to play haptics
    /// automatically using the dynamic relationship between this and HapticSender.
    /// </summary>
    public class HapticReceiver : MonoBehaviour
    {
        public bool IsActive = true;
        public PositionTag PositionTag = PositionTag.Body;

        void Awake()
        {
            var col = GetComponent<Collider>();

            if (col == null)
            {
                BhapticsLogger.LogInfo("collider is not detected");
            }
        }

        void OnTriggerEnter(Collider bullet)
        {
            if (IsActive)
            {
                Handle(bullet.transform.position, bullet.GetComponent<HapticSender>());
            }
            
        }

        void OnCollisionEnter(Collision bullet)
        {
            if (IsActive)
            {
                Handle(bullet.contacts[0].point, bullet.gameObject.GetComponent<HapticSender>());
            }
        }

        private void Handle(Vector3 contactPoint, HapticSender tactSender)
        {
            if (tactSender != null)
            {
                Debug.Log("HIT AT: " + PositionTag);
                var targetCollider = GetComponent<Collider>();
                tactSender.Play(PositionTag, contactPoint, targetCollider);
            }
        }
    }
}
