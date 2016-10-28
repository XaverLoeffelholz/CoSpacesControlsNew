using UnityEngine;
using System.Collections;

public class TranslationController : MonoBehaviour {

	public enum Axis
	{
		X,
		Y,
		Z,
		XZ,
		None = 0
	}

	[SerializeField]
	GameObject m_transformTarget;
	[SerializeField]
	Camera m_raycastCamera;
	[SerializeField]
	GameObject m_xAxis;
	[SerializeField]
	GameObject m_yAxisA1;
	[SerializeField]
	GameObject m_yAxisA2;
	[SerializeField]
	GameObject m_yAxisA3;
	[SerializeField]
	GameObject m_yAxisA4;
	[SerializeField]
	GameObject m_zAxis;

	[SerializeField]
	GameObject m_xAxis2;
	[SerializeField]
	GameObject m_zAxis2;

	[SerializeField]
	GameObject m_objectCollider;
	[SerializeField]
	float m_maxRaycastDistance = 1000.0f;

	public bool isDragging = false;
	Axis activeAxis = Axis.None;
	Vector3 lastMousePosition;
	float m_dampening;
	GameObject collider;
	public ObjectManager objManager;

	public MouseControl mouseControl;
	public ScaleController scaleControl;

	public NewBoundingBox newBoundingBox;

	void Awake() {
		//m_dampening = (Screen.dpi / 96) * 10 * 0.5f;
		m_dampening = 1f;
		m_raycastCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		mouseControl = GameObject.Find ("MouseControl").GetComponent<MouseControl>();
	}

	void Update () {
		Ray ray = m_raycastCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (collider) {
			if (!isDragging && ((collider == m_xAxis || collider == m_yAxisA1 || collider == m_yAxisA2 || collider == m_yAxisA3 || collider == m_yAxisA4 || collider == m_zAxis || collider == m_xAxis2 || collider == m_zAxis2))) {
				collider.transform.GetChild (1).gameObject.SetActive (false);
			}
		}


		if (!scaleControl.isDragging && !isDragging && Physics.Raycast(ray, out hit, m_maxRaycastDistance)) {
			collider = hit.collider.gameObject;

			if (!objManager.IsAnyObjectDragging() && (collider == m_xAxis || collider == m_yAxisA1 || collider == m_yAxisA2 || collider == m_yAxisA3 || collider == m_yAxisA4 || collider == m_zAxis  || collider == m_xAxis2 || collider == m_zAxis2)) {
				collider.transform.GetChild (1).gameObject.SetActive (true);
			}

			if (!isDragging && m_raycastCamera && Input.GetMouseButtonDown(0)) {

				if (collider == m_xAxis || collider == m_xAxis2) {
					objManager.HideHandles ();
					collider.SetActive (true);
					activeAxis = Axis.X;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
				}
				else if (collider == m_yAxisA1 || collider == m_yAxisA2 ||collider == m_yAxisA3 ||collider == m_yAxisA4) {
					objManager.HideHandles ();
					collider.SetActive (true);
					activeAxis = Axis.Y;
					lastMousePosition = mouseControl.ScreenToWorldXY(Input.mousePosition);
				}
				else if (collider == m_zAxis || collider == m_zAxis2) {
					objManager.HideHandles ();
					collider.SetActive (true);
					activeAxis = Axis.Z;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
				}
				else if (m_objectCollider && collider == m_objectCollider) {
					objManager.HideHandles ();
					activeAxis = Axis.XZ;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
				}
				else {
					return;
				}

				isDragging = true;

			}
		}

		if (isDragging && Input.GetMouseButton(0)) {

			Vector3 delta;

			switch(activeAxis) {
				case Axis.X:
					delta = mouseControl.ScreenToWorldXZ (Input.mousePosition) - lastMousePosition;
					
					delta = RasterManager.Instance.Raster (new Vector3 (-delta.x, 0, 0));
					if (delta.magnitude > 0.0f) {
						lastMousePosition = mouseControl.ScreenToWorldXZ (Input.mousePosition);
					}
					break;
				case Axis.Y:
					delta = mouseControl.ScreenToWorldXY (Input.mousePosition) - lastMousePosition;
						
					delta = RasterManager.Instance.Raster (new Vector3 (0, delta.y, 0));
					if (delta.magnitude > 0.0f) {
						lastMousePosition = mouseControl.ScreenToWorldXY (Input.mousePosition);
					}
					break;
				case Axis.Z:
					delta = mouseControl.ScreenToWorldXZ (Input.mousePosition) - lastMousePosition;
					
					delta = RasterManager.Instance.Raster (new Vector3 (0, 0, -delta.z));
					if (delta.magnitude > 0.0f) {
						lastMousePosition = mouseControl.ScreenToWorldXZ (Input.mousePosition);	
					}
					break;
				case Axis.XZ:
					delta = mouseControl.ScreenToWorldXZ(Input.mousePosition) - lastMousePosition;					
					delta = RasterManager.Instance.Raster(new Vector3(delta.x, 0, delta.z));
					if (delta.magnitude > 0.0f) {
						lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);	
					}
					break;

				default:
					return;
			}

			if (activeAxis != Axis.XZ) {
				m_transformTarget.transform.Translate(delta / (m_dampening >= 1.0f ? m_dampening : 1.0f));
				//m_transformTarget.transform.Translate(delta);
			} else {
				if (!newBoundingBox.localCoordinates) {
					m_transformTarget.transform.Translate(delta / (m_dampening >= 1.0f ? m_dampening : 1.0f), Space.World);
				} else {
					m_transformTarget.transform.Translate(delta / (m_dampening >= 1.0f ? m_dampening : 1.0f), Space.Self);
				}

				//m_transformTarget.transform.Translate(delta, Space.World);

			}

			if (m_transformTarget.transform.position.y < 0.33f) {
				m_transformTarget.transform.position = new Vector3 (m_transformTarget.transform.position.x, 0.33f, m_transformTarget.transform.position.z);
			}

			newBoundingBox.DrawBoundingBox ();
		}

		if (Input.GetMouseButtonUp(0)) {
			objManager.ShowHandles ();

			isDragging = false;
			activeAxis = Axis.None;

			if ((collider == m_xAxis || collider == m_yAxisA1 || collider == m_yAxisA2 ||collider == m_yAxisA3 ||collider == m_yAxisA4 ||collider == m_zAxis)) {
				collider.transform.GetChild (1).gameObject.SetActive (false);
			}
		}
	}

	public void HideTranslationHandles(){
		m_xAxis.SetActive (false);
		m_yAxisA1.SetActive (false);
		m_yAxisA2.SetActive (false);
		m_yAxisA3.SetActive (false);
		m_yAxisA4.SetActive (false);
		m_zAxis.SetActive (false);
		m_xAxis2.SetActive (false);
		m_zAxis2.SetActive (false);

	}

	public void ShowTranslationHandles(){
		RepositionHandles ();
		m_xAxis.SetActive (true);
		m_yAxisA1.SetActive (true);
		m_yAxisA2.SetActive (true);
		m_yAxisA3.SetActive (true);
		m_yAxisA4.SetActive (true);
		m_zAxis.SetActive (true);
		m_xAxis2.SetActive (true);
		m_zAxis2.SetActive (true);
	}

	public void RepositionHandles(){

		// always position handles based on bounding box

		m_zAxis.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [5] + 0.5f * newBoundingBox.coordinatesBoundingBox [6];
		m_zAxis2.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [7] + 0.5f * newBoundingBox.coordinatesBoundingBox [4];
				
		/*
		m_yAxisA1.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [0] + 0.5f * newBoundingBox.coordinatesBoundingBox [1];
		m_yAxisA2.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [1] + 0.5f * newBoundingBox.coordinatesBoundingBox [2]; 
		m_yAxisA3.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [2] + 0.5f * newBoundingBox.coordinatesBoundingBox [3];
		m_yAxisA4.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [3] + 0.5f * newBoundingBox.coordinatesBoundingBox [0];
		*/

		m_yAxisA1.transform.position = newBoundingBox.coordinatesBoundingBox [0];
		m_yAxisA2.transform.position = newBoundingBox.coordinatesBoundingBox [1]; 
		m_yAxisA3.transform.position = newBoundingBox.coordinatesBoundingBox [2];
		m_yAxisA4.transform.position = newBoundingBox.coordinatesBoundingBox [3];

		m_xAxis.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [6] + 0.5f * newBoundingBox.coordinatesBoundingBox [7];
		m_xAxis2.transform.position = 0.5f * newBoundingBox.coordinatesBoundingBox [4] + 0.5f * newBoundingBox.coordinatesBoundingBox [5];

		/*
		if (newBoundingBox.localCoordinates) {
			m_transformTarget.transform.localRotation = m_objectCollider.transform.parent.parent.transform.localRotation;
			m_objectCollider.transform.parent.parent.transform.localRotation = Quaternion.Euler (Vector3.zero);
		} else {
			m_objectCollider.transform.parent.parent.transform.localRotation = m_transformTarget.transform.localRotation;
			m_transformTarget.transform.localRotation = Quaternion.Euler (Vector3.zero);
		}*/
	}
}
