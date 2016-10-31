using UnityEngine;
using System.Collections;

public class Handle : MonoBehaviour {

	public enum HandleType { translateY, ScaleUniform, ScaleNonUniform, Rotate };

	public HandleType typeOfHandle;

	public bool dragging;
	private float rayDistance;

	Plane dragPlane;
	private Vector3 posOnDragBegin;
	private Vector3 offsetOnDragBegin;
	public Vector3 InitialTopToBottom;
    public Vector3 InitialHandleToCenter;

    public ObjectScript connectObject;

	private Vector3 scaleOneBeginDrag;

	// Use this for initialization
	void Start () {

        // quick hack
		connectObject =GameObject.Find("CubeObject").GetComponent<ObjectScript> ();

    }
	
	// Update is called once per frame
	void Update () {
		if (dragging) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // ray to ground plane
            if (dragPlane.Raycast(ray, out rayDistance))
            {

                if (typeOfHandle == HandleType.translateY)
                {
                    // maybe add maximum
                    Vector3 newPos = connectObject.transform.position + Vector3.Project((ray.GetPoint(rayDistance) + offsetOnDragBegin) - connectObject.transform.position, connectObject.transform.up);

                    newPos = Vector3.ClampMagnitude(RasterManager.Instance.Raster(newPos), 10f);

                    // check y is not below 0
                    if (newPos.y < 0f)
                    {
                        newPos = new Vector3(newPos.x, 0f, newPos.z);
                    }

                    connectObject.transform.position = newPos;
                }
                else if (typeOfHandle == HandleType.ScaleUniform)
                {
                    Vector3 newPos = connectObject.transform.position + Vector3.Project((ray.GetPoint(rayDistance) + offsetOnDragBegin) - connectObject.transform.position, connectObject.transform.up);
                    newPos = Vector3.ClampMagnitude(RasterManager.Instance.Raster(newPos), 10f);

                    // check y is not below 0
                    if (newPos.y < 0f)
                    {
                        newPos = new Vector3(newPos.x, 0f, newPos.z);
                    }

                    // get new scale Top to Bottom
                    Vector3 newTopToBottom = newPos - connectObject.boundingBox.GetBottomCenter();

                    // scale accordingly
                    Vector3 newScale = scaleOneBeginDrag * (newTopToBottom.magnitude / InitialTopToBottom.magnitude);

                    connectObject.cube.transform.parent.localScale = Vector3.ClampMagnitude(new Vector3(Mathf.Max(0.4f, newScale.x), Mathf.Max(0.4f, newScale.y), Mathf.Max(0.4f, newScale.z)), 7f);
                    connectObject.boundingBox.CalculateBoundingBox();

                    // use update bb from master thesis
                    connectObject.boundingBox.DrawBoundingBox();
                    connectObject.PlaceHandles();
                }
                else if (typeOfHandle == HandleType.ScaleNonUniform)
                {
                    Debug.Log("nonuniform in action");

                    Vector3 newPos = transform.parent.position + Vector3.Project((ray.GetPoint(rayDistance) + offsetOnDragBegin) - transform.parent.position, transform.parent.forward);
                    newPos = RasterManager.Instance.Raster(newPos);


                    // get new scale Top to Bottom
                    Vector3 newHandleToCenter= newPos - connectObject.boundingBox.GetCenter();

                    // scale accordingly
                    Vector3 newScale = scaleOneBeginDrag * (newHandleToCenter.magnitude / InitialHandleToCenter.magnitude);

                    //connectObject.cube.transform.parent.localScale = Vector3.ClampMagnitude(new Vector3(Mathf.Max(0.4f, newScale.x), Mathf.Max(0.4f, newScale.y), Mathf.Max(0.4f, newScale.z)), 7f);

                    // apply only to one direction
                    newScale = Vector3.Project(newScale, transform.parent.forward);

                    if (newScale.x <= 0.01f)
                    {
                        newScale = new Vector3(connectObject.cube.transform.parent.localScale.x, newScale.y, newScale.z);
                    }

                    if (newScale.y <= 0.01f)
                    {
                        newScale = new Vector3(newScale.x, connectObject.cube.transform.parent.localScale.y, newScale.z);
                    }

                    if (newScale.z <= 0.01f)
                    { 
                        newScale = new Vector3(newScale.x, newScale.y, connectObject.cube.transform.parent.localScale.z);
                    }

                    connectObject.cube.transform.parent.localScale = newScale;
                    connectObject.boundingBox.CalculateBoundingBox();

                    // use update bb from master thesis
                    connectObject.boundingBox.DrawBoundingBox();
                    connectObject.PlaceHandles();

                }
                else if (typeOfHandle == HandleType.Rotate)
                {
                    Debug.Log("Rotation in action");
                }
            }
        }
	}

	public void StartDragging(){
		
		if (typeOfHandle == HandleType.translateY || typeOfHandle == HandleType.ScaleUniform || typeOfHandle == HandleType.ScaleNonUniform) {
			// create plane 

			dragPlane = new Plane ();

			Vector3 groundPointBelowCam = new Vector3 (Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
			Vector3 groundPointBelowObject = new Vector3 (connectObject.transform.position.x, 0f, connectObject.transform.position.z);

			dragPlane.SetNormalAndPosition (groundPointBelowCam - groundPointBelowObject, transform.position); 

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (dragPlane.Raycast (ray, out rayDistance)) {

				posOnDragBegin = ray.GetPoint(rayDistance);

				if (typeOfHandle == HandleType.translateY) {
					offsetOnDragBegin = connectObject.transform.position - posOnDragBegin;
				} else if (typeOfHandle == HandleType.ScaleUniform){
					offsetOnDragBegin = connectObject.boundingBox.GetTopCenter() - posOnDragBegin;
					scaleOneBeginDrag = connectObject.cube.transform.parent.localScale;

					// get current scale Top to bottom
					InitialTopToBottom = connectObject.boundingBox.GetTopCenter() - connectObject.boundingBox.GetBottomCenter();
				} else if (typeOfHandle == HandleType.ScaleUniform)
                {
                    offsetOnDragBegin = connectObject.boundingBox.GetTopCenter() - posOnDragBegin;
                    scaleOneBeginDrag = connectObject.cube.transform.parent.localScale;

                    // get current scale Top to bottom
                    InitialTopToBottom = connectObject.boundingBox.GetTopCenter() - connectObject.boundingBox.GetBottomCenter();
                }
                else if (typeOfHandle == HandleType.ScaleNonUniform)
                {
                    offsetOnDragBegin = transform.parent.position - posOnDragBegin;
                    scaleOneBeginDrag = connectObject.cube.transform.parent.localScale;

                    // get current scale Top to bottom
                    InitialHandleToCenter = transform.parent.position - connectObject.boundingBox.GetCenter();
                }

            }

			dragging = true;

            connectObject.HideHandlesExcept(transform.parent.gameObject);
        }
	}

	public void StopDragging(){
		dragging = false;
        connectObject.ShowAllHandles();
	}

	public void Focus(){

	}

	public void UnFocus(){

	}

	public void DragY(){

	}

	public void ScaleXYZ(){

	}

	public void Rotate(){

	}

	public void ScaleNonUniform(){

	}
}
