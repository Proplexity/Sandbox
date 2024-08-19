using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class location : MonoBehaviour
{

    public GameObject cam;
    public GameObject LaserPointer;

    private void Update()
    {
        RaycastHit laser;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out laser))
        {
            LaserPointer.transform.LookAt(laser.point);
        }
    }


}
