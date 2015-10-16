using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/**
 * 
 */
[RequireComponent(typeof(CarDrivingBase))]
public class CarAI1 : MonoBehaviour {

	CarDrivingBase driving;

	// Use this for initialization
	void Start () {
		InitAI ();
		driving = GetComponent<CarDrivingBase> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		AI ();
	}

	public TrackBuilder trackBuilder;
	int trackIdx = 0;
	
	public List<Vector3> positions {
		get;
		private set;
	}
	public List<Vector3> directions {
		get;
		private set;
	}

	void InitAI() {
		positions = new List<Vector3> ();
		directions = new List<Vector3> ();
		Vector3 position = Vector3.zero;
		Vector3 direction = new Vector3(0f,0f,1f);
		Vector3 up = new Vector3(0f,1f,0f);
		
		positions.Add (position);
		directions.Add (direction);
		
		for (int i=0; i<trackBuilder.points.Count; i++) {
			TrackPoint p = trackBuilder.points[i];
			if(p.type == TrackType.Straight) {
				position = position + direction * p.distance;
				positions.Add (position);
				directions.Add (direction);
			}
			else if(p.type == TrackType.Curve) {
				float splitAngleMax = 10f;
				int split = Mathf.CeilToInt(Mathf.Abs(p.curveAngle) / splitAngleMax);
				
				float angle = p.curveAngle / split;
				float dist = 2 * p.curveRadius * Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(angle) / 2f);
				for(int j=0;j<split;j++) {
					position = position + Quaternion.AngleAxis(angle/2f,up) * direction *dist;
					direction = Quaternion.AngleAxis(angle,up)*direction;
				}
				positions.Add (position);
				directions.Add (direction);
			}
		}
	}
	
	public float alpha {
		get;
		private set;
	}
	public Vector3 Nearist {
		get {
			TrackPoint tp = trackBuilder.points[trackIdx];
			Vector3 tPos = positions [trackIdx];
			Vector3 tDir = directions [trackIdx];
			if(tp.type == TrackType.Straight) {
				return tPos + tDir * tp.distance * alpha;
			} else if(tp.type == TrackType.Curve) {
				Vector3 center = tPos - tp.curveRadius * Vector3.Cross (tDir,Vector3.up) * Mathf.Sign(tp.curveAngle);
				float angle = tp.curveAngle * alpha;
				Vector3 CtoP = tPos - center;
				return center + Quaternion.AngleAxis(angle,Vector3.up) * CtoP;
			} else {
				return Vector3.zero;
			}
		}
	}
	
	public Vector3 NearistDir {
		get {
			TrackPoint tp = trackBuilder.points[trackIdx];
			Vector3 tPos = positions [trackIdx];
			Vector3 tDir = directions [trackIdx];
			if(tp.type == TrackType.Straight) {
				return tDir;
			} else if(tp.type == TrackType.Curve) {
				return Quaternion.AngleAxis(tp.curveAngle * alpha,Vector3.up) * tDir;
			} else {
				return Vector3.zero;
			}
		}
	}

	void AI() {
		driving.isAccel = true;
		
		Vector3 posMe = transform.position;
		posMe.y = 0;
		Vector3 dirMe = transform.forward;
		
		TrackPoint tpNow = trackBuilder.points [trackIdx];
		Vector3 tPos = positions [trackIdx];
		Vector3 tDir = directions [trackIdx];
		if (tpNow.type == TrackType.Straight) {
			float angle = Vector3.Angle(tDir,posMe - tPos);
			float d = (posMe - tPos).magnitude;
			
			alpha = d * Mathf.Cos(Mathf.Deg2Rad * angle)  / tpNow.distance;
			if(alpha > 1) {
				trackIdx ++;
				trackIdx = trackIdx % trackBuilder.points.Count;
				AI ();
				return;
			} else {
				Vector3 near = Nearist;
				Vector3 nearDir = NearistDir;
				float r = Vector3.Dot (posMe - near,Vector3.Cross (nearDir,Vector3.up));
				float a = Mathf.Asin(Vector3.Dot (Vector3.up,Vector3.Cross (nearDir,dirMe))) * Mathf.Rad2Deg;
				driving.steer = -Mathf.Sign(a) * a*a*5f + r * 35f;
			}
		} else if(tpNow.type == TrackType.Curve) {
			Vector3 center = tPos - tpNow.curveRadius * Vector3.Cross (tDir,Vector3.up) * Mathf.Sign(tpNow.curveAngle);
			float angle = Vector3.Angle (tPos - center , posMe - center);
			if(Vector3.Dot (tDir,posMe - tPos) < 0) {
				angle = 360 - angle;
			}
			alpha = angle/Mathf.Abs(tpNow.curveAngle);
			if(alpha > 1 ) {
				trackIdx ++ ;
				trackIdx = trackIdx % trackBuilder.points.Count;
				AI ();
				return;
			} else {
				Vector3 near = Nearist;
				Vector3 nearDir = NearistDir;
				float r = Vector3.Dot (posMe - near,Vector3.Cross (nearDir,Vector3.up));
				float a = Mathf.Asin(Vector3.Dot (Vector3.up,Vector3.Cross (nearDir,dirMe))) * Mathf.Rad2Deg;
				driving.steer = -Mathf.Sign(a) * a*a*5f + r * 35f;
			}
			
		}
		
	}
}
