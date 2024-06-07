using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputBehavior : MonoBehaviour
{
    public SmartphoneRemote smartphoneRemote;

    public Vector3 accel = new Vector3();
    public Vector3 gyro = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        accel = smartphoneRemote.accel;
        gyro = smartphoneRemote.gyro;
    }

    // Update is called once per frame
    void Update()
    {
        accel = smartphoneRemote.accel;
        gyro = smartphoneRemote.gyro;
    }

    
    public Vector3 GetAccel() {
        return smartphoneRemote.accel;
    }

    public Vector3 GetGyro() {
        return smartphoneRemote.gyro;
    }
}
