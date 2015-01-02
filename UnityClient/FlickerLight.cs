using UnityEngine;
using System.Collections;

public class FlickerLight : MonoBehaviour {

	public GameObject particle;
	private float flickerSpeed = 0.1f;
	private int randomizer = 0;

	// Use this for initialization
	void Start () {
		StartCoroutine (flicker ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator flicker() {
		while (true) {
			if (randomizer == 0) {
					particle.light.enabled = false;
			} else {
					particle.light.enabled = true;
			}
			randomizer = (int)Random.Range (0.0f, 1.1f);
			yield return new WaitForSeconds (flickerSpeed);
		}
	}
}
