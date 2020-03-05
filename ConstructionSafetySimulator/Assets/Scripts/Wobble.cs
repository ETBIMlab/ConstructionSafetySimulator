using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour {

    public int frequency = 30;
    public float wobbleAmt = 0.2f;
    private int count = 0;
    private bool goingOut = true;

    // Update is called once per frame
    void Update() {
        if (goingOut) {
            transform.Rotate(wobbleAmt, 0, 0, Space.World);
            count++;
            if (count > frequency)
                goingOut = false;
        } else {
            transform.Rotate(-wobbleAmt, 0, 0, Space.World);
            count--;
            if (count < 0)
                goingOut = true;
        }
    }
}
