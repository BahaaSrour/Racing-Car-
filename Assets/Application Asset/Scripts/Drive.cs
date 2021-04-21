using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
    public WheelCollider WC;
    public float torque = 200;


    void Start()
    {
        WC = GetComponent<WheelCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        float a = Input.GetAxis("Vertical");
        Debug.Log("a ="+ a);
        if(a!=0)
        Go(a);
    }

    private void Go(float accel)
    {
        accel = Mathf.Clamp(accel, -1,1);
        float thrustTorque = torque * accel;
        WC.motorTorque = thrustTorque;

        Quaternion quat;
        Vector3 pos;
        WC.GetWorldPose(out pos, out quat);
        transform.position = pos;
        transform.rotation = quat;

    }
}
