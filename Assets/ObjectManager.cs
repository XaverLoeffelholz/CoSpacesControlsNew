using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

	public ScaleController scaleController;
	public TranslationController translationController;
	public RotationController rotationController;

	public ScaleController scaleController2;
	public TranslationController translationController2;

	public GameObject mainCamera;
	public NewBoundingBox newBoundingBox;

	private int handleSet = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		/*

		if ((mainCamera.transform.localRotation.eulerAngles.y - transform.parent.localRotation.eulerAngles.y < 140f) && (mainCamera.transform.localRotation.eulerAngles.y - transform.parent.localRotation.eulerAngles.y > -80f)) {
			if (handleSet == 1) {
				SwitchToHandle2 ();
			}
		} else {
			if (handleSet == 2) {
				SwitchToHandle1 ();
			}
		}

		*/

	}

	public void HideHandles(){

		rotationController.HideRotationHandles ();
		scaleController.HideScaleHandles ();
		translationController.HideTranslationHandles ();

		//transform.GetChild (0).gameObject.SetActive (false);

	}

	public void DeSelect(){
		HideHandles ();
		transform.GetChild (0).gameObject.SetActive (false);
	}

	public void ShowHandles(){

		// check mode:

		ModeChanger.mode currentMode = GameObject.Find ("General Manager").GetComponent<ModeChanger> ().currentMode;

		newBoundingBox.DrawBoundingBox ();

		switch (currentMode) {
			case ModeChanger.mode.Translate:
				translationController.ShowTranslationHandles ();
				scaleController.HideScaleHandles ();
				rotationController.HideRotationHandles ();
				break;
			case ModeChanger.mode.Rotate:
				rotationController.ShowRotationHandles ();
				translationController.HideTranslationHandles ();
				scaleController.HideScaleHandles ();
				break;
			case ModeChanger.mode.Scale:
				scaleController.ShowScaleHandles ();
				translationController.HideTranslationHandles ();
				rotationController.HideRotationHandles ();
				break;
			case ModeChanger.mode.All:
				translationController.ShowTranslationHandles ();
				rotationController.ShowRotationHandles ();
				scaleController.ShowScaleHandles ();
				break;
		}

		transform.GetChild (0).gameObject.SetActive (true);

	}

	public bool IsAnyObjectDragging(){
		if (scaleController.isDragging || translationController.isDragging || rotationController.isDragging) {
			return true;
		} else {
			return false;
		}
	}

	public void SwitchToHandle1(){
		handleSet = 1;
		scaleController2.gameObject.SetActive (false);
		translationController2.gameObject.SetActive (false);
		scaleController.gameObject.SetActive (true);
		translationController.gameObject.SetActive (true);

	}

	public void SwitchToHandle2(){
		handleSet = 2;
		scaleController2.gameObject.SetActive (true);
		translationController2.gameObject.SetActive (true);
		scaleController.gameObject.SetActive (false);
		translationController.gameObject.SetActive (false);
	}
}
