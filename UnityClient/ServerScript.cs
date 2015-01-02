using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerScript : MonoBehaviour {
	
	public string IP = "127.0.0.1";			// server IP
	public int Port = 250001;				// server Port	
	public GameObject audienceAvatar;
	public Transform[] spawnPoints;			// all possible spawn positions
	
	private Hashtable lookup;
	private Hashtable clientsConnected;
	
	// Use this for initialization
	void Start () {
		lookup = new Hashtable();
		clientsConnected = new Hashtable ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Network.peerType == NetworkPeerType.Server) {
			Camera.main.fieldOfView = 60;
			CameraView(Camera.main);	
		}
	}
	
	void OnGUI() {
		if (Network.peerType == NetworkPeerType.Disconnected) {
			GUI.Label(new Rect(200, 125, 100, 25), "Server Port");
			Port = int.Parse(GUI.TextField(new Rect(300, 125, 100, 25), Port.ToString()));
			if (GUI.Button(new Rect(100, 125, 100, 25), "Start Server")) {
				Network.InitializeServer(10, Port);
				MasterServer.RegisterHost("VirtualPerformace", "VirtualStage");
			}
			
			Camera.SetupCurrent(Camera.main);
		} else {
			if (Network.peerType == NetworkPeerType.Server) {
				GUI.Label(new Rect(100, 100, 200, 25), "Server IP: " + Network.player.externalIP + " : " + Network.player.externalPort);
				GUI.Label (new Rect(100, 125, 100, 25), "Connections: " + Network.connections.Length);
				GUI.Label (new Rect(100, 150, 100, 25), "Lookup: " + lookup.Keys.Count);
				
				if (GUI.Button(new Rect(100, 175, 100, 25), "Logout")) {
					Network.Disconnect(250);
				}
			}
		}
	}
	
	// This is for camera movement
	void CameraView(Camera cam) {
		float speed = 10.0F;
		float rotationSpeed = 100.0F;
		
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		
		if (Input.GetKey(KeyCode.RightShift)) {
			// Strafing
			cam.transform.Translate(translation, 0, 0);
		} else {
			// Normal Forward/Back
			cam.transform.Translate(0, 0, translation);
		}
		
		if (Input.GetKey(KeyCode.RightShift)) {
			cam.transform.Rotate(rotation, 0, 0);
		} else if(Input.GetKey(KeyCode.RightControl)) {
			cam.transform.Rotate(0, 0, rotation);
		} else {
			cam.transform.Rotate(0, rotation, 0);
		}
		
		if (Input.GetKey(KeyCode.F)) {
			cam.transform.rotation = new Quaternion();
		}
	}
	
	// This is for player movement
	[RPC]
	void CharacterMove(string id, float vertical, float horizontal, bool isRotate) {
		Debug.Log (string.Format("ServerCall {0}: V->{1}, H->{2}, R->{3}", id, vertical, horizontal, isRotate));
		float moveSpeed = 10.0F;
		float rotationSpeed = 100.0F;
		GameObject clientCharacter = lookup [id] as GameObject;
		networkView.RPC("CharacterMoveClient", RPCMode.OthersBuffered, id, vertical, horizontal, isRotate);
		
		vertical *= Time.deltaTime;
		horizontal *= Time.deltaTime;
		
		if (isRotate) {
			// rotate
			clientCharacter.transform.Rotate(0, horizontal * rotationSpeed, 0);
		} else {
			// move
			// Normal Forward/Back
			clientCharacter.transform.Translate(0, 0, vertical * moveSpeed);
			// Strafing
			clientCharacter.transform.Translate(horizontal * moveSpeed, 0, 0);
		}
		
		//		if (Input.GetKey(KeyCode.F)) {
		//			// Face front
		//			clientCharacter.transform.rotation = new Quaternion();
		//		}
	}
	
	[RPC]
	void addAudience(string id) {
		int spawnIndex = Random.Range(0, spawnPoints.Length - 1);	//TODO: Check if this method generates different spawn locations on different clients 
		if (!lookup.ContainsKey(id)) {
			GameObject clientCharacter = Instantiate(audienceAvatar, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation) as GameObject;
			lookup[id] = clientCharacter;
		}
		networkView.RPC("setClientCharacter", RPCMode.OthersBuffered, id, spawnIndex);
	}
	
	[RPC]
	void addMainCharacter(string id) {
		int spawnIndex = Random.Range(0, spawnPoints.Length - 1);	//TODO: Check if this method generates different spawn locations on different clients 
		if (!lookup.ContainsKey(id)) {
			GameObject clientCharacter = Instantiate(audienceAvatar, new Vector3(0, 2, 0), new Quaternion()) as GameObject;
			lookup[id] = clientCharacter;
		}
		networkView.RPC("setClientCharacter", RPCMode.OthersBuffered, id, -1);
	}
	
	[RPC]
	void deleteAudience(string id) {
		if (lookup.ContainsKey(id)) {
			networkView.RPC("removeClientCharacter", RPCMode.OthersBuffered, id);
			GameObject.Destroy(lookup[id] as GameObject);
			lookup.Remove(id);
		}
	}
}
