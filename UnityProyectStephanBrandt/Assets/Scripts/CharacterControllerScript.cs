using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour {

	public float inputDelay = 0.1f;

	public float forwardVel = 12;

	public float rotateVel = 100;

	private Quaternion targetRotation;

	private Rigidbody rBody;

	private float forwardInput, turnInput;

	void Start () {
		
		if(GetComponent<Rigidbody>()){
			
			targetRotation = transform.rotation;

			rBody = GetComponent<Rigidbody> ();
		}

		forwardInput = turnInput = 0;

	}

	void GetInput(){
		
		forwardInput = Input.GetAxis ("Vertical");

		turnInput = Input.GetAxis ("Horizontal");

	}

	// Update is called once per frame
	void Update () {
		
		GetInput ();

		Turn ();

	}

	//For updating physics components
	void FixedUpdate(){
		
		Run();
	}

	void Run(){
		//If the input that we have is bigger than the predefine 
		//sensitivity that is allowed, for the character to move
		//we move wth the specified speed
		//else, our velocity is zero, so no movement.
		if (Mathf.Abs (forwardInput) > inputDelay) {
			
			rBody.velocity = transform.forward * forwardInput * forwardVel;

		} else {
			
			rBody.velocity = Vector3.zero;

		}
	}

	void Turn(){
		
		//Same as run, we only rotate on the Vector3.up, which is Y axis, so we rotatte around the Y axis
		//and then we pass that rotation to our character
		if (Mathf.Abs (turnInput) > inputDelay) {

			targetRotation *= Quaternion.AngleAxis (rotateVel * turnInput * Time.deltaTime, Vector3.up);

		}

		transform.rotation = targetRotation;

	}
}
