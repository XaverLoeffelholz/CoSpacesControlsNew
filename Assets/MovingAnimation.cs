using UnityEngine;
using System.Collections;

public class MovingAnimation : MonoBehaviour {
	private Vector3 initialPos;


	// Use this for initialization
	void Start () {
		initialPos = transform.localPosition;
		ScaleUp ();
	}

	// Update is called once per frame
	void Update () {

	}

	public void ScaleUp(){
		LeanTween.moveLocal (this.gameObject, new Vector3(initialPos.x, initialPos.y*4, initialPos.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(ScaleDown);
	}

	public void ScaleDown(){
		LeanTween.moveLocal (this.gameObject, new Vector3(initialPos.x, initialPos.y, initialPos.z), 1f).setEase( LeanTweenType.easeInOutExpo ).setOnComplete(ScaleUp);
	}
}
