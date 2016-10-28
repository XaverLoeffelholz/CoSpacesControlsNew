using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour {

	public Camera mainCamera;

	public ObjectScript currentFocusedObject;
	public ObjectScript currentSelectedObject;

	public Handle currentFocusedHandle;
	public Handle currentDraggedHandle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000.0f)) {
			if (hit.collider.transform.CompareTag ("Object")) {
				currentFocusedObject = hit.collider.transform.parent.parent.GetComponent<ObjectScript> ();
				currentFocusedObject.Focus ();
			} else if (hit.collider.transform.CompareTag ("YTranslation")) {
				currentFocusedHandle = hit.collider.transform.parent.GetComponent<Handle> ();
				currentFocusedHandle.Focus ();
			} else if (hit.collider.transform.CompareTag ("UniformScaling")) {
				currentFocusedHandle = hit.collider.transform.parent.GetComponent<Handle> ();
				currentFocusedHandle.Focus ();
			} else {
				if (currentFocusedObject != null) {
					currentFocusedObject.UnFocus ();
					currentFocusedObject = null;
				}
			}
		} else {
			if (currentFocusedObject != null) {
				currentFocusedObject.UnFocus ();
				currentFocusedObject = null;
			}

			if (currentFocusedHandle != null) {
				currentFocusedHandle.UnFocus ();
				currentFocusedHandle = null;
			}
		}

		// other colliders for handles
		if (Input.GetMouseButtonDown (0)) {
			if (currentFocusedHandle != null) {
				
				currentDraggedHandle = currentFocusedHandle;
				currentDraggedHandle.StartDragging ();

			} else if (currentFocusedObject != null) {
				
				if (currentSelectedObject != null) {
					if (currentSelectedObject != currentFocusedObject) {
						currentSelectedObject.DeSelect ();
						currentSelectedObject = null;
					}
				}
				currentFocusedObject.Select ();
				currentSelectedObject = currentFocusedObject;
				currentSelectedObject.StartDragging (ray);

			} else {				
				if (currentSelectedObject != null && !currentSelectedObject.dragging && currentFocusedHandle == null) {
					currentSelectedObject.DeSelect ();
					currentSelectedObject = null;
				}
			}

		} 

		if (Input.GetMouseButtonUp (0)){
			if (currentSelectedObject != null) {
				currentSelectedObject.StopDragging ();

			}

			if (currentDraggedHandle != null){
				currentDraggedHandle.StopDragging ();
				currentDraggedHandle = null;
			}
		}

	}
}
