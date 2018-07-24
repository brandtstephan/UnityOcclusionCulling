using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
	
	public float speed = 5.0f;

	public Vector3 randomPos;

	public NavMeshAgent navMeshAgent;

	private Vector3 currentAi = Vector3.zero;

	private float terrainDistance;

	// Use this for initialization
	void Start () {
		
		terrainDistance = GameObject.FindGameObjectWithTag ("Terrain").transform.localScale.x;

		if (GetComponent<NavMeshAgent> ()) {
			
			navMeshAgent.speed = speed;

		} else {
			
			Debug.Log ("Missing a nav mesh agent");

		}

		randomPos = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(currentAi != Vector3.zero){
			
			navMeshAgent.SetDestination (currentAi);

			currentAi = Vector3.zero;
		}
			Walk ();
			//If the ai position hasn't changed much in the last second
	}

	public void setValue(Vector3 currentAi){
		
		this.currentAi = currentAi;

	}

	void Walk(){

		//We check if we are still on the same position or we haven't created a new position
		if ((transform.position - randomPos).magnitude <= 1.0f || randomPos == Vector3.zero) {
			
			randomPos = RandomPos (transform.position, Random.Range(-terrainDistance/2,terrainDistance/2), -1);

			navMeshAgent.SetDestination (randomPos);
		}

	}

	Vector3 RandomPos(Vector3 origin, float dist, int LayerMask){

		NavMeshHit navHit;

		Vector3 ranDir;

		//We create a random position inside a sphere of dist radius
		ranDir = Random.insideUnitSphere*dist;

		//We sum it to our original position
		ranDir += origin;

		//We set Y to the current position on Y of our AI to avoid creating points
		//That are not inside our NavMesh and therefor not walkable
		ranDir.y = 1.0f;

		//We sample this position to check if its a allowed position inside our NavMesh
		//We check on all the layers (-1)
		Vector3 finalPosition = transform.position;

		if (NavMesh.SamplePosition (ranDir, out navHit, dist, LayerMask)) {
			
			finalPosition = navHit.position;

		}

		//We return this new random position
		return finalPosition;
	}
}
