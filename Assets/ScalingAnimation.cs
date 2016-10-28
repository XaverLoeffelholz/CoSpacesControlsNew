using UnityEngine;
using System.Collections;

public class ScalingAnimation : MonoBehaviour {
	private Vector3 initialScale;
	public ScaleController scaleController;
	private bool nonUniformScaling;

	// Use this for initialization
	void Start () {
		nonUniformScaling = scaleController.nonUniformScaling; 
		initialScale = transform.localScale;
		ScaleUp ();
	}
	
	// Update is called once per frame
	void Update () {
		if (nonUniformScaling != scaleController.nonUniformScaling) {
			nonUniformScaling = scaleController.nonUniformScaling;
			LeanTween.cancel(this.gameObject);
			ScaleUp ();
		}
	}

	public void ScaleUp(){
		if (scaleController.nonUniformScaling) {
			LeanTween.scale (this.gameObject, new Vector3 (initialScale.x, initialScale.y * 5, initialScale.z), 1f).setEase (LeanTweenType.easeInOutExpo).setOnComplete (ScaleDown);
		} else {
			LeanTween.scale (this.gameObject, new Vector3 (initialScale.x * 2, initialScale.y * 2, initialScale.z * 2), 1f).setEase (LeanTweenType.easeInOutExpo).setOnComplete (ScaleDown);
		}

	}

	public void ScaleDown(){
		LeanTween.scale (this.gameObject, new Vector3(initialScale.x, initialScale.y, initialScale.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(ScaleUp);
	}
}
