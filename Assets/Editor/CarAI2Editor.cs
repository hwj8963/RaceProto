using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CarAI2))]
public class CarAI2Editor : Editor {

	CarAI2 ai;

	void OnEnable() {
		ai = target as CarAI2;
	}
	void OnSceneGUI() {
		Handles.color = Color.red;
		Vector3 viewPos = ai.ViewPosition;
		Handles.CubeCap (-1, viewPos, Quaternion.identity, 3f);


	}


}
