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

    public AudioSource skidSound;
    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");
        Go(a,steer,brake);
        CheckForSkid();
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

    void CheckForSkid()
    {
        int skidingNum = 0;

        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelHit;
            WCs[i].GetGroundHit(out wheelHit);
            if (Mathf.Abs(wheelHit.forwardSlip) >= .5f || Mathf.Abs(wheelHit.sidewaysSlip) >= .5f)
            {
                skidingNum++;
                if (!skidSound.isPlaying)
                    skidSound.Play();
            }
        }
        if (skidingNum == 0 && skidSound.isPlaying)
            skidSound.Stop();
    }
}
