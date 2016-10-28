using UnityEngine;
using System.Collections;

public class ScaleController : MonoBehaviour {

	public enum Axis
	{
		X,
		Y,
		Z,
		All,
		None = 0
	}

	[SerializeField]
	GameObject m_transformTarget;
	[SerializeField]
	Camera m_raycastCamera;
	[SerializeField]
	GameObject m_xAxis;
	[SerializeField]
	GameObject m_yAxis;
	[SerializeField]
	GameObject m_zAxis;

	[SerializeField]
	GameObject m_xAxis2;
	[SerializeField]
	GameObject m_yAxis2;
	[SerializeField]
	GameObject m_zAxis2;

	[SerializeField]
	GameObject m_allAxis;
	[SerializeField]
	float m_maxRaycastDistance = 1000.0f;

	public Transform ScalingAnchorX;
	public Transform ScalingAnchorY;
	public Transform ScalingAnchorZ;
	public Transform ObjectParent;
	GameObject collider;

	public GameObject ToggleUniformScaling;

	public bool isDragging = false;
	Axis activeAxis = Axis.None;
	Vector3 lastMousePosition;
	Vector3 initialScale;
	Vector3 centerPoint;
	float m_dampening;
	int layerMask;

	public MouseControl mouseControl;
	public Transform boundingBox;
	public GameObject lines;
	public GameObject lockIcon;
	public GameObject lockIconUnlocked;
	public ObjectManager objManager;

	public bool nonUniformScaling = false;
	private Color startColorLockIcon;
	private bool hoveringUniformScaling = false;

	public NewBoundingBox newBoundingBox;

	void Awake() {
		m_dampening = (Screen.dpi / 96) * 100;
		m_raycastCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		mouseControl = GameObject.Find ("MouseControl").GetComponent<MouseControl>();
		startColorLockIcon = lockIcon.GetComponent<Renderer> ().material.color;

		layerMask = ~(1 << 8);
	}


