using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public float torque = 200;
    public float steerAngle = 30;
    public WheelCollider WC;
    public GameObject mesh;

    void Start()
    {
        WC = GetComponent<WheelCollider>();

    }

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
      
        WC.motorTorque = thrustTorque;
        WC.steerAngle = steer;
    }
    public void AnimiateTyres()
    {
        Quaternion quat;
        Vector3 pos;
        WC.GetWorldPose(out pos, out quat);
        mesh.transform.position = pos;
        mesh.transform.rotation = quat;
    }
}
