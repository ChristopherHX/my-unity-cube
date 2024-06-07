using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private TcpListener listener = null;
    private List<Socket> sockets = new List<Socket>();

    // Start is called before the first frame update
    async void Start()
    {
        listener = new TcpListener(IPAddress.Any, 8909);
        listener.Start();
        while(true) {
            var socket = await listener.AcceptSocketAsync();
            sockets.Add(socket);
        }
    }

    ~NewBehaviourScript() {
        listener.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(sockets.Any()) {
            Vector3 vector3 = new Vector3();
            Vector3 vector3rot = new Vector3();
            foreach(var s in sockets) {
                byte[] bytes = new byte[4 * 6];
                int received = s.Receive(bytes);
                float[] xyz = new float[6];
                for(int i = 0; i < 6; i++) {
                    Array.Reverse(bytes, i * 4, 4);
                    xyz[i] = BitConverter.ToSingle(bytes, i * 4);
                }
                vector3.Set(xyz[0], /*xyz[1] - 9.81f*/ 0, xyz[1]);
                vector3rot.Set(xyz[3], xyz[4], xyz[5]);
            }
            // Move the object forward along its z axis 1 unit/second.
            //transform.Translate(Vector3.forward * Time.deltaTime);
            transform.Translate(vector3 * Time.deltaTime);
            transform.Rotate(vector3rot);
            // Move the object upward in world space 1 unit/second.
            //transform.Translate(Vector3.up * Time.deltaTime, Space.World);
        }
    }
}
