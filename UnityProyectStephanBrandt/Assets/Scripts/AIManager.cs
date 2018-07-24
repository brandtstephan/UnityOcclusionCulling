using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Struct of  AI on leichtgewicht
public struct aiStruct{
	public Vector3 pos;
	public Vector3 dir;
	public bool isActivated;
	public Vector3 randomPos;

	public aiStruct(Vector3 p, Vector3 d){
		pos = p;
		dir = d;
		isActivated = false;
		randomPos = Vector3.zero;
	}
}

public class AIManager : MonoBehaviour {
	//
	public Material material;
	public float forwardVel = 0.12f;

	//Scale of our terrain, used to create new positions
	public Transform terrain;
	private float terrainDistance;
	
	//List of all our AI
	private GameObject[] listOfAiObjects;
	private aiStruct[] listOfAiStructs;

	//allowed AI
	public int numAI;
	public int amountAllowed;

	//The sphere radius we use to check whether we do full render or only
	//date update
	private GameObject leichtgetwichtKügel;

	//I need to instatiate this to a game object(Prefab), which we render and then work with that
	public GameObject prefabGameObject;

	//Used for rendering simple geometry using OpenGL
	public bool fovActivated;

	// Use this for initialization
	void Start () {
		
		terrainDistance = (terrain.localScale.x / 2);

		fovActivated = false;


		//If amount of AI bigger or equal 0 and the amount of AI is smaller or equal to the
		//maximum amount of AI allowed.
		if (numAI >= 0 && amountAllowed > 0 && numAI <= amountAllowed) {

			//List of Structs
			listOfAiStructs = new aiStruct[numAI];

			//We intialize the list with this amount of AI
			listOfAiObjects = new GameObject[numAI];

			for (int i = 0; i < listOfAiObjects.Length; i++) {
				//And create each AI
				StartAI(i);
			}
		}

		if (leichtgetwichtKügel == null) {
			//initialize sphere
			leichtgetwichtKügel = GameObject.FindGameObjectWithTag ("LeichtgewichtRender");
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		manageList ();

		manageRendering ();

	}

	//Manage creation of AI on runtime
	void manageList(){

		if (numAI > amountAllowed) {
			//We cap the amount of AI we want to create.
			numAI = amountAllowed;

		}

		if (numAI != listOfAiObjects.Length ) {
			
			changeSize ();

		}

	}

	//Used to check whether a GameObject AI is on the field of view of our camera
	void manageRendering(){
		for (int i = 0; i < listOfAiObjects.Length; i++) {
			
			//We turn our AI position into a Viewport point,
			//If the x,y elements of this point are between 0 and 1
			//and the y between 0 and 50 (Basically the far and near planes)
			//means that this point is on the field of view of the camera
			Vector3 pointOnScreen;

			bool isInsideSphere;

			//Is the AI GameObject is the one active

			if (listOfAiObjects [i].activeSelf) {
				//we save its position
				pointOnScreen = Camera.main.WorldToViewportPoint (listOfAiObjects [i].transform.position);

				isInsideSphere = isOnLeichtgewichtRender (listOfAiObjects [i].transform.position);

			} else {
				
				pointOnScreen = Camera.main.WorldToViewportPoint (listOfAiStructs [i].pos);

				isInsideSphere = isOnLeichtgewichtRender (listOfAiStructs[i].pos);

			}

			//If AI or Struct inside of the field of view
			if (pointOnScreen.z > 0 && pointOnScreen.z < 50 && pointOnScreen.x > 0 && pointOnScreen.x < 1 && pointOnScreen.y > 0 && pointOnScreen.y < 1) {

				listOfAiObjects [i].SetActive (true);

				//If the struct is the one active
				if (listOfAiStructs [i].isActivated) {
						
					aiStruct currentAi = listOfAiStructs [i];

					//We pass this values to the GameObject itself
					//And deactive the struct
					listOfAiObjects [i].transform.forward = currentAi.dir;

					listOfAiObjects [i].transform.position = currentAi.pos;

					listOfAiObjects [i].GetComponent<AIController> ().randomPos = currentAi.randomPos;

					//This line is to pass the new destination to the NavMesh.
					//If we send this to our NavMesh before our AIController script gets fired
					//We get a nullpoint exception, since the other in which the values get started
					//Change from one script to the other
					listOfAiObjects [i].GetComponent<AIController> ().setValue (currentAi.randomPos);

					listOfAiStructs [i].isActivated = false;

				}

				//If inside the sphere but not inside the field of view
			} else if (isInsideSphere) {

				//If our struct isnt active yet
				if (!listOfAiStructs [i].isActivated) {
					
					//And our GameObject is
					if (listOfAiObjects [i].activeSelf) {

						//We pass those values to the struct and deactive the GameObject
						listOfAiStructs [i].pos = listOfAiObjects [i].transform.position;

						listOfAiStructs [i].dir = listOfAiObjects [i].transform.forward;

						listOfAiStructs [i].randomPos = listOfAiObjects [i].GetComponent<AIController> ().randomPos;

						listOfAiObjects [i].SetActive (false);

					}

					//We start the struct
					listOfAiStructs [i].isActivated = true;
			
				}

				//And update its position
				listOfAiStructs [i] = updateStruct (listOfAiStructs [i]);



			} else {

				//If AI outside of everything, deactivate them
				listOfAiObjects [i].SetActive (false);

				listOfAiStructs [i].isActivated = false;


			}
		}

	}
		
	//Dynamically generate AI on run time
	void changeSize(){

		int target = numAI;

		int current = listOfAiObjects.Length;

		if(target < 0) 
			
			target = 0;

		if (target == current)
			
			return;

		aiStruct[] oldStructs = listOfAiStructs;

		GameObject[] oldObjects = listOfAiObjects;

		listOfAiObjects = new GameObject[target];

		listOfAiStructs = new aiStruct[target];

		if (target < current) {

			//delete stuff

			for (int i = 0; i < target; i++) {

				listOfAiObjects [i] = oldObjects [i];

				listOfAiStructs [i] = oldStructs [i];

			}

			for (int i = target; i < current; i++) {

				Destroy (oldObjects[i]);

			}
				

		} else {

			//create stuff

			for (int i = 0; i < current; i++) {

				listOfAiObjects [i] = oldObjects [i];

				listOfAiStructs [i] = oldStructs [i];

			}

			for (int i = current; i < target; i++) {
				//And start the new AI
				StartAI (i);

			}


		}

	}

	//Start new AI and create a equivalent struct
	void StartAI(int i){

		Vector3 randomPosition = new Vector3 (Random.Range(-terrainDistance,terrainDistance), 1.0f,Random.Range(-terrainDistance,terrainDistance));

		listOfAiObjects[i] = (GameObject)Instantiate (prefabGameObject);

		listOfAiObjects[i].transform.position = randomPosition;

		listOfAiStructs[i] = new aiStruct (randomPosition, Vector3.zero);

	}
		
	//Help method used to check if our AI is insde of the sphere used to update the position
	bool isOnLeichtgewichtRender(Vector3 aiPos){
		
		Vector3 pos = leichtgetwichtKügel.transform.position;

		Vector3 scale = leichtgetwichtKügel.transform.localScale; 

		Vector3 distance = pos - aiPos;

		float dist = distance.magnitude;


		if (dist < scale.x) {
			
			return true;

		}

		return false;
	}


	//For generating new positions for our struct
	aiStruct updateStruct (aiStruct automat) {
		
		RaycastHit hit;

		//If we reached our destination, create new target position
		if ((automat.pos-automat.randomPos).magnitude <= 0.2f|| automat.randomPos == Vector3.zero) {
			
			automat.randomPos = new Vector3 (Random.Range (-terrainDistance, terrainDistance), automat.pos.y, Random.Range (-terrainDistance, terrainDistance));

			//If this created position hits an obstacle, stay where we are
			if (Physics.Raycast (automat.pos, automat.randomPos - automat.pos, out hit, 20, -1)) {
			
				automat.randomPos = automat.pos;

				return automat;

			}

		}

		Vector3 randomPos = automat.randomPos;

		//If poisition inside the terrain
		if (isInside (randomPos, terrainDistance)) {

			Vector3 randir = (randomPos - automat.pos).normalized;

			//Are we looking at the new target position?
			if ((automat.dir - randir).magnitude > 0.05f) {

				//If no, interpolate till we looking at it
				automat.dir = Vector3.Lerp (automat.dir, randir, 0.2f).normalized;


			} else {

				//If yes, move towards that position
				automat.pos += automat.dir * forwardVel * Time.deltaTime; 

			}

		} else {
			
			
			automat.randomPos = automat.pos;

		}
			
		return automat;
	}


	//Helps struct rotate via LERP, checks if we are looking at the new position
	//We round with a precision variable to create a proper interpolation
	public bool isLookingAt(float p, aiStruct automat){
		
		Vector3 randomPos = automat.randomPos;

		Vector3 autDir = Vector3.Normalize(automat.dir);

		autDir = new Vector3 (Mathf.Round (autDir.x* p)/p, Mathf.Round (autDir.y * p)/p, Mathf.Round (autDir.z * p)/p);

		Vector3 direction = Vector3.Normalize (randomPos - automat.pos);

		direction = new Vector3 (Mathf.Round (direction.x * p)/p, Mathf.Round (direction.y * p)/p, Mathf.Round (direction.z * p)/p);

		return (direction == autDir) ;

	}

	//To check if the new point created is outside our terrain
	public bool isInside(Vector3 target, float wall){
		
		return (target.x > -wall) && (target.x < wall) && (target.z > -wall) && (target.z < wall);

	}

	//Method to draw the struct position with triangles
	//using GL calls on the mini-map
	public void OnRenderObject(){
		
		if (fovActivated) {
			
			GL.PushMatrix ();

			GL.Begin (GL.TRIANGLES);

			material.SetPass (0);

			GL.Color (material.color);

			for (int i = 0; i < listOfAiStructs.Length; i++) {

				//Draw only the active ones, that means, the ones inside the sphere but not inside the field of view
				if (listOfAiStructs [i].isActivated) {
					
					Vector3 pos2 = new Vector3 (listOfAiStructs [i].pos.x - listOfAiStructs [i].dir.x*5.0f, 10.0f, listOfAiStructs [i].pos.z - listOfAiStructs [i].dir.z*5.0f);

					Vector3 crossedPos2 = Vector3.Cross (listOfAiStructs[i].dir, new Vector3 (0, 1, 0)).normalized;

					crossedPos2 *= 1.25f;

					GL.Vertex3 (listOfAiStructs [i].pos.x, 10.0f, listOfAiStructs [i].pos.z);

					GL.Vertex3 (pos2.x - crossedPos2.x, 10.0f, pos2.z - crossedPos2.z);

					GL.Vertex3 (pos2.x + crossedPos2.x, 10.0f, pos2.z + crossedPos2.z);

				}

			}
				
			GL.End ();

			GL.PopMatrix ();
		
		}
	
	}

}
