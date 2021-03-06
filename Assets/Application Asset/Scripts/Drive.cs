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
    public AudioSource highAccel;

    public Transform skidTrailPrefab;
    Transform[] skidTrails = new Transform[4];
    public ParticleSystem smokePrefab;
    ParticleSystem[] skidsmoke = new ParticleSystem[4];

    public Rigidbody rb;
    public float gearLength;
    public float currentSpeed{get{ return rb.velocity.magnitude * gearLength; }}
    public float lowPitch = 1;
    public float highPitch = 6;
    public float numGear = 5;
    float rbm;
    int currentGear = 1;
    float  currentGearPerc ;
    public float maxSpeed = 200;


    public void CalculateGearSound()
    {
        float gearPercentage = (1 / numGear);

        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1), Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        //calculate rbm per minute
        var gearNumFactor = currentGear / (float)numGear;
        rbm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);


        //we will determine if we are going to change gear
        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        
        //next gear starting value
        float upperGearMax = (1/(float)numGear)*(currentGear+1); 
        float downGearMax = (1/(float)numGear)*currentGear;

        if (currentGear > 0 && currentGear < downGearMax)
            currentGear--;
        if (currentGear > upperGearMax && currentGear < (numGear - 1))
            currentGear++;

        float pitch = Mathf.Lerp(lowPitch, highPitch, rbm);
        highAccel.pitch = Math.Min(highPitch, pitch) * .25f;
    }


    public GameObject BrakeLight;
    private void Start()
    {
        BrakeLight.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            skidsmoke[i] = Instantiate(smokePrefab);
            skidsmoke[i].Stop();
        }
    }

    void Update()
    {
        float a = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");
        float brake = Input.GetAxis("Jump");
        Go(a,steer,brake);
        CheckForSkid();
        CalculateGearSound();
    }

    private void Go(float accel,float steer,float brake)
    {
        accel = Mathf.Clamp(accel, -1,1);
        brake = Mathf.Clamp(brake, 0,1)* breakTorque;
        steer = Mathf.Clamp(steer, -1,1)*steerAngle;
        float thrustTorque = torque * accel;
        if (brake > 0)
            BrakeLight.SetActive(true);
        else
            BrakeLight.SetActive(false);
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
            if (Mathf.Abs(wheelHit.forwardSlip) >= .4f || Mathf.Abs(wheelHit.sidewaysSlip) >= .4f)
            {
                skidingNum++;
                if (!skidSound.isPlaying)
                {
                    skidSound.Play();
                }
                StartSkidTrail(i);
                skidsmoke[i].transform.position = WCs[i].transform.position - Vector3.up * WCs[i].radius;
                skidsmoke[i].Emit(1);
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

        //to make the skid trail always looks up 
        skidTrails[i].localRotation = Quaternion.Euler(90, 0, 0);
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
        //because it will get a new roattion accourading to the holder as a new parent
        holder.rotation= Quaternion.Euler(90, 0, 0);
        // we destroyed that trail after 3o sec to let the player watch and it disapears
        Destroy(holder.gameObject, 30f);
    }
}
