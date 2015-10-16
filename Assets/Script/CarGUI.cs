using UnityEngine;
using System.Collections;

public class CarGUI : MonoBehaviour {

	public Transform car;
	void OnGUI() {
		if (car != null) {
			float vmps = car.GetComponent<Rigidbody>().velocity.magnitude; 
			float vkmph = vmps * 36f / 10f;
			string vStr = string.Format("{0:0} km/h",vkmph);
			GUI.Label (new Rect(30,30,200,20),vStr);
		}
	}

}
