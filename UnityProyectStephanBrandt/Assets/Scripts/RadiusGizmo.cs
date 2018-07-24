using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusGizmo : MonoBehaviour {
	
	public Material material;
	public GameObject renderSphere;
	public bool isOn = false;

	//Used to draw the sphere that delimitates the rendering states
	//OpenGL calls.
	void OnRenderObject(){
		if(isOn){
		
		float radius = renderSphere.transform.localScale.x+2.5f;
		float angle = 20.0f;

		GL.PushMatrix ();
		GL.Begin (GL.LINES);

		material.SetPass (0);
		GL.Color (material.color);

		for (int i = 0; i <= 1000; i++) {
			float x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			float z = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

			GL.Vertex3 (x + transform.position.x, 1.0f, transform.position.z + z);
			angle += (360.0f / 300);
		}

		GL.End ();
		GL.PopMatrix ();
		}
	}
}