	void Update () {

		Ray ray = m_raycastCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (collider) {
			if (!isDragging && ((collider == m_xAxis || collider == m_yAxis || collider == m_zAxis || collider == m_xAxis2 || collider == m_yAxis2 || collider == m_zAxis2))) {
				if (!nonUniformScaling) {
					m_xAxis.transform.GetChild (1).gameObject.SetActive (false);
					m_yAxis.transform.GetChild (1).gameObject.SetActive (false);
					m_zAxis.transform.GetChild (1).gameObject.SetActive (false);
					if (!nonUniformScaling) {
						lines.transform.GetChild (1).gameObject.SetActive (false);
					}
				} else {
					collider.transform.GetChild (1).gameObject.SetActive (false);
				}
			}

			if (!objManager.IsAnyObjectDragging () && collider == ToggleUniformScaling) {
				UnHoverUniformScalingToggle ();
			}
		}


		if (!isDragging && Physics.Raycast(ray, out hit, m_maxRaycastDistance, layerMask)) {
			collider = hit.collider.gameObject;

			if (!objManager.IsAnyObjectDragging() && (collider == m_xAxis || collider == m_yAxis || collider == m_zAxis || collider == m_xAxis2 || collider == m_yAxis2 || collider == m_zAxis2)) {
				if (!nonUniformScaling) {
					m_xAxis.transform.GetChild (1).gameObject.SetActive (true);
					m_yAxis.transform.GetChild (1).gameObject.SetActive (true);
					m_zAxis.transform.GetChild (1).gameObject.SetActive (true);
					if (!nonUniformScaling) {
						lines.transform.GetChild (1).gameObject.SetActive (true);
					}
				} else {
					collider.transform.GetChild (1).gameObject.SetActive (true);
				}
			}

			if (!objManager.IsAnyObjectDragging () && collider == ToggleUniformScaling) {
				HoverUniformScalingToggle ();
			}

			if (!isDragging && m_raycastCamera && Input.GetMouseButtonDown(0)) {

				if (collider == m_xAxis || collider == m_xAxis2) {
					activeAxis = Axis.X;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
					initialScale = ScalingAnchorX.localScale;
				}
				else if (collider == m_yAxis || collider == m_yAxis2) {
					activeAxis = Axis.Y;
					lastMousePosition = mouseControl.ScreenToWorldXY(Input.mousePosition);
					initialScale = ScalingAnchorY.localScale;
				}
				else if (collider == m_zAxis || collider == m_zAxis2) {
					activeAxis = Axis.Z;
					lastMousePosition = mouseControl.ScreenToWorldXZ(Input.mousePosition);
					initialScale = ScalingAnchorZ.localScale;
				}
				else if (m_allAxis && collider == m_allAxis) {
					activeAxis = Axis.All;
					lastMousePosition = mouseControl.ScreenToWorldXY(Input.mousePosition);
					initialScale = ScalingAnchorY.localScale;
				} else if(ToggleUniformScaling && collider == ToggleUniformScaling){
					ToggleScalingMode ();
					return;
				}
					
				else {
					return;
				}

				objManager.HideHandles ();
				ShowScaleHandles ();

				isDragging = true;
				//boundingBox.SetParent (m_transformTarget.transform);
			}
		}
			

		if (isDragging && Input.GetMouseButton(0)) {

			Vector3 delta;
			Vector3 newScale;

			switch(activeAxis) {
			case Axis.X:
				delta = mouseControl.ScreenToWorldXZ (Input.mousePosition) - lastMousePosition;
				if (nonUniformScaling) {
					newScale = new Vector3 (initialScale.x * (1f + delta.x * (-12f) / 100), initialScale.y, initialScale.z);
				} else {
					newScale = new Vector3 (initialScale.x * (1f + delta.x * (-12f) / 100), initialScale.y * (1f + delta.x * (-12f) / 100), initialScale.z * (1f + delta.x * (-12f) / 100));
				}
				m_transformTarget.transform.SetParent (ScalingAnchorX);
				ScalingAnchorX.localScale = newScale;
				break;
			case Axis.Y:
				delta = mouseControl.ScreenToWorldXY (Input.mousePosition) - lastMousePosition;
				if (nonUniformScaling) {
					newScale = new Vector3 (initialScale.x, initialScale.y * (1f + delta.y * 6f / 100), initialScale.z);
				} else {
					newScale = new Vector3 (initialScale.x * (1f + delta.y * 12f / 100), initialScale.y * (1f + delta.y * 12f / 100), initialScale.z * (1f + delta.y * 12f / 100));
				}
				m_transformTarget.transform.SetParent (ScalingAnchorY);
				ScalingAnchorY.localScale = newScale;
				break;
			case Axis.Z:
				delta = mouseControl.ScreenToWorldXZ (Input.mousePosition) - lastMousePosition;
				if (nonUniformScaling) {
					newScale = new Vector3 (initialScale.x, initialScale.y, initialScale.z * (1f + delta.z * (-12f) / 100));
				} else {
					newScale = new Vector3 (initialScale.x  * (1f + delta.z * (-12f) / 100), initialScale.y  * (1f + delta.z * (-12f) / 100), initialScale.z * (1f + delta.z * (-12f) / 100));
				}
				m_transformTarget.transform.SetParent (ScalingAnchorZ);
				ScalingAnchorZ.localScale = newScale;
				break;
			default:
				return;
			}

			m_transformTarget.transform.SetParent (ObjectParent);

			//boundingBox.GetComponent<BoundingBox>().UpdatePositionHandles ();
		}

		if (Input.GetMouseButtonUp(0)) {
			objManager.ShowHandles ();
			isDragging = false;
			//boundingBox.SetParent (ObjectParent);
			activeAxis = Axis.None;

			if ((collider == m_xAxis || collider == m_yAxis || collider == m_zAxis || collider == m_allAxis)) {
				collider.transform.GetChild (1).gameObject.SetActive (false);
			}
		}
	}


	public void ToggleScalingMode(){

		nonUniformScaling = !nonUniformScaling;

		if (!nonUniformScaling) {
			// show lines
			lines.SetActive(true);

			// fade in Icon 100%
			lockIcon.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 1f);
			lockIconUnlocked.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 0f);

