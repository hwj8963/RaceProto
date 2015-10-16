using UnityEngine;
using System.Collections;

public class CarDrivingBasic : CarDrivingBase {

	Rigidbody rigidbody;
	
	public float maxSpeed = 75f;
	public float reverseMaxSpeed = -50f;
	public float accel = 20f;
	public float brake = 30f;
	public float steerMax = 120f;
	
	CarBodyTweener bodyTweener;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		bodyTweener = GetComponentInChildren<CarBodyTweener> ();
	}

	public float speed;
	// Update is called once per frame

	void FixedUpdate () {
		//AI ();
		
		
		float dt = Time.fixedDeltaTime;
		
		speed = rigidbody.velocity.magnitude * Vector3.Dot (rigidbody.velocity.normalized,transform.forward);
		steer = Mathf.Clamp (steer,-steerMax, steerMax);

		float forceX = 0f;
		float forceY = 0f;

		if (rigidbody.velocity.magnitude > 1f && speed > 1) {
			transform.RotateAround (transform.position, transform.up, steer * dt);
			Vector3 dirNow = rigidbody.velocity.normalized;
			Vector3 wantV = transform.forward;
			Vector3 right = Vector3.Cross (dirNow, Vector3.Cross (wantV, dirNow));
			
			rigidbody.AddForce (right * 300000f);

			forceX = steer * speed / 1000f;
		}
		


		
		
		if (speed < maxSpeed && isAccel) {
			rigidbody.AddForce(transform.forward * 20000f);
			forceY = -2f;
		} else if (speed > reverseMaxSpeed && isBrake) {
			rigidbody.AddForce(transform.forward * -30000f);
			forceY = 2f;
		}

		bodyTweener.SetForce (forceX, forceY);
	}
	
	
	

	


}
