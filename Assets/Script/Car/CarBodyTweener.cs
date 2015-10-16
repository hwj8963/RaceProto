using UnityEngine;
using System.Collections;

public class CarBodyTweener : MonoBehaviour {


	public float period = 3f;
	public float angle = 0.6f;

	float time = 0f;
	// Update is called once per frame
	void Update () {/*
		time += Time.deltaTime;

		float angleNow = Mathf.Sin (time / period * 2 * Mathf.PI) * angle;
		transform.localRotation = Quaternion.Euler (0f, 0f, angleNow);*/
	}


	//float angleXMax = 15f;
	//float angleYMax = 15f;

	float forceX = 0f; // target Pos
	float forceY = 0f; // target Pos

	float velX = 10f;
	float velY = 10f;

	float nowX = 0f;
	float nowY = 0f;

	float damp = 10f;
	float accCoeff = 15f;

	public void SetForce(float x, float y) {
		forceX = x;
		forceY = y;
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		/*
		float accX = (forceX - nowX) * accCoeff;
		float accY = (forceY - nowY) * accCoeff;

		velX = velX + accX * dt;
		velX = velX - (velX * damp * dt);

		velY = velY  + accY * dt;
		velY = velY - (velY * damp * dt);

		nowX = nowX + velX * dt;
		nowY = nowY + velY * dt;
	*/
		float delX = velX * dt;
		if (nowX < forceX) {
			nowX = Mathf.Clamp(nowX+delX,nowX,forceX);
		} else if(nowX > forceX) {
			nowX = Mathf.Clamp (nowX-delX,forceX,nowX);
		}

		float delY = velY * dt;
		if (nowY < forceY) {
			nowY = Mathf.Clamp(nowY+delY,nowY,forceY);
		} else if(nowY > forceY) {
			nowY = Mathf.Clamp (nowY-delY,forceY,nowY);
		}



		transform.localRotation = Quaternion.Euler (nowY, 0f, nowX);
	}
}
