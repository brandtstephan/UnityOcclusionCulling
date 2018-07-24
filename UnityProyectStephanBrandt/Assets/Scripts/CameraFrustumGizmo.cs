using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFrustumGizmo : MonoBehaviour {
	public Material material;
	public bool isOn = false;

	//Gadget that uses OpenGL calls to draw the Camera Frustum
	void OnRenderObject () {
		if (isOn) {
			Camera cam = Camera.main;

			Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners

			Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners

			Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes (cam); //get planes from matrix

			Plane temp = camPlanes [1];

			camPlanes [1] = camPlanes [2];

			camPlanes [2] = temp; //swap [1] and [2] so the order is better for the loop
 
			for (int i = 0; i < 4; i++) {
				
				nearCorners [i] = Plane3Intersect (camPlanes [4], camPlanes [i], camPlanes [(i + 1) % 4]); //near corners on the created projection matrix

				farCorners [i] = Plane3Intersect (camPlanes [5], camPlanes [i], camPlanes [(i + 1) % 4]); //far corners on the created projection matrix
			
			}

			GL.PushMatrix ();

			GL.Begin (GL.LINES);

			material.SetPass (0);

			for (int i = 0; i < 4; i++) {
				
				//near corners on the created projection matrix
				GL.Color (new Color(1, 0, 0, 1));

				GL.Vertex3 (nearCorners [i].x, nearCorners [i].y, nearCorners [i].z);

				GL.Vertex3 (nearCorners [(i + 1) % 4].x, nearCorners [(i + 1) % 4].y, nearCorners [(i + 1) % 4].z);

				GL.Color (new Color(1, 0, 0, 1));

				//far corners on the created projection matrix
				GL.Vertex3 (farCorners [i].x, farCorners [i].y, farCorners [i].z);

				GL.Vertex3 (farCorners [(i + 1) % 4].x, farCorners [(i + 1) % 4].y, farCorners [(i + 1) % 4].z);

				GL.Color (new Color(1, 0, 0, 1));

				GL.Vertex3 (nearCorners [i].x, nearCorners [i].y, nearCorners [i].z);

				GL.Vertex3 (farCorners [i].x, farCorners [i].y, farCorners [i].z);

			}

			GL.End ();

			GL.PopMatrix ();

		}

    }
 
    Vector3 Plane3Intersect ( Plane p1, Plane p2, Plane p3 ) { //get the intersection point of 3 planes
		
        return ( ( -p1.distance * Vector3.Cross( p2.normal, p3.normal ) ) +
                ( -p2.distance * Vector3.Cross( p3.normal, p1.normal ) ) +
                ( -p3.distance * Vector3.Cross( p1.normal, p2.normal ) ) ) /
            ( Vector3.Dot( p1.normal, Vector3.Cross( p2.normal, p3.normal ) ) );
    }

	//Used to turn on and of on our canvas
	public void turnOn(bool newValue){
		
		this.isOn = newValue;

	}
 
}
