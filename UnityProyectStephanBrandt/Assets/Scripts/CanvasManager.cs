using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CanvasManager : MonoBehaviour {
	
	public GameObject aiManagerObject;

	public InputField numAiField;

	public Camera mainCamera;

	public GameObject player;

	private AIController aiController;

	private AIManager aiManager;

	private CameraFrustumGizmo gizmoFOV;

	private RadiusGizmo gizmoSphere;

	// Use this for initialization
	void Start () {
		
		if (aiManagerObject) {
			
			aiManager = aiManagerObject.GetComponent<AIManager> ();

		} else {
			
			Debug.Log ("Canvas missing AIManager game object.");

		}

		if (numAiField == null) {
			
			Debug.Log ("Input Field missing.");

		}

		if (player) {
			
			gizmoSphere = player.GetComponent<RadiusGizmo> ();

		} else {
			
			Debug.Log ("Canvas missing player.");

		}

		if (mainCamera) {
			
			gizmoFOV = mainCamera.GetComponent<CameraFrustumGizmo> ();

		} else {
			
			Debug.Log ("Canvas missing camera.");

		}

	}

	//Manage generation of AI through canvas
	public void setNumAi(string text){
		
		if (!text.Equals("")) {
		
			int numberAI = int.Parse (text);

			if (numberAI > aiManager.amountAllowed) {
			
				numberAI = aiManager.amountAllowed;

			}

			aiManager.numAI = numberAI;

		}

	}
		
	//Manage toggle of field of view gizmo through canvas
	public void fovToggle(bool newValue){
		
		gizmoFOV.turnOn (newValue);

	}

	//Manage toggle of sphere gizmo through canvas
	public void sphereToggle(bool newValue){

		gizmoSphere.isOn = newValue;

	}

	//Manage toggle of the structs drawn as triangles on the mini-map through canvas
	public void structToggle(bool newValue){

		aiManager.fovActivated = newValue;

	}

}
