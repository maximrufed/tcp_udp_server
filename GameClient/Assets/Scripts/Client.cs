using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
	public static Client instance;
	public static int dataBufferSize = 4096;
	
	public string ip = "127.0.0.1";
	public int port = 26950;
	public int myId = 0;
	public TCP tcp;
	
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Debug.Log("Instance already exists, destroying object");
			Destroy(this);
		}
	}
	
    void Start()
    {
		tcp = new TCP();
    }

	public void ConnectToServer()
    {
		tcp.Connect();
    }

	public class TCP
    {
		public TcpClient socket;

		private NetworkStream stream;
		private byte[] receiveBuffer;

		public void Connect()
        {
			socket = new TcpClient
			{
				SendBufferSize = dataBufferSize,
				ReceiveBufferSize = dataBufferSize
			};


			receiveBuffer = new byte[dataBufferSize];
			socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

		private void ConnectCallback(IAsyncResult _result)
        {
			socket.EndConnect(_result);

			if (!socket.Connected)
            {
				Debug.Log("Not connected");
				return;
            }

			stream = socket.GetStream();

			stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

		private void ReceiveCallback(IAsyncResult _result)
        {
			try
			{
				int _byteLength = stream.EndRead(_result);
				if (_byteLength <= 0)
				{
					// TODO Disconnect
					return;
				}

				byte[] _data = new byte[_byteLength];
				Array.Copy(receiveBuffer, _data, _byteLength);

				// TODO Handle Data
				stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

			}
			catch (Exception _ex)
			{
				// TODO Disconnect
			}
		}

    }
    
}
