using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float torque = 200;
    public float steerAngle = 30;
    public WheelCollider[] WCs;
    public GameObject[] Wheels;

 
    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        Go(a,steer);
        AnimiateTyres();
    }

    private void Go(float accel,float steer)
    {
        accel = Mathf.Clamp(accel, -1,1);
        steer = Mathf.Clamp(steer, -1,1)*steerAngle;
        float thrustTorque = torque * accel;
        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            WCs[i].steerAngle = steer;
        }
    }
    public void AnimiateTyres()
    {
        for (int i = 0; i < 4; i++)
        {

        Quaternion quat;
        Vector3 pos;
        WCs[i].GetWorldPose(out pos, out quat);
        Wheels[i].transform.position = pos;
            //if(i<2 )
        Wheels[i].transform.rotation = quat;
        }
    }
}
