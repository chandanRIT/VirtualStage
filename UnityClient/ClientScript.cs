using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;

enum MoveDirection {
	DONT_MOVE, TURN_LEFT, TURN_RIGHT, MOVE_FRONT, MOVE_BACK, MOVE_LEFT, MOVE_RIGHT, MOVE_FRONTLEFT, MOVE_BACKLEFT, MOVE_FRONTRIGHT, MOVE_BACKRIGHT
}

public class ClientScript : MonoBehaviour {
	
	public string IP = "127.0.0.1";			// server IP
	public int Port = 250001;				// server Port
	public GameObject audienceAvatar;	
	public Transform[] spawnPoints;			// all possible spawn positions
	
	private GameObject clientCharacter;		// local instantance of the character
	private Camera localCamera;
	private MouseLook mouseLook;
	private CharacterController characterController;
	private NetworkViewID myId;
	private Hashtable lookup;
	private string label = "Client";
	private Vector3 front;

	//KC
	private UdpClient udp;
	private int UDP_LISTENPORT = 9877;
	private MoveDirection moveOrTurnCmd;

	// Use this for initialization
	void Start () {
		lookup = new Hashtable();
		front = new Vector3 (0, 0, 0);

		udp = new UdpClient(UDP_LISTENPORT);
		Receive();
	}

	private void MyReceiveCallback(IAsyncResult result) 
	{
		IPEndPoint ip = new IPEndPoint(IPAddress.Any, UDP_LISTENPORT); //mayb 0
		Byte[] arr = udp.EndReceive(result, ref ip);
		string str = System.Text.Encoding.Default.GetString(arr);
		string[] strArr = str.Split(new Char[]{','});

		//Debug.Log("Arr: "+strArr.ToString());
		//updateAvatarPosition (x,y,z);

		if(strArr[0] == "L"){
			moveOrTurnCmd = MoveDirection.TURN_LEFT;
			//Debug.Log("Left Message, rotating cam left");
		} else if(strArr[0] == "R"){
			moveOrTurnCmd = MoveDirection.TURN_RIGHT;
			//Debug.Log("Left Message, rotating cam left");
		} else if (strArr[0] == "A") { //front
			moveOrTurnCmd = MoveDirection.MOVE_FRONT;
		} else if (strArr[0] == "B") { //back
			moveOrTurnCmd = MoveDirection.MOVE_BACK;
		} else if (strArr[0] == "C") { // strafe left
			moveOrTurnCmd = MoveDirection.MOVE_LEFT;
		} else if (strArr[0] == "D") { // strafe right
			moveOrTurnCmd = MoveDirection.MOVE_RIGHT;
		} else if (strArr[0] == "E") { // front and left
			moveOrTurnCmd = MoveDirection.MOVE_FRONTLEFT;
		} else if (strArr[0] == "F") { //back and left
			moveOrTurnCmd = MoveDirection.MOVE_BACKLEFT;
		} else if (strArr[0] == "G") { //front and right
			moveOrTurnCmd = MoveDirection.MOVE_FRONTRIGHT;
		} else if (strArr[0] == "H") { //back and right
			moveOrTurnCmd = MoveDirection.MOVE_BACKRIGHT;
		} else {
			moveOrTurnCmd = MoveDirection.DONT_MOVE;
		}
		
		Receive(); // <-- this will be our loop
	}
	
	private void Receive() 
	{   
		udp.BeginReceive(new AsyncCallback(MyReceiveCallback), null);
	}

	/*private void updateAvatarPosition(double x, double y, double z){
		newx,newy,newz = transformIntoWorldCoords (x,y,z);
		gameObject.transform.position.Set(newx,newy,newz); 
		gameObject.transform.rotation.Set(new_x:,newy,newz,newTheta);

	}*/
	
