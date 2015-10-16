using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TrackType {
	Straight,
	Curve,
}

[System.Serializable]
public class TrackPoint {
	public float curveRadius;
	public float curveAngle;
	public float distance;
	public TrackType type;
}


public class TrackBuilder : MonoBehaviour {
	public float width = 10f;
	public string name = "track";

	public List<TrackPoint> points = new List<TrackPoint>();
}
