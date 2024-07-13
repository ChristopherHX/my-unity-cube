using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Smartphone", order = 1)]
public class SmartphoneRemote : ScriptableObject
{
    private TcpListener listener = null;
    public Vector3 accel = new Vector3();
    public Vector3 gyro = new Vector3();
    public Vector3 position = new Vector3();
    public int Port = 0;

    private CancellationTokenSource cancellationToken = null;

    // Start is called before the first frame update
    public void OnEnable()
    {
// #if UNITY_EDITOR
//         EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
//         EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
//         return;
// #endif
        StartListener();
    }

    private async void StartListener() {
        if(listener != null) {
            return;
        }
        cancellationToken = new CancellationTokenSource();
        listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        Port = ((IPEndPoint)listener.LocalEndpoint).Port;
        await Task.Run(async () => {
            while(!cancellationToken.IsCancellationRequested) {
                var socket = await listener.AcceptSocketAsync();
                ProcessInputs(socket);
            }
        });
    }

    private async void ProcessInputs(Socket s) {
        while(!cancellationToken.IsCancellationRequested) {
            byte[] bytes = new byte[4 * 9];
            int r = 0;
            while(r < bytes.Length) {
                int received = s.Receive(bytes, r, bytes.Length - r, SocketFlags.None);
                if(received <= 0) {
                    break;
                }
                r += received;
            }
            float[] xyz = new float[9];
            for(int i = 0; i < 9; i++) {
                Array.Reverse(bytes, i * 4, 4);
                xyz[i] = BitConverter.ToSingle(bytes, i * 4);
            }
            accel.Set(xyz[0], xyz[1], xyz[2]);
            gyro.Set(xyz[3], xyz[4], xyz[5]);
            position.Set(xyz[6], xyz[7], xyz[8]);
            // await Task.Delay(10);
        }
    }

    public void StopListener() {
        cancellationToken?.Cancel();
        listener?.Stop();
        listener = null;
        cancellationToken = null;
    }

    public void OnDisable() {
        StopListener();
    }

    ~SmartphoneRemote() {
        StopListener();
    }
}
