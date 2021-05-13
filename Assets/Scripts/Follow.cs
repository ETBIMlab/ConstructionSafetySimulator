using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Follow : MonoBehaviour
{
    private GameObject wayPoint;
    private Vector3 distanceVect;
    private Vector3 wayPointPos;
    private Vector3 targetPos;
    private GameObject boxHead;
    private GameObject player;
    private float distance = 5.0f;
    private bool met = false; 

    private float speed = 6.0f;
    void Start()
    {
        //At the start of the game, boxhead will find the gameobject called wayPoint.
        wayPoint = GameObject.Find("wayPoint");
        boxHead = GameObject.Find("boxHead.v2");
        player = GameObject.Find("Player");
    }

    void Update()
    {
        LookAt();
        wayPointPos = wayPoint.transform.position;

        distanceVect = new Vector3((wayPoint.transform.position.x - boxHead.transform.position.x),
                                   (wayPoint.transform.position.y - boxHead.transform.position.y),
                                   (wayPoint.transform.position.z - boxHead.transform.position.z));

        double d = Math.Sqrt((wayPoint.transform.position.x - boxHead.transform.position.x) * (wayPoint.transform.position.x - boxHead.transform.position.x)
                           + (wayPoint.transform.position.y - boxHead.transform.position.y) * (wayPoint.transform.position.y - boxHead.transform.position.y)
                           + (wayPoint.transform.position.z - boxHead.transform.position.z) * (wayPoint.transform.position.z - boxHead.transform.position.z));

        Vector3 u = new Vector3(distanceVect.x / (float)d, distanceVect.y / (float)d, distanceVect.z / (float)d);

        //targetPos = new Vector3(wayPointPos.x + u.x * distance, wayPointPos.y + u.y * distance, wayPointPos.z + u.z * distance);
        targetPos = new Vector3(((float)(1 - distance / d)*wayPointPos.x + (float)(distance / d)*boxHead.transform.position.x),
                                ((float)(1 - distance / d) * wayPointPos.y + (float)(distance / d) * boxHead.transform.position.y),
                                ((float)(1 - distance / d) * wayPointPos.z + (float)(distance / d) * boxHead.transform.position.z));

        //Here, boxhead will follow the waypoint after meeting with the player and after 5.25 meters
        if(d >= 5.25 && !met)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if(d > 10 && met)
        {
            met = false;
        }
        else if(d > 5 && d < 5.25)
        {
            met = true;
        }
    }

    //Rotate towards player
    void LookAt()
    {
        if(player != null)
        {
            Quaternion lookAtRotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 5 * Time.deltaTime);
        }
    }
}