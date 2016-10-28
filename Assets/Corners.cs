using UnityEngine;
using System.Collections;

public class Corners : MonoBehaviour {

	public GameObject Camera;
	public GameObject NonUniformScalingButton;

	public Transform p1;
	public Transform p2;
	public Transform p3;
	public Transform p4;
	private Transform[] points;

	public GameObject[] RotationHandles1;
	public GameObject[] RotationHandles2;
	public GameObject[] RotationHandles3;
	public GameObject[] RotationHandles4;

	float distance = 0f;
	public Vector3 closestPoint;

	public NewBoundingBox newBoundingBox;

	// Use this for initialization
	void Start () {
		points = new Transform[4];
		points [0] = p1;
		points [1] = p2;
		points [2] = p3;
		points [3] = p4;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetMouseButton(1)) {

			// check if object is selected!!!!

			UpdateRotationHandles ();
		}
	}

	public void UpdateRotationHandles(){
		Vector3 closestPoint = ClosestToCamera();

		HideRotationHandles (RotationHandles1);
		HideRotationHandles (RotationHandles2);
		HideRotationHandles (RotationHandles3);
		HideRotationHandles (RotationHandles4);

		if (closestPoint == newBoundingBox.coordinatesBoundingBox[2]) {
			DisplayRotationHandles (RotationHandles1);
		} else if (closestPoint == newBoundingBox.coordinatesBoundingBox[3]) {
			DisplayRotationHandles (RotationHandles2);
		} else if (closestPoint == newBoundingBox.coordinatesBoundingBox[0]) {
			DisplayRotationHandles (RotationHandles3);
		} else if (closestPoint == newBoundingBox.coordinatesBoundingBox[1]) {
			DisplayRotationHandles (RotationHandles4);
		}

		NonUniformScalingButton.transform.position = closestPoint;

	}

	public Vector3 ClosestToCamera(){
		closestPoint = newBoundingBox.coordinatesBoundingBox[0];
		distance = 99999f;

		for (int i = 0; i < 4; i++) {
			float distanceNew = Vector3.Distance(newBoundingBox.coordinatesBoundingBox[i], Camera.transform.position);

			if (distanceNew < distance) {
				distance = distanceNew;
				closestPoint = newBoundingBox.coordinatesBoundingBox [i];
			}
		}

		return closestPoint;
	}

	public void DisplayRotationHandles(GameObject[] RotationHandles) {
		for (int i = 0; i < RotationHandles.Length; i++) {
			RotationHandles [i].SetActive (true);
		}
	}

	public void HideRotationHandles(GameObject[] RotationHandles) {
		for (int i = 0; i < RotationHandles.Length; i++) {
			RotationHandles [i].SetActive (false);
		}
	}
}
