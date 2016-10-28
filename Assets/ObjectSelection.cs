using UnityEngine;
using System.Collections;

public class ObjectSelection : MonoBehaviour {

	public Camera mainCamera;
	private GameObject colliderGO;

	private GameObject hoveredCurrent;
	private GameObject selectedCurrent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		if (hoveredCurrent && selectedCurrent==null) {
			hoveredCurrent.transform.GetChild (0).gameObject.SetActive (false);
		}


		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 1000f)) {

			// select object
			colliderGO = hit.collider.gameObject;

			if (colliderGO.CompareTag ("Object") && colliderGO != selectedCurrent) {
				
				hoveredCurrent = colliderGO;
				hoveredCurrent.transform.GetChild (0).gameObject.SetActive (true);

				if (Input.GetMouseButtonDown (0)) {
					if (selectedCurrent) {
						
						selectedCurrent.transform.parent.parent.GetComponent<ObjectManager> ().DeSelect ();

						//selectedCurrent.transform.parent.parent.GetChild (0).gameObject.SetActive (false);
						selectedCurrent.transform.GetChild (0).gameObject.SetActive (false);
					}

					selectedCurrent = colliderGO;
					selectedCurrent.transform.parent.parent.GetComponent<ObjectManager> ().ShowHandles ();

					// selectedCurrent.transform.parent.parent.GetChild (0).gameObject.SetActive (true);
				}
			}
		} 


	}
}
