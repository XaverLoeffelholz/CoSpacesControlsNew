using UnityEngine;
using System.Collections;

public class ObjectScript : MonoBehaviour {

	public bool focused;
	public bool selected;
	public bool dragging;
	private float rayDistance;

	private Vector3 posOnDragBegin;
	private Vector3 offsetOnDragBegin;

	private Plane groundPlane;

	public NewBoundingBox boundingBox;

	public GameObject Handles;

	public GameObject UniformScalingHandle;
	public GameObject YAxisHandle;

	public GameObject NonUniformYScalingHandle;
	public GameObject NonUniformXScalingHandleX1;
	public GameObject NonUniformXScalingHandleX2;
	public GameObject NonUniformZScalingHandleZ1;
	public GameObject NonUniformZScalingHandleZ2;

	public GameObject XRotationHandle;
	public GameObject YRotationHandle;
	public GameObject ZRotationHandle;

	public GameObject topHandles;


	public bool local = false;

	// Use this for initialization
	void Start () {
		groundPlane = new Plane ();
		groundPlane.normal = Vector3.up;
		groundPlane.distance = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (1)) {
			Handles.SetActive (false);
		}

		if (Input.GetMouseButtonUp (1) && selected) {
			Handles.SetActive (true);
		}

		if (dragging) {

			Handles.SetActive (false);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// ray to ground plane
			if (groundPlane.Raycast (ray, out rayDistance)) {
				// check distance to initial pos

				// maybe add maximum
				Vector3 newPos = RasterManager.Instance.Raster(ray.GetPoint (rayDistance) + offsetOnDragBegin);
				transform.position = Vector3.ClampMagnitude(newPos, 10f);
			}		
		}

		if (selected && !dragging) {
			PlaceRotationControls ();
			RescaleHandles ();
		}

	}

	public void Focus(){
		if (!focused) {
			focused = true;
			// add focus effect
		}
	}

	public void UnFocus(){
		if (focused) {
			focused = false;
		}
	}

	public void Select(){
		if (!selected) {
			selected = true;
			Handles.SetActive (true);
			boundingBox.DrawBoundingBox ();
			PlaceHandles ();
		}
	}

	public void DeSelect(){
		if (selected) {
			selected = false;
			Handles.SetActive (false);
		}
	}

	public void StartDragging(Ray ray){
		dragging = true;

		if (groundPlane.Raycast (ray, out rayDistance)) {
			posOnDragBegin = ray.GetPoint(rayDistance);
			offsetOnDragBegin = transform.position - posOnDragBegin;
		}
 
	}

	public void StopDragging(){
		dragging = false;
		Handles.SetActive (true);
	}


	public void PlaceHandles(){
		// Top center Uniform Scaling, plus Y hande plus hight
		Vector3 topCenterBB = boundingBox.GetTopCenter();

		float distanceToCamera = (Camera.main.transform.position - transform.position).magnitude;
		//Vector3 size = Vector3.one * distanceToCamera * 0.12f;

		NonUniformYScalingHandle.transform.position = topCenterBB;
		topHandles.transform.position = topCenterBB;

		NonUniformXScalingHandleX1.transform.position = boundingBox.CenterPosXDirection(local);
		NonUniformXScalingHandleX2.transform.position = boundingBox.CenterNegXDirection(local);

		NonUniformZScalingHandleZ1.transform.position = boundingBox.CenterPosZDirection(local);
		NonUniformZScalingHandleZ2.transform.position = boundingBox.CenterNegZDirection(local);

		Vector3 bbCenter = boundingBox.GetCenter ();

		NonUniformXScalingHandleX1.transform.rotation = Quaternion.LookRotation (NonUniformXScalingHandleX1.transform.position - bbCenter);
		NonUniformXScalingHandleX2.transform.rotation = Quaternion.LookRotation (NonUniformXScalingHandleX2.transform.position - bbCenter);
		NonUniformZScalingHandleZ1.transform.rotation = Quaternion.LookRotation (NonUniformZScalingHandleZ1.transform.position - bbCenter);
		NonUniformZScalingHandleZ2.transform.rotation = Quaternion.LookRotation (NonUniformZScalingHandleZ2.transform.position - bbCenter);
		NonUniformYScalingHandle.transform.rotation = Quaternion.LookRotation (NonUniformYScalingHandle.transform.position - bbCenter);

		PlaceRotationControls ();
	}

	public void PlaceRotationControls(){
		// get closest corner of Bounding box
		boundingBox.CalculateBoundingBox();

		int idOfClosestVertToCamera = boundingBox.closestCorner(Camera.main.transform.position);

		int nextId = idOfClosestVertToCamera + 1;

		if (nextId > 3) {
			nextId = 0;
		}

		int preId = idOfClosestVertToCamera - 1;

		if (preId < 0) {
			preId = 3;
		}

		Debug.Log ("Id of closesst" + idOfClosestVertToCamera);
		int idOfCornerOnGround = idOfClosestVertToCamera + 4;

		// get next, get previous and get the one at bottom

		Vector3 direction = boundingBox.coordinatesBoundingBox [preId] - boundingBox.coordinatesBoundingBox [idOfClosestVertToCamera];

		if (Mathf.Abs (Vector3.Dot (direction, Vector3.forward)) == 1f) {
			ZRotationHandle.transform.position = boundingBox.coordinatesBoundingBox [preId];
			XRotationHandle.transform.position = boundingBox.coordinatesBoundingBox [nextId];
		} else {
			XRotationHandle.transform.position = boundingBox.coordinatesBoundingBox [preId];
			ZRotationHandle.transform.position = boundingBox.coordinatesBoundingBox [nextId];
		}

		YRotationHandle.transform.position = boundingBox.coordinatesBoundingBox [idOfCornerOnGround];

		Vector3 bbCenter = boundingBox.GetCenter ();

		// RotateHandles 
		XRotationHandle.transform.rotation = Quaternion.LookRotation (new Vector3(1f,0f,0f), XRotationHandle.transform.position - bbCenter);
		YRotationHandle.transform.rotation = Quaternion.LookRotation (new Vector3(0f,1f,0f), YRotationHandle.transform.position - bbCenter);
		ZRotationHandle.transform.rotation = Quaternion.LookRotation (new Vector3(0f,0f,1f), ZRotationHandle.transform.position - bbCenter);
	}

	public void RescaleHandles (){

		float distanceToCamera = (Camera.main.transform.position - transform.position).magnitude;
		Vector3 size = Vector3.one * distanceToCamera * 0.12f;

		XRotationHandle.transform.localScale = size;
		YRotationHandle.transform.localScale = size;
		ZRotationHandle.transform.localScale = size;

		NonUniformXScalingHandleX1.transform.localScale = size;
		NonUniformXScalingHandleX2.transform.localScale = size;
		NonUniformZScalingHandleZ1.transform.localScale = size;
		NonUniformZScalingHandleZ2.transform.localScale = size;
		NonUniformYScalingHandle.transform.localScale = size;

		topHandles.transform.localScale = size;
	}


}
