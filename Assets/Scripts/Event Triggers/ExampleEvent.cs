using UnityEngine;
// (1) Include Library
using UnityEngine.Events;

public class ExampleEvent : MonoBehaviour
{
    // (2) Declare a public unity event
    public UnityEvent OnSpaceBar;

    // Update is called once per frame
    private void Update()
    {
        // Returns true on the frame spacebar is hit
        if (Input.GetKeyDown(KeyCode.Space))
            // (3) Invoke Listeners
            OnSpaceBar.Invoke();
    }
}



