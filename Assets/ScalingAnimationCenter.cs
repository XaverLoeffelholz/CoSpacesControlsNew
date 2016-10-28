using UnityEngine;
using System.Collections;

public class ScalingAnimationCenter : MonoBehaviour {
	private Vector3 initialScale;


	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
		ScaleUp ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void ScaleUp(){
		LeanTween.scale (this.gameObject, new Vector3(initialScale.x*2, initialScale.y*2, initialScale.z*2), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(ScaleDown);
	}

	public void ScaleDown(){
		LeanTween.scale (this.gameObject, new Vector3(initialScale.x, initialScale.y, initialScale.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(ScaleUp);
	}
}
