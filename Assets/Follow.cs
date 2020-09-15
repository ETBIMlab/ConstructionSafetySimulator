using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Follow : MonoBehaviour
{
    private GameObject wayPoint;
    private Vector3 wayPointPos;
    private GameObject boxHead;
    private float distance = 5.0f;

    private float speed = 6.0f;
    void Start()
    {
        //At the start of the game, boxhead will find the gameobject called wayPoint.
        wayPoint = GameObject.Find("wayPoint");
        boxHead = GameObject.Find("boxHead.v2");
    }

    void Update()
    {
        //wayPointPos = new Vector3(wayPoint.transform.position.x, transform.position.y, wayPoint.transform.position.z);

        //wayPointPos = new Vector3(((wayPoint.transform.position.x - boxHead.transform.position.x) * (wayPoint.transform.position.x - boxHead.transform.position.x)),
        //                         ((wayPoint.transform.position.y - boxHead.transform.position.y) * (wayPoint.transform.position.y - boxHead.transform.position.y)),
        //                         ((wayPoint.transform.position.z - boxHead.transform.position.z) * (wayPoint.transform.position.z - boxHead.transform.position.z)));
        wayPointPos = new Vector3((wayPoint.transform.position.x - boxHead.transform.position.x),
                                       (wayPoint.transform.position.y - boxHead.transform.position.y),
                                       (wayPoint.transform.position.z - boxHead.transform.position.z));

        double d = Math.Sqrt((wayPoint.transform.position.x - boxHead.transform.position.x) * (wayPoint.transform.position.x - boxHead.transform.position.x)
                            + (wayPoint.transform.position.y - boxHead.transform.position.y) * (wayPoint.transform.position.y - boxHead.transform.position.y)
                            + (wayPoint.transform.position.z - boxHead.transform.position.z) * (wayPoint.transform.position.z - boxHead.transform.position.z));

        Vector3 u = new Vector3(wayPointPos.x / (float)d, wayPointPos.y / (float)d, wayPointPos.z / (float)d);

        wayPointPos = new Vector3(wayPointPos.x - u.x * distance, wayPointPos.y - u.y * distance, wayPointPos.z - u.z * distance);

        //Here, boxhead will follow the waypoint.
        transform.position = Vector3.MoveTowards(transform.position, wayPointPos, speed * Time.deltaTime);
    }
}