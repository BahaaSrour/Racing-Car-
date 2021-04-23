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
    public float breakTorque = 2000;
 
    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");
        Go(a,steer,brake);
        AnimiateTyres();
    }

    private void Go(float accel,float steer,float brake)
    {
        accel = Mathf.Clamp(accel, -1,1);
        brake = Mathf.Clamp(brake, 0,1)* breakTorque;
        steer = Mathf.Clamp(steer, -1,1)*steerAngle;
        float thrustTorque = torque * accel;
        for (int i = 0; i < 4; i++)
        {
            WCs[i].motorTorque = thrustTorque;
            if (i < 2)
                WCs[i].steerAngle = steer;
            else
                WCs[i].brakeTorque = brake;
            Quaternion quat;
            Vector3 pos;
            WCs[i].GetWorldPose(out pos, out quat);
            Wheels[i].transform.position = pos;
            Wheels[i].transform.localRotation = quat;
        }
    }

    public void AnimiateTyres()
    {
        //for (int i = 0; i < 4; i++)
        //{

        ////Quaternion quat;
        ////Vector3 pos;
        ////WCs[i].GetWorldPose(out pos, out quat);
        ////Wheels[i].transform.position = pos;
        ////    if (i < 2)
        ////        Wheels[i].transform.rotation = quat;
        //}
    }
}
