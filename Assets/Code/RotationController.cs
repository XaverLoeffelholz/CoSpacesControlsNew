using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour {

	public enum Axis
	{
		X,
		Y,
		Z,
		None = 0
	}

	[SerializeField]
	GameObject m_transformTarget;
	[SerializeField]
	Camera m_raycastCamera;
	[SerializeField]
	GameObject m_BoundingBox;
	[SerializeField]
	float m_maxRaycastDistance = 1000.0f;


	[SerializeField]
	GameObject m_xAxis0;
	[SerializeField]
	GameObject m_xAxis1;

	[SerializeField]
	GameObject m_yAxis0;
	[SerializeField]
	GameObject m_yAxis1;
	[SerializeField]
	GameObject m_yAxis2;
	[SerializeField]
	GameObject m_yAxis3;

	[SerializeField]
	GameObject m_zAxis0;
	[SerializeField]
	GameObject m_zAxis1;


	public bool isDragging = false;
	Axis activeAxis = Axis.None;
	Vector3 lastMousePosition;
	float m_dampening;
	GameObject collider;
	public ObjectManager objManager;

	public Corners corners;

	public MouseControl mouseControl;
	public NewBoundingBox newBoundingBox;


	GameObject parentGO; 
	float prevRotationAmountX;
	float prevRotationAmountY;
	float prevRotationAmountZ;
	private bool newRotation = false;
	private GameObject currentHandle;
	float newRotationAmount;
	private bool resetLastPosition = true;
	private Vector3 directionHandle;
	private Vector3 lastPosition;

	void Awake() {
		m_dampening = 1f;
		m_raycastCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		mouseControl = GameObject.Find ("MouseControl").GetComponent<MouseControl>();
	}

	private float CalculateInputFromPoint(Vector3 pointOfCollision, Vector3 pos1, Vector3 pos2)
	{
		if (resetLastPosition)
		{
			directionHandle = Vector3.Normalize(pos2 - pos1);
		}

		Vector3 pq = pointOfCollision - pos1;
		Vector3 newPoint = pos1 + (directionHandle * (Vector3.Dot(pq, directionHandle) / directionHandle.sqrMagnitude));

		if (resetLastPosition)
		{
			resetLastPosition = false;
			lastPosition = newPoint;
		}

		float input = (newPoint - lastPosition).magnitude;

		// check direction of vector:
		if (Vector3.Dot((newPoint - lastPosition), directionHandle) < 0f)
		{
			input = input * (-1f);
		} 

		return input;

	}



	void Update () {
		Ray ray = m_raycastCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (collider) {
			if (!isDragging && (collider.CompareTag ("xRotation") || collider.CompareTag ("yRotation") || collider.CompareTag ("zRotation"))) {
				collider.transform.parent.GetChild (1).gameObject.SetActive (false);
			}
		}

		if (!isDragging && Physics.Raycast(ray, out hit, m_maxRaycastDistance)) {
			collider = hit.collider.gameObject;

			if (!objManager.IsAnyObjectDragging() &&  (collider.CompareTag ("xRotation") || collider.CompareTag ("yRotation") || collider.CompareTag ("zRotation"))) {
				collider.transform.parent.GetChild (1).gameObject.SetActive (true);
				currentHandle = collider;
			}

			if (!isDragging && m_raycastCamera && Input.GetMouseButtonDown(0)) {
				if (m_BoundingBox && collider.CompareTag("xRotation") && collider.transform.parent.parent == m_BoundingBox.transform) {
					activeAxis = Axis.X;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
				}
				else if (m_BoundingBox && collider.CompareTag("yRotation") && collider.transform.parent.parent == m_BoundingBox.transform) {
					activeAxis = Axis.Y;
					lastMousePosition = mouseControl.ScreenToWorldXY(Input.mousePosition);
				}
				else if (m_BoundingBox && collider.CompareTag("zRotation") && collider.transform.parent.parent == m_BoundingBox.transform) {
					activeAxis = Axis.Z;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
				}
				else {
					return;
				}

				objManager.HideHandles ();
				ShowRotationHandles ();

				parentGO = new GameObject();
				parentGO.transform.SetParent (transform.parent);
				parentGO.transform.localPosition = new Vector3(0f, 0f, 0f);
				parentGO.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

				m_transformTarget.transform.SetParent(parentGO.transform);

				isDragging = true;
				newRotation = true;
			}			
		}

		if (isDragging && Input.GetMouseButton(0)) {

			Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (m_raycastCamera.transform.position - m_transformTarget.transform.position).magnitude));

			// get vector from handle to center of object
			Vector3 HandleToCenter = currentHandle.transform.position - parentGO.transform.position;

			// get vector form direction of handle
			Vector3 handleDirection = currentHandle.transform.up;

			// cross product
			Vector3 crossProduct = Vector3.Cross(HandleToCenter, handleDirection) * (-1f);

			//newRotationAmount = 15f * HandleUtility.PointOnLineParameter(p, currentHandle.transform.position, crossProduct);
	
			newRotationAmount = 15f * CalculateInputFromPoint (p, currentHandle.transform.position, currentHandle.transform.position + crossProduct);

			switch(activeAxis) {
			case Axis.X:
				if (newRotation)
				{
					prevRotationAmountX = newRotationAmount;
					newRotation = false;
				}

				parentGO.transform.Rotate(new Vector3(newRotationAmount-prevRotationAmountX, 0f, 0f));
				prevRotationAmountX = newRotationAmount;

				break;

			case Axis.Y:
				if (newRotation)
				{
					prevRotationAmountY = newRotationAmount;
					newRotation = false;
				}

				parentGO.transform.Rotate(new Vector3(0f, newRotationAmount-prevRotationAmountY, 0f));
				prevRotationAmountY = newRotationAmount;

				break;

			case Axis.Z:
				if (newRotation)
				{
					prevRotationAmountZ = -newRotationAmount;
					newRotation = false;
				}

				parentGO.transform.Rotate(new Vector3(0f, 0f, -newRotationAmount-prevRotationAmountZ));
				prevRotationAmountZ = -newRotationAmount;
				break;

			default:
				return;
			}
						
			// m_transformTarget.transform.rotation = m_transformTarget.transform.rotation * Quaternion.Euler(delta / (m_dampening >= 1.0f ? m_dampening : 1.0f));
			//newBoundingBox.DeleteBoundingBox ();
			newBoundingBox.CalculateBoundingBox ();
			newBoundingBox.DrawBoundingBox ();
		}

		if (Input.GetMouseButtonUp(0)) {

			m_transformTarget.transform.SetParent(transform.parent);
			GameObject.Destroy(parentGO);

			objManager.ShowHandles ();
			isDragging = false;
			activeAxis = Axis.None;

			if ((collider.CompareTag ("xRotation") || collider.CompareTag ("yRotation") || collider.CompareTag ("zRotation"))) {
				collider.transform.parent.GetChild (1).gameObject.SetActive (false);
			}
		}
	}


	public void HideRotationHandles(){
		m_BoundingBox.SetActive (false);
	}

	public void ShowRotationHandles(){
		RepositionHandles ();

		corners.UpdateRotationHandles ();
		m_BoundingBox.SetActive (true);
	}

	public void RepositionHandles(){

		// always position handles based on bounding box

		m_xAxis1.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [0] + 0.5f * newBoundingBox.coordinatesBoundingBox [3];
		m_xAxis0.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [2] + 0.5f * newBoundingBox.coordinatesBoundingBox [1];

		m_yAxis1.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [0] + 0.5f * newBoundingBox.coordinatesBoundingBox [4];
		m_yAxis0.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [1] + 0.5f * newBoundingBox.coordinatesBoundingBox [5]; 
		m_yAxis3.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [2] + 0.5f * newBoundingBox.coordinatesBoundingBox [6];
		m_yAxis2.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [3] + 0.5f * newBoundingBox.coordinatesBoundingBox [7];

		m_zAxis1.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [2] + 0.5f * newBoundingBox.coordinatesBoundingBox [3];
		m_zAxis0.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [0] + 0.5f * newBoundingBox.coordinatesBoundingBox [1];
       
		/*
		if (newBoundingBox.localCoordinates) {
			transform.parent.localRotation = m_transformTarget.transform.localRotation;
			m_transformTarget.transform.localRotation = Quaternion.Euler (Vector3.zero);
		}else {
			m_transformTarget.transform.localRotation = transform.parent.localRotation;
			transform.localRotation = Quaternion.Euler (Vector3.zero);
		}*/
	}
}
