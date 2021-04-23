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

    public Transform skidTrailPrefab;
    Transform[] skidTrails = new Transform[4];



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
                {
                    skidSound.Play();
                    StartSkidTrail(i);
                }
            }
            else 
            {
                EndSkidTrail(i);
            }
        }
        if (skidingNum == 0 && skidSound.isPlaying)
            skidSound.Stop();
    }

    public void StartSkidTrail(int i)
    {
        if (skidTrails[i] == null)
            skidTrails[i] = Instantiate(skidTrailPrefab);
        skidTrails[i].parent = WCs[i].transform;
        //to start the skidd from the buttom of the tyre
        skidTrails[i].localPosition = -Vector3.up * WCs[i].radius;
    }

    public void EndSkidTrail(int i)
    {
        if (skidTrails[i] == null) return;
        //we created holder to hold the trail object then frees our skilltrail
        Transform holder = skidTrails[i];
        skidTrails[i] = null;
        //it now has no parent
        holder.parent = null;
        // we destroyed that trail after 3o sec to let the player watch and it disapears
        Destroy(holder.gameObject, 30f);
    }
}
