using UnityEngine;

namespace CrazyMinnow.SALSA
{
    /// <summary>
    /// Simple script to update the microphone record head position in SALSA for automatic position adjustments.
    /// NOTE: This script was added for SALSA v2.3.0+ due to removing the Microphone class from the core
    ///     SALSA code. This version of micInput requires SALSA LipSync Suite v2.3.0+.
    /// </summary>
    public class SalsaMicPointerSync : MonoBehaviour
    {
        private Salsa salsa;
        private SalsaMicInput micInput;

        private void Start()
        {
            salsa = gameObject.GetComponent<Salsa>();
            micInput = gameObject.GetComponent<SalsaMicInput>();
        }

        private void Update()
        {
            if (salsa != null && micInput != null)
            {
                salsa.microphoneRecordHeadPointer = Microphone.GetPosition(micInput.selectedMic);
            }
        }
    }
}