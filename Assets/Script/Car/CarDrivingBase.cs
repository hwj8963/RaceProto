using UnityEngine;
using System.Collections;

public abstract class CarDrivingBase : MonoBehaviour {
	public bool isAccel {
		get;
		set;
	}
	public bool isBrake {
		get;
		set;
	}
	public float steer {
		get;
		set;
	}
}
