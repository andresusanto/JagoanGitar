using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

public class SensorReader : MonoBehaviour {
	public float currentFrequency;
	private Socket socket;

	public class StateObject {
		// Client  socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 1024;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		// Received data string.
		public StringBuilder sb = new StringBuilder();  
	}

	// Use this for initialization
	void Start () {
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );	
		IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		socket.Bind (new IPEndPoint (ipAddress, 1231));
		socket.Listen (100);
		socket.BeginAccept( new AsyncCallback(AcceptCallback), socket);
		Debug.Log ("STARTED");
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void AcceptCallback(IAsyncResult ar) {
		Socket listener = (Socket) ar.AsyncState;
		Socket handler = listener.EndAccept(ar);

		// Create the state object.
		StateObject state = new StateObject();
		state.workSocket = handler;
		handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
	}

	public void ReadCallback(IAsyncResult ar) {
		String content = String.Empty;

		StateObject state = (StateObject) ar.AsyncState;
		Socket handler = state.workSocket;

		int bytesRead = handler.EndReceive(ar);

		if (bytesRead > 0) {
			// There  might be more data, so store the data received so far.
			state.sb.Append(Encoding.ASCII.GetString(
				state.buffer,0,bytesRead));

			// Check for end-of-file tag. If it is not there, read 
			// more data.
			content = state.sb.ToString();
			if (content.IndexOf("\n") > -1) {
				float.TryParse (content.Trim(), out currentFrequency);
				state.sb.Length = 0;
				Debug.Log (content);
			}
			handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
		}
	}
}