	// Update is called once per frame
	void Update () {
		if (Network.peerType == NetworkPeerType.Client) {
			if (localCamera != null) {
				//StaticCameraView(localCamera);
				//rotateCam(localCamera);
				if(moveOrTurnCmd == MoveDirection.TURN_LEFT) {
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.0F, -1.0F, true);
				} else if (moveOrTurnCmd == MoveDirection.TURN_RIGHT) {
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.0F, 1.0F, true);
				} 
				else if(moveOrTurnCmd == MoveDirection.MOVE_FRONT){ //front
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.3F, 0.0F, false);
				}
				else if(moveOrTurnCmd == MoveDirection.MOVE_BACK){ //back
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), -0.3F, 0.0F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_LEFT) { //left
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.0F, -0.3F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_RIGHT) { //right
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.0F, 0.3F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_FRONTLEFT ) { //front and left
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.3F, -0.3F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_BACKLEFT ) { // back and left
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), -0.3F, -0.3F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_FRONTRIGHT ) { //front and right
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), 0.3F, 0.3F, false);
				} 
				else if (moveOrTurnCmd == MoveDirection.MOVE_BACKRIGHT ) { //back and right
					networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), -0.3F, 0.3F, false);
				}
			}

			if (clientCharacter != null) {
				RecordMove();
			}
		}
	}

	void rotateCam(Camera cam) {
		float rotationSpeed = 100.0F;
		
		//float verticalRotation = Input.GetAxis("Vertical") * rotationSpeed;
		float horizontalRotation = 0.0F;
		if (moveOrTurnCmd == MoveDirection.TURN_LEFT)
			horizontalRotation = -1.0F*rotationSpeed;
		if(moveOrTurnCmd ==MoveDirection.TURN_RIGHT)
			horizontalRotation = 1.0F*rotationSpeed;
		if(moveOrTurnCmd == 0)
			horizontalRotation = 0.0F;	
		
		//verticalRotation *= Time.deltaTime;
		horizontalRotation *= Time.deltaTime;
		
		//cam.transform.Rotate(verticalRotation, 0, 0);
		cam.transform.Rotate(0, horizontalRotation, 0);
		
		if (Input.GetKey(KeyCode.F)) {
			cam.transform.rotation = new Quaternion();
		}
	}
	
	void OnGUI() {
		if (Network.peerType == NetworkPeerType.Disconnected) {
			GUI.Label(new Rect(200, 100, 100, 25), "Connect To");
			IP = GUI.TextField(new Rect(300, 100, 100, 25), IP);
			Port = int.Parse(GUI.TextField(new Rect(400, 100, 100, 25), Port.ToString()));
			if (GUI.Button(new Rect(100, 100, 100, 25), "Start Client")) {
				Network.Connect(IP, Port);
				myId = Network.AllocateViewID();
			}
			Camera.SetupCurrent(Camera.main);
		} else {
			if (Network.peerType == NetworkPeerType.Client) {
				GUI.Label(new Rect(100, 100, 200, 25), "Client IP: " + Network.player.ipAddress);
				GUI.Label(new Rect(100, 125, 250, 25), "Connected To : " + IP);
				
				if (clientCharacter == null) {
					GUI.Label(new Rect(100, 200, 200, 25), "Connect As : ");
					if (GUI.Button(new Rect(125, 250, 100, 25), "Audience")) {
						networkView.RPC("addAudience", RPCMode.Server, myId.ToString());
						label = "Crowd";
					}
					if (GUI.Button(new Rect(125, 225, 100, 25), "Main Charachter")) {
						networkView.RPC("addMainCharacter", RPCMode.Server, myId.ToString());
						label = "Main Character";
					}
				}
				
				GUI.Label(new Rect(100, 150, 100, 25), label);
				
				if (GUI.Button(new Rect(100, 175, 100, 25), "Logout")) {
					clientCharacter = null;
					networkView.RPC("deleteAudience", RPCMode.Server, myId.ToString());
					label = "Idle";
					Network.Disconnect(250);
					Camera.Destroy(localCamera);
					localCamera = null;
				}
			}
		}
	}
	
	// This is for camera movement
	void StaticCameraView(Camera cam) {
		
		float rotationSpeed = 100.0F;
		
		// THIS CODE IS FOR USING ARROW KEYS TO MANIPULATE CAMERA
		float verticalRotation = Input.GetAxis("Vertical") * rotationSpeed;
		float horizontalRotation = Input.GetAxis("Horizontal") * rotationSpeed;
		verticalRotation *= Time.deltaTime;
		horizontalRotation *= Time.deltaTime;
		
		cam.transform.Rotate(verticalRotation, 0, 0);
		cam.transform.Rotate(0, horizontalRotation, 0);
		if (Input.GetKey(KeyCode.F)) {
			cam.transform.rotation = new Quaternion();
		}
	}
	
	void RecordMove() {
		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");
		bool isRotate = Input.GetKey (KeyCode.RightShift);
		if (vertical != 0 || horizontal != 0) {
			networkView.RPC("CharacterMove", RPCMode.Server, myId.ToString(), vertical, horizontal, isRotate);
		}
	}
	
	// This is for player movement
	[RPC]
	void CharacterMoveClient(string lookupIndex, float vertical, float horizontal, bool isRotate) {
		//Debug.Log (string.Format ("ClientCall {0}: V->{1}, H->{2}, R->{3}", lookupIndex, vertical, horizontal, isRotate));
		float moveSpeed = 10.0F;
		float rotationSpeed = 100.0F;
		GameObject character = lookup [lookupIndex] as GameObject;
		
		vertical *= Time.deltaTime;
		horizontal *= Time.deltaTime;
		
		if (isRotate) {
			// rotate
			character.transform.Rotate(0, horizontal * rotationSpeed, 0);
		} else {
			// move
			// Normal Forward/Back
			character.transform.Translate(0, 0, vertical * moveSpeed);
			// Strafing
			character.transform.Translate(horizontal * moveSpeed, 0, 0);
			//Debug.Log ("M");
		}
		
		//		if (Input.GetKey(KeyCode.F)) {
		//			// Face front
		//			clientCharacter.transform.rotation = new Quaternion();
		//		}
	}

	
	[RPC]
	void setClientCharacter(string lookupIndex, int spawnIndex) {
		if (!lookup.ContainsKey (lookupIndex)) {
			if (spawnIndex == -1) {
				// Main charachter
				lookup [lookupIndex] = Instantiate (audienceAvatar, new Vector3(0, 2, 0), new Quaternion()) as GameObject;
				front = spawnPoints[0].position;
				front.y = 1;
			} else {
				lookup [lookupIndex] = Instantiate (audienceAvatar, spawnPoints [spawnIndex].position, spawnPoints [spawnIndex].rotation) as GameObject;
				front = new Vector3(0, 3, 0);
			}
			
			if (lookupIndex == myId.ToString ()) {
				clientCharacter = lookup[lookupIndex] as GameObject;	
				GameObject proxyObj = GameObject.Instantiate(new GameObject(), Vector3.up, new Quaternion()) as GameObject;
				proxyObj.transform.parent = clientCharacter.transform;
				mouseLook = proxyObj.AddComponent<MouseLook>();
				localCamera = proxyObj.AddComponent<Camera>();
				Camera.SetupCurrent(localCamera);
				
				Transform[] bodyParts = clientCharacter.GetComponentsInChildren<Transform>();
				foreach (Transform item in bodyParts) {
					if (item.name == "LeftEye" || item.name == "Head") {
						localCamera.transform.position = item.transform.position;
					}
				}
				if (spawnIndex == -1) {
					localCamera.transform.rotation = new Quaternion();
				} else {
					localCamera.transform.LookAt(front);
				}
				
				localCamera.fieldOfView = 60;
			}
		}
	}
	
	[RPC]
	void removeClientCharacter(string lookupIndex) {
		if (lookup.ContainsKey(lookupIndex)) {
			GameObject.Destroy(lookup[lookupIndex] as GameObject);
			lookup.Remove(lookupIndex);
			if (lookupIndex == myId.ToString()) {
				clientCharacter = null;
			}
		}
	}
}
