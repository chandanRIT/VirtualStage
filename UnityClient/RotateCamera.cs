using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateCamera : MonoBehaviour {
	
	public List<GameObject> models;
	public GameObject target;//the target object
	private float speedMod = 2.0f;//a speed modifier
	private Vector3 point;//the coord to the point where the camera looks at
	private bool rotate = true;
	private List<Animator> animators;
	private List<GameObject> model_heads;
	private Vector3 last_cam_pos;
	
	// Use this for initialization
	void Start () {
		point = target.transform.position;//get target's coords
		point = point + new Vector3 (0f, 3.5f, 0f);
		transform.LookAt(point);//makes the camera look to it
		
		last_cam_pos = transform.position;// set current camera pos
		
		// get animators for all the models
		animators = models.ConvertAll (m => m.GetComponent<Animator>());
		
		// get head portion for all the models
		model_heads = models.ConvertAll (m => GameObject.Find(m.name + "/Hips/Spine/Spine1/Spine2/Neck/Neck1/Head/LeftEye"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) { 
			rotate = rotate ? false : true;
			if (rotate) {
				transform.position = last_cam_pos;
				transform.LookAt (point);
			}
		}
		
		for (int i = 0; i < models.Count; i++) {
			if (Input.GetKeyDown ((i + 1).ToString ())) {
				rotate = false;
				transform.position = model_heads [i].transform.position;
				transform.LookAt (point);
			}	
		}
		
		animators.ForEach (a => 
		                   { 
			if (Vector3.Distance (transform.position, a.transform.position) > 10) { 
				a.speed = Random.Range(0.1f, 1.0f);
			} else {
				a.speed = 0.0f;
			}});
		
//		if (rotate) {
//			//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
//			transform.RotateAround (point, new Vector3 (0.0f, 1.0f, 0.0f), 20 * Time.deltaTime * speedMod);
//		} else {
//			transform.RotateAround (point, new Vector3 (0.0f, 1.0f, 0.0f), 0);
//		}
	}
}
