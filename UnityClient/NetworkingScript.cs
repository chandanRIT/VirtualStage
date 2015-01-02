using UnityEngine;
using System.Collections;

public class NetworkingScript : MonoBehaviour {
	
	public string IP = "127.0.0.1";			// server IP
	public int Port = 250001;				// server Port
	
	public GameObject audience;
	public Transform[] spawnPoints;			// all possible spawn positions
	
	private GameObject clientCharacter;		// local instantance of the character
	private Camera localCamera;
	
	public GameObject mainCharacter;		// local instantance of the main character
	private Camera mainCharacterCamera;
	//	This is used for testing animations
	//	private bool flag = false;
	
	void OnGUI() {
		if (Network.peerType == NetworkPeerType.Disconnected) {
			
			if (GUI.Button(new Rect(100, 100, 100, 25), "Start Client")) {
				Network.Connect(IP, Port);
			}
			if (GUI.Button(new Rect(100, 125, 100, 25), "Start Server")) {
				Network.InitializeServer(10, Port);
			}
			
			Camera.SetupCurrent(Camera.main);
		} else {
			if (Network.peerType == NetworkPeerType.Client) {
				GUI.Label(new Rect(100, 100, 100, 125), "Client");
				
				if (clientCharacter == null) {
					if (GUI.Button(new Rect(100, 125, 100, 25), "Main Charachter")) {
					
						networkView.RPC("addMainCharacter", RPCMode.All);
						
						localCamera = clientCharacter.AddComponent<Camera> ();
						Camera.SetupCurrent(localCamera);
						
						Transform[] bodyParts = clientCharacter.GetComponentsInChildren<Transform>();
						foreach (Transform item in bodyParts) {
							if (item.name == "LeftEye" || item.name == "Head") {
								localCamera.transform.position = item.transform.position;
							}
						}
					}
				
					if (GUI.Button(new Rect(100, 150, 100, 25), "Audience")) {
						networkView.RPC("addAudience", RPCMode.All, Random.Range (0, spawnPoints.Length));
						
						localCamera = clientCharacter.AddComponent<Camera> ();
						Camera.SetupCurrent(localCamera);
						
						Transform[] bodyParts = clientCharacter.GetComponentsInChildren<Transform>();
						foreach (Transform item in bodyParts) {
							if (item.name == "LeftEye" || item.name == "Head") {
								localCamera.transform.position = item.transform.position;
							}
						}
						
						//GameObject characterHead = GameObject.Find(clientCharacter.name + "/Hips/Spine/Spine1/Spine2/Neck/Neck1/Head/LeftEye");
						//transform.position = characterHead.transform.position;
						localCamera.transform.LookAt(new Vector3 (0, 2, 0));
					}
				}
				
				if (GUI.Button(new Rect(100, 175, 100, 25), "Logout")) {
					networkView.RPC("deleteAudience", RPCMode.All);
					Network.Disconnect(250);
				}
				
			}
			if (Network.peerType == NetworkPeerType.Server) {
				GUI.Label(new Rect(100, 100, 100, 125), "Server");
				GUI.Label (new Rect(100, 125, 100, 25), "Connections " + Network.connections.Length);
				
				if (GUI.Button(new Rect(100, 150, 100, 25), "Logout")) {
					Network.Disconnect(250);
				}
			}
		}
	}
	
	// This is for movement
	void CameraView(Transform t) {
		float speed = 10.0F;
		float rotationSpeed = 100.0F;
		
		float translation = Input.GetAxis("Vertical") * speed;
		float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		
		if (Input.GetKey(KeyCode.RightShift)) {
			// Strafing
			t.Translate(translation, 0, 0);
		} else {
			// Normal Forward/Back
			t.Translate(0, 0, translation);
		}
		
		if (Input.GetKey(KeyCode.RightShift)) {
			t.Rotate(rotation, 0, 0);
		} else if(Input.GetKey(KeyCode.RightControl)) {
			t.Rotate(0, 0, rotation);
		} else {
			t.Rotate(0, rotation, 0);
		}
		
		if (Input.GetKey(KeyCode.F)) {
			//transform.position = new Vector3(0, 0, 0);
			t.rotation = new Quaternion();
		}
	}
	
	void Update() {
		if (Network.peerType == NetworkPeerType.Server) {
			CameraView(Camera.main.transform);	
		}
	}
	
	[RPC]
	void addMainCharacter() {
		Vector3 stageCenter = new Vector3 (0, 2, 0);
		clientCharacter = Instantiate(mainCharacter, stageCenter, new Quaternion()) as GameObject;
		//		Animator animator = clientCharachter.GetComponent<Animator> ();
		//		animator.speed = flag ? 1.0f : 0.0f;
		//		flag = flag ? false : true;
	}
	
	[RPC]
	void deleteMainCharacter() {
		GameObject.Destroy (clientCharacter);
		clientCharacter = null;
	}
	
	[RPC]
	void addAudience(int index) {
		Vector3 stageCenter = new Vector3 (0, 2, 0);
		clientCharacter = Instantiate(audience, spawnPoints[index].position, spawnPoints[index].rotation) as GameObject;
		//		Animator animator = clientCharachter.GetComponent<Animator> ();
		//		animator.speed = flag ? 1.0f : 0.0f;
		//		flag = flag ? false : true;
	}
	
	[RPC]
	void deleteAudience() {
		GameObject.Destroy (clientCharacter);
		clientCharacter = null;
	}
}
