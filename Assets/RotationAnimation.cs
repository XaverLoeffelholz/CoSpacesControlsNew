using UnityEngine;
using System.Collections;

public class RotationAnimation : MonoBehaviour {
	private Vector3 initialRot;

	// Use this for initialization
	void Start () {
		initialRot = transform.localRotation.eulerAngles;
		RotateUp ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			this.gameObject.SetActive(false);
		} 

		if (Input.GetMouseButtonUp(0)){
			this.gameObject.SetActive(true);
		} 
	}

	public void RotateUp(){
		LeanTween.rotateLocal (this.gameObject, new Vector3(initialRot.x, initialRot.y-70f, initialRot.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(RotateDown);

	}

	public void RotateDown(){
		LeanTween.rotateLocal (this.gameObject, new Vector3(initialRot.x, initialRot.y, initialRot.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(RotateUp);
	}
}
