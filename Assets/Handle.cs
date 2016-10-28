using UnityEngine;
using System.Collections;

public class Handle : MonoBehaviour {

	public enum HandleType { translateY, ScaleUniform, ScaleNonUniform, Rotate };

	public HandleType typeOfHandle;

	public bool dragging;
	private float rayDistance;

	Plane dragPlane;
	private Vector3 posOnDragBegin;
	private Vector3 offsetOnDragBegin;

	public Transform connectObject;

	// Use this for initialization
	void Start () {
		connectObject = transform.parent.parent.parent.parent;
	}
	
	// Update is called once per frame
	void Update () {
		if (dragging) {
			
			if (typeOfHandle == HandleType.translateY) {
				
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				// ray to ground plane
				if (dragPlane.Raycast (ray, out rayDistance)) {
					// check distance to initial pos

					// maybe add maximum
					Vector3 newPos = Vector3.Project((ray.GetPoint (rayDistance) + offsetOnDragBegin), connectObject.up);

					connectObject.position = Vector3.ClampMagnitude(newPos, 10f);

					// check y is not below 0
				}		
			}
		}
	}

	public void StartDragging(){
		dragging = true;

		if (typeOfHandle == HandleType.translateY) {
			// create plane 

			dragPlane = new Plane ();

			Vector3 groundPointBelowCam = new Vector3 (Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
			Vector3 groundPointBelowObject = new Vector3 (connectObject.position.x, 0f, connectObject.position.z);

			dragPlane.SetNormalAndPosition (groundPointBelowCam - groundPointBelowObject, transform.position); 

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (dragPlane.Raycast (ray, out rayDistance)) {
				posOnDragBegin = ray.GetPoint(rayDistance);
				offsetOnDragBegin = connectObject.position - posOnDragBegin;
			}

		}

	}

	public void StopDragging(){
		dragging = false;
	}

	public void Focus(){

	}

	public void UnFocus(){

	}

	public void DragY(){

	}

	public void ScaleXYZ(){

	}

	public void Rotate(){

	}

	public void ScaleNonUniform(){

	}
}
