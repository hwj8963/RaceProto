using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(TrackBuilder))]
public class TrackBuilderEditor : Editor {

	TrackBuilder trackBuilder;
	void OnEnable() {
		trackBuilder = target as TrackBuilder;
	}
	public override void OnInspectorGUI() {
		base.OnInspectorGUI ();


		if (GUILayout.Button ("Build")) {
			BuildTrack();
		}
	}
	public void BuildTrack() {
		List<Vector3> positions = new List<Vector3>();
		List<Vector3> directions = new List<Vector3> ();

		Vector3 position = Vector3.zero;
		Vector3 direction = new Vector3(0f,0f,1f);
		Vector3 up = new Vector3(0f,1f,0f);

		positions.Add (position);
		directions.Add (direction);

		float splitAngleMax = 10f;
		float height = 1f;

		float vDist = 5f;
		for (int i=0; i<trackBuilder.points.Count; i++) {
			TrackPoint p = trackBuilder.points[i];
			if(p.type == TrackType.Straight) {
				position = position + direction * p.distance;
				positions.Add (position);
				directions.Add (direction);
			}
			else if(p.type == TrackType.Curve) {
				int split = Mathf.CeilToInt(Mathf.Abs(p.curveAngle) / splitAngleMax);
				
				float angle = p.curveAngle / split;
				float dist = 2 * p.curveRadius * Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(angle) / 2f);
				for(int j=0;j<split;j++) {
					position = position + Quaternion.AngleAxis(angle/2f,up) * direction *dist;
					direction = Quaternion.AngleAxis(angle,up)*direction;

					positions.Add (position);
					directions.Add (direction);
				}
			}
		}


		List<Vector3> vertexList = new List<Vector3> ();
		List<Vector2> uvList = new List<Vector2> ();
		List<int> triangleList = new List<int> ();



		Vector3 hVec = new Vector3(0f,height,0f);
		float vPos = 0f;
		for(int i=0;i<positions.Count;i++) {
			if(i> 0) {
				vPos += Vector3.Distance(positions[i-1],positions[i])/vDist;
			}
			Vector3 posNow = positions[i];
			Vector3 rightVec = Vector3.Cross(directions[i],up).normalized * trackBuilder.width;
			Vector3 right = posNow + rightVec;
			Vector3 left = posNow - rightVec;
			vertexList.Add (left);
			vertexList.Add (right);
			vertexList.Add (right+hVec);
			vertexList.Add (left+hVec);
			uvList.Add (new Vector2(0f,vPos));
			uvList.Add(new Vector2(0.75f,vPos));
			uvList.Add (new Vector2(1f,vPos));
			uvList.Add (new Vector2(-0.25f,vPos));
		}
		for (int i=0; i<positions.Count-1; i++) {
			/*
			triangleList.Add (i*2);
			triangleList.Add (i*2+1);
			triangleList.Add (i*2+3);
			
			triangleList.Add (i*2);
			triangleList.Add (i*2+3);
			triangleList.Add (i*2+2);
*/

			triangleList.Add (i*4);
			triangleList.Add (i*4+1);
			triangleList.Add (i*4+5);

			triangleList.Add (i*4);
			triangleList.Add (i*4+5);
			triangleList.Add (i*4+4);

			triangleList.Add (i*4);
			triangleList.Add (i*4+4);
			triangleList.Add (i*4+3);

			triangleList.Add (i*4+3);
			triangleList.Add (i*4+4);
			triangleList.Add (i*4+7);

			triangleList.Add (i*4+1);
			triangleList.Add (i*4+6);
			triangleList.Add (i*4+5);

			triangleList.Add (i*4+1);
			triangleList.Add (i*4+2);
			triangleList.Add (i*4+6);
		}

