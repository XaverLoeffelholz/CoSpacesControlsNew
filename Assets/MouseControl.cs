using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 ScreenToWorldXY( Vector2 screenPos )
	{
		// Create a ray going into the scene starting 
		// from the screen position provided 
		Ray ray = Camera.main.ScreenPointToRay( screenPos );

		// ray didn't hit any solid object, so return the 
		// intersection point between the ray and 
		// the Y=0 plane (horizontal plane)
		float t = -ray.origin.z / ray.direction.z;
		return ray.GetPoint( t );
	}

	public Vector3 ScreenToWorldXZ( Vector2 screenPos )
	{
		// Create a ray going into the scene starting 
		// from the screen position provided 
		Ray ray = Camera.main.ScreenPointToRay( screenPos );

		// ray didn't hit any solid object, so return the 
		// intersection point between the ray and 
		// the Z=0 plane (horizontal plane)
		float t = -ray.origin.y / ray.direction.y;
		return ray.GetPoint( t );
	}
}