			m_yAxis.SetActive (true);
			m_xAxis.SetActive (false);
			m_zAxis.SetActive (false);
			m_xAxis2.SetActive (false);
			m_zAxis2.SetActive (false);


		} else {
			// hide lines
			lines.SetActive(false); 

			// fade out icon
			lockIcon.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 0.0f);
			lockIconUnlocked.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 1f);

			m_yAxis.SetActive (true);
			m_xAxis.SetActive (true);
			m_zAxis.SetActive (true);
			m_xAxis2.SetActive (true);
			m_zAxis2.SetActive (true);
		}
	}


	public void HideScaleHandles(){
		
		ToggleUniformScaling.SetActive (false);

		if (!nonUniformScaling) {
			m_yAxis.SetActive (false);
			lines.SetActive (false);
		} else {
			m_xAxis.SetActive (false);
			m_yAxis.SetActive (false);
			m_zAxis.SetActive (false);
			m_xAxis2.SetActive (false);
			m_yAxis2.SetActive (false);
			m_zAxis2.SetActive (false);
		}
	}

	public void ShowScaleHandles(){
		RepositionHandles ();

	//ToggleUniformScaling.SetActive (true);

		if (!nonUniformScaling) {
			m_yAxis.SetActive (true);
			lines.SetActive (true);
		} else {
			m_xAxis.SetActive (true);
			m_yAxis.SetActive (true);
			m_zAxis.SetActive (true);
			m_xAxis2.SetActive (true);
			m_yAxis2.SetActive (true);
			m_zAxis2.SetActive (true);
		}

	}

	public void UnHoverUniformScalingToggle(){

		hoveringUniformScaling = false;

		if (nonUniformScaling) {
			lockIcon.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 0f);
		} else {
			lockIcon.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 1f);
		}
			
		if (nonUniformScaling) {
			lines.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0f);
			lines.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0f);
			lines.transform.GetChild(0).GetChild(2).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0f);
		} else {
			lines.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 1f);
			lines.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 1f);
			lines.transform.GetChild(0).GetChild(2).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 1f);
		}

	}

	public void HoverUniformScalingToggle(){	
	
		hoveringUniformScaling = true;

		lockIcon.GetComponent<Renderer>().material.color = new Color (startColorLockIcon.r,startColorLockIcon.g,startColorLockIcon.b, 0.5f);
	
		lines.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0.5f);
		lines.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0.5f);
		lines.transform.GetChild(0).GetChild(2).GetComponent<Renderer>().material.color = new Color (1f,1f,1f, 0.5f);
	}

	public void RepositionHandles(){

		// always position handles based on bounding box
		m_xAxis2.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [0] + 0.25f * newBoundingBox.coordinatesBoundingBox [4] + 0.25f * newBoundingBox.coordinatesBoundingBox [1] + 0.25f * newBoundingBox.coordinatesBoundingBox [5];
		m_xAxis.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [2] + 0.25f * newBoundingBox.coordinatesBoundingBox [6] + 0.25f * newBoundingBox.coordinatesBoundingBox [3] + 0.25f * newBoundingBox.coordinatesBoundingBox [7];

		m_yAxis.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [0] + 0.25f * newBoundingBox.coordinatesBoundingBox [1] + 0.25f * newBoundingBox.coordinatesBoundingBox [2] + 0.25f * newBoundingBox.coordinatesBoundingBox [3];
		m_yAxis2.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [4] + 0.25f * newBoundingBox.coordinatesBoundingBox [5] + 0.25f * newBoundingBox.coordinatesBoundingBox [6] + 0.25f * newBoundingBox.coordinatesBoundingBox [7];

		m_zAxis.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [1] + 0.25f * newBoundingBox.coordinatesBoundingBox [5] + 0.25f * newBoundingBox.coordinatesBoundingBox [2] + 0.25f * newBoundingBox.coordinatesBoundingBox [6];
		m_zAxis2.transform.position = 0.25f * newBoundingBox.coordinatesBoundingBox [3] + 0.25f * newBoundingBox.coordinatesBoundingBox [7] + 0.25f * newBoundingBox.coordinatesBoundingBox [4] + 0.25f * newBoundingBox.coordinatesBoundingBox [4];
	
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
