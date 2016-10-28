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
	public Vector3 InitialTopToBottom;

	public ObjectScript connectObject;

	private Vector3 scaleOneBeginDrag;

	// Use this for initialization
	void Start () {
		connectObject = transform.parent.parent.parent.parent.GetComponent<ObjectScript> ();
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
					Vector3 newPos = Vector3.Project((ray.GetPoint (rayDistance) + offsetOnDragBegin), connectObject.transform.position + connectObject.transform.up);

					connectObject.transform.position = Vector3.ClampMagnitude(RasterManager.Instance.Raster(newPos), 10f);

					// check y is not below 0
				}		
			} else if (typeOfHandle == HandleType.ScaleUniform) {
				
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				// ray to ground plane
				if (dragPlane.Raycast (ray, out rayDistance)) {
					Vector3 newPos = Vector3.Project ((ray.GetPoint (rayDistance) + offsetOnDragBegin), connectObject.transform.position + connectObject.transform.up);
					newPos = Vector3.ClampMagnitude (RasterManager.Instance.Raster (newPos), 10f);

					// get new scale Top to Bottom
					Vector3 newTopToBottom = newPos - connectObject.boundingBox.GetBottomCenter();

					// scale accordingly
					Vector3 newScale = scaleOneBeginDrag * (newTopToBottom.magnitude/InitialTopToBottom.magnitude);

					connectObject.cube.transform.parent.localScale = new Vector3 (Mathf.Max (0.2f, newScale.x), Mathf.Max (0.2f, newScale.y), Mathf.Max (0.2f, newScale.z));
					connectObject.boundingBox.CalculateBoundingBox();

					// use update bb from master thesis
					connectObject.boundingBox.DrawBoundingBox();
					connectObject.PlaceHandles ();
				}

			}

		}
	}

	public void StartDragging(){
		
		if (typeOfHandle == HandleType.translateY || typeOfHandle == HandleType.ScaleUniform) {
			// create plane 

			dragPlane = new Plane ();

			Vector3 groundPointBelowCam = new Vector3 (Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
			Vector3 groundPointBelowObject = new Vector3 (connectObject.transform.position.x, 0f, connectObject.transform.position.z);

			dragPlane.SetNormalAndPosition (groundPointBelowCam - groundPointBelowObject, transform.position); 

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (dragPlane.Raycast (ray, out rayDistance)) {
				posOnDragBegin = ray.GetPoint(rayDistance);

				if (typeOfHandle == HandleType.translateY) {
					offsetOnDragBegin = connectObject.transform.position - posOnDragBegin;
				} else if (typeOfHandle == HandleType.ScaleUniform){
					offsetOnDragBegin = connectObject.boundingBox.GetTopCenter() - posOnDragBegin;
					scaleOneBeginDrag = connectObject.cube.transform.parent.localScale;

					// get current scale Top to bottom
					InitialTopToBottom = connectObject.boundingBox.GetTopCenter() - connectObject.boundingBox.GetBottomCenter();
				}

			}

			dragging = true;
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