		int triangleCount = triangleList.Count;
		for(int i=0;i<triangleCount;i+=3) {
			triangleList.Add (triangleList[i]);
			triangleList.Add (triangleList[i+2]);
			triangleList.Add (triangleList[i+1]);
		}
		Mesh mesh = new Mesh ();
		mesh.vertices = vertexList.ToArray ();
		mesh.uv = uvList.ToArray ();
		mesh.triangles = triangleList.ToArray ();
		AssetDatabase.CreateAsset (mesh, "Assets/" + trackBuilder.name + ".asset");


	}


	public void OnSceneGUI() {
		Handles.color = Color.red;

		Vector3 position = trackBuilder.transform.position;
		Vector3 direction = new Vector3 (0f, 0f, 1f);
		Vector3 up = new Vector3 (0f, 1f, 0f);
		for (int i=0; i<trackBuilder.points.Count; i++) {
			Handles.CubeCap(1000+i,position,Quaternion.identity,3f);
			TrackPoint p = trackBuilder.points[i];
			if(p.type == TrackType.Straight) {
				Vector3 next = position + direction * p.distance;
				Handles.DrawLine (position,next);
				position = next;
			} else if(p.type == TrackType.Curve) {
				Vector3 center = position;
				Vector3 pToCenter  = Vector3.Cross(direction,up) * p.curveRadius;
				if(p.curveAngle <= 0) {
					center += pToCenter;
				} else {
					center -= pToCenter;
				}
				Handles.DrawWireArc(center,up,position-center,p.curveAngle,p.curveRadius);
				Quaternion q = Quaternion.AngleAxis(p.curveAngle,up);
				direction = q*direction;
				position = center + q * (position-center);
			}
		}

		Handles.CubeCap (1000 + trackBuilder.points.Count, position, Quaternion.identity, 3f);
	}

	//angle : degree
	//+angle : turn right
	public void BuildMeshCurve (string name,float rMin, float rMax , float angle, int splitNum,float height,int uvSplit) {
		float sign = Mathf.Sign (angle);
		if (angle < 0) {
			angle = -angle;
		}
		float rRight = sign > 0 ? rMin : rMax;
		float rLeft = sign > 0 ? rMax : rMin;

		Vector3[] vertices = new Vector3[(splitNum + 1) * 4];
		int[] triangles = new int[splitNum * 6 * 3 * 2];
		Vector2[] uvs = new Vector2[(splitNum + 1) * 4];

		Vector3 circleCenter = new Vector3 ((rMin + rMax) / 2f * sign, 0f, 0f);
		Vector3 heightVec = new Vector3 (0f, height, 0f);
		int offset = (splitNum + 1) * 2;
		for (int i=0; i<=splitNum; i++) {
			float angleNowRad = angle * i / splitNum * Mathf.Deg2Rad;
			Vector3 rightPoint = circleCenter + rRight * new Vector3(-Mathf.Cos(angleNowRad)*sign,0f,Mathf.Sin(angleNowRad));
			Vector3 leftPoint = circleCenter + rLeft * new Vector3(-Mathf.Cos(angleNowRad)*sign,0f,Mathf.Sin (angleNowRad));
          	vertices[i*2] = rightPoint;
			vertices[i*2+1] = leftPoint;
			vertices[i*2 + offset] = rightPoint + heightVec;
			vertices[i*2 + offset + 1] = leftPoint + heightVec;

			float vPos = 1f - 1f * i / splitNum * uvSplit;
			uvs[i*2] = new Vector2(0.75f,vPos);
			uvs[i*2+1] = new Vector2(0f,vPos);
			uvs[i*2 + offset] = new Vector2(1f,vPos);
			uvs[i*2 + offset+1] = new Vector2(-0.25f,vPos);
		}

		int triOffset = splitNum * 6 * 3;
		for (int i=0; i<splitNum; i++) {
			triangles[i*18] = i*2;
			triangles[i*18+1] = i*2+2;
			triangles[i*18+2] = i*2+1;

			triangles[i*18+3] = i*2+1;
			triangles[i*18+4] = i*2+2;
			triangles[i*18+5] = i*2+3;

			triangles[i*18+6] = i*2;
			triangles[i*18+7] = i*2+2+offset;
			triangles[i*18+8] = i*2+2;

			triangles[i*18+9] = i*2;
			triangles[i*18+10] = i*2+offset;
			triangles[i*18+11] = i*2+2+offset;

			triangles[i*18+12] = i*2+1;
			triangles[i*18+13] = i*2+3;
			triangles[i*18+14] = i*2+1+offset;

			triangles[i*18+15] = i*2+3;
			triangles[i*18+16] = i*2+3+offset;
			triangles[i*18+17] = i*2+1+offset;

		}
		for (int i=triOffset; i<triOffset*2; i+=3) {
			triangles[i] = triangles[i-triOffset];
			triangles[i+1] = triangles[i-triOffset+2];
			triangles[i+2] = triangles[i-triOffset+1];
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		AssetDatabase.CreateAsset (mesh, "Assets/" + name + ".asset");
		                             

	}



	public void BuildMeshTest() {
		Mesh mesh = new Mesh ();
		Vector3[] vertices = new Vector3[8];
		vertices [0] = new Vector3 (-3f, 0f, -3f);
		vertices [1] = new Vector3 (3f, 0f, -3f);
		vertices [2] = new Vector3 (3f, 0f, 3f);
		vertices [3] = new Vector3 (-3f, 0f, 3f);
		vertices [4] = new Vector3 (-3f, 1f, -3f);
		vertices [5] = new Vector3 (3f, 1f, -3f);
		vertices [6] = new Vector3 (3f, 1f, 3f);
		vertices [7] = new Vector3 (-3f, 1f, 3f);

		Vector2[] uvs = new Vector2[8];
		uvs [0] = new Vector2 (0f, 0f);
		uvs [1] = new Vector2 (0.75f, 0f);
		uvs [2] = new Vector2 (0.75f, 1f);
		uvs [3] = new Vector2 (0f, 1f);
		uvs [4] = new Vector2 (-0.2f, 0f);
		uvs [5] = new Vector2 (1f, 0f);
		uvs [6] = new Vector2 (1f, 1f);
		uvs [7] = new Vector2 (-0.2f, 1f);

		int[] triangles = new int[6*3];
		triangles [0] = 0;
		triangles [1] = 3;
		triangles [2] = 1;
		triangles [3] = 1;
		triangles [4] = 3;
		triangles [5] = 2;
		triangles [6] = 0;
		triangles [7] = 4;
		triangles [8] = 7;
		triangles [9] = 0;
		triangles [10] = 7;
		triangles [11] = 3;
		triangles [12] = 1;
		triangles [13] = 2;
		triangles [14] = 5;
		triangles [15] = 2;
		triangles [16] = 6;
		triangles [17] = 5;


		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;

		AssetDatabase.CreateAsset (mesh, "Assets/straight.asset");
	}

}