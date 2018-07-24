using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;

	//How fast do we want to look to the target
	//Giving smoothness to the camera
	public float lookSmooth = 0.09f;

	//Distane to target
	public Vector3 offsetFromTarget = new Vector3(0,3,-8);

	//How far in x-Axis will our camera be rotated
	//for 3rd person perspective
	public float xTilt = 10;

	Vector3 destination = Vector3.zero;

	float rotateVel = 0;

	void Start(){
		
		if(target == null){
			
			Debug.Log ("Missing a character on the camera script");

		}

	}

	//To ensure that we get the latest input
	void LateUpdate(){
		
		//moving
		moveToTarget ();

		//rotating
		lookAtTarget ();

	}

	void moveToTarget(){
		
		//Sets our camera to always be behind our character
		destination = target.rotation * offsetFromTarget;

		destination += target.position;

		transform.position = destination;

	}

	void lookAtTarget(){
		
		//We always look at our character
		float eulerYAngle = Mathf.SmoothDampAngle (transform.eulerAngles.y, target.eulerAngles.y, ref rotateVel, lookSmooth);

		//I get a quaternion, when i pass a Vector3 (or floats)
		//And we set it to our cameras new rotation
		transform.rotation = Quaternion.Euler (transform.eulerAngles.x, eulerYAngle, 0);

	}

}
