using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
	public bool brake;
	public bool side;
}

public class SimpleCarController : MonoBehaviour {
	public List<AxleInfo> axleInfos; 
	public float maxMotorTorque;
	public float maxSteeringAngle;
	Rigidbody rigidBody;

	void Start () {
		rigidBody = this.GetComponent<Rigidbody> ();
		rigidBody.centerOfMass = new Vector3(0f,-0.6f,0f);
	}
	// finds the corresponding visual wheel
	// correctly applies the transform
	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}
		
		Transform visualWheel = collider.transform.GetChild(0);
		
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}
	
	public void FixedUpdate()
	{
		float motor = maxMotorTorque * Input.GetAxis("Vertical");
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		bool brake = Input.GetKey (KeyCode.Z);
		bool side = Input.GetKey (KeyCode.X);
		bool boost = Input.GetKey (KeyCode.C);
		
		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
			if(axleInfo.brake) {
				if(brake) {
					axleInfo.leftWheel.brakeTorque = 12000f;
					axleInfo.rightWheel.brakeTorque = 12000f;
				} else {
					axleInfo.leftWheel.brakeTorque = 0f;
					axleInfo.rightWheel.brakeTorque = 0f;
				}
			}
			if(axleInfo.side) {
				if(side) {
					WheelFrictionCurve curve = axleInfo.leftWheel.sidewaysFriction;
					curve.stiffness = 2f;
					axleInfo.leftWheel.sidewaysFriction = curve;
					axleInfo.rightWheel.sidewaysFriction = curve;
				} else {
					WheelFrictionCurve curve = axleInfo.leftWheel.sidewaysFriction;
					curve.stiffness = 8f;
					axleInfo.leftWheel.sidewaysFriction = curve;
					axleInfo.rightWheel.sidewaysFriction = curve;
				}
			}

			if(boost) {
				Vector3 direction = gameObject.transform.forward;
				rigidBody.AddForce (direction * 200000f);
			}


			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
	}
}