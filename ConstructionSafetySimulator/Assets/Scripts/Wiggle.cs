using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour {

    public int frequency = 30;
    public float wiggleAmt = 0.2f;
    private int count = 0;
    private bool goingUp = true;
	
	// Update is called once per frame
	void Update () {
		if(goingUp) {
            transform.Translate(0, wiggleAmt, 0);
            count++;
            if (count > frequency)
                goingUp = false;
        } else {
            transform.Translate(0, -wiggleAmt, 0);
            count--;
            if (count < 0)
                goingUp = true;
        }
	}
}
