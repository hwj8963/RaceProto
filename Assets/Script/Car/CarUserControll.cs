using UnityEngine;
using System.Collections;

public class CarUserControll : MonoBehaviour {

	CarDrivingBase driving;
	
	// Use this for initialization
	void Start () {
		driving = GetComponent<CarDrivingBase> ();
	}	
	float steerMax = 120;
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			driving.isAccel = true;
			driving.isBrake = false;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			driving.isAccel = false;
			driving.isBrake = true;
		} else {
			driving.isAccel = false;
			driving.isBrake = false;
		} 
		
		if (Input.GetKey (KeyCode.LeftArrow)) {
			driving.steer = -steerMax;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			driving.steer = steerMax;
		} else {
			driving.steer = 0;
		}

	}
}


//user control
/*
		isAccel = false;
		isBrake = false;
		steer = 0;
		
		
		*/
//