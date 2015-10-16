using UnityEngine;
using System.Collections;

public class CameraMoving : MonoBehaviour {

	public Transform car;
	Vector3 relativePos;
	// Use this for initialization
	void Start () {
		relativePos = this.transform.position - car.position;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = car.position + relativePos;
	}
}
