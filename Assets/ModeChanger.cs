using UnityEngine;
using System.Collections;

public class ModeChanger : MonoBehaviour {

	public enum mode {Scale, Rotate, Translate, All};
	public mode currentMode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ScaleMode(){
		currentMode = mode.Scale;
		Debug.Log ("Scale Mode");
	}

	public void RotationMode(){
		currentMode = mode.Rotate;
		Debug.Log ("Rotate Mode");

	}

	public void TranslationMode(){
		currentMode = mode.Translate;
		Debug.Log ("Translate Mode");

	}

	public void AllMode(){
		currentMode = mode.All;
		Debug.Log ("All Mode");
	}
}
