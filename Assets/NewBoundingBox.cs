using UnityEngine;
using System.Collections;

public class NewBoundingBox : MonoBehaviour {

	public GameObject[] coordinates;
	public Vector3[] coordinatesBoundingBox;
	public GameObject linesPrefab;

	public bool localCoordinates;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DeleteBoundingBox(){
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}
	}

	public void SetWorld(){
		localCoordinates = false;
	}

	public void SetLocal(){
		localCoordinates = true;
	}


	public void DrawBoundingBox(){	
		DeleteBoundingBox ();

		CalculateBoundingBox ();	
		GameObject linesGO = Instantiate(linesPrefab);
		linesGO.transform.SetParent(transform);

		Lines lines = linesGO.GetComponent<Lines> ();

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[0], coordinatesBoundingBox[1], coordinatesBoundingBox[2], coordinatesBoundingBox[3]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[4], coordinatesBoundingBox[5], coordinatesBoundingBox[6], coordinatesBoundingBox[7]});

		/*
		if (coordinatesBoundingBox [4].y > 0.1f) {
			lines.DrawLinesWorldCoordinate(new Vector3[] { new Vector3(coordinatesBoundingBox[4].x,0f,coordinatesBoundingBox[4].z), 
				new Vector3(coordinatesBoundingBox[5].x,0f,coordinatesBoundingBox[5].z), 
				new Vector3(coordinatesBoundingBox[6].x,0f,coordinatesBoundingBox[6].z), 
				new Vector3(coordinatesBoundingBox[7].x,0f,coordinatesBoundingBox[7].z) });

			Vector3 centerBottomBB = 0.25f * new Vector3 (coordinatesBoundingBox [4].x, 0f, coordinatesBoundingBox [4].z) +
				0.25f *  new Vector3 (coordinatesBoundingBox [5].x, 0f, coordinatesBoundingBox [5].z) +
				0.25f *  new Vector3 (coordinatesBoundingBox [6].x, 0f, coordinatesBoundingBox [6].z) +
				0.25f *   new Vector3 (coordinatesBoundingBox [7].x, 0f, coordinatesBoundingBox [7].z);

			Vector3 centerBottomBBground = 0.25f * coordinatesBoundingBox[4] +
				0.25f *  coordinatesBoundingBox[5] +
				0.25f *  coordinatesBoundingBox[6] +
				0.25f *  coordinatesBoundingBox[7];

			lines.DrawLinesWorldCoordinate(new Vector3[] {centerBottomBB, centerBottomBBground});
		}*/

		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[0], coordinatesBoundingBox[4]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[1], coordinatesBoundingBox[5]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[2], coordinatesBoundingBox[6]});
		lines.DrawLinesWorldCoordinate(new Vector3[] {coordinatesBoundingBox[3], coordinatesBoundingBox[7]});
	}

	public void CalculateBoundingBox()
	{
		coordinatesBoundingBox = new Vector3[8];

		// get highest and lowest values for x,y,z
		Vector3 minima; 
		Vector3 maxima;

		if (localCoordinates) {
			// set all points
			coordinatesBoundingBox[0] = coordinates[0].transform.position;
			coordinatesBoundingBox[1] = coordinates[1].transform.position;
			coordinatesBoundingBox[2] = coordinates[2].transform.position;
			coordinatesBoundingBox[3] = coordinates[3].transform.position;

			coordinatesBoundingBox[4] = coordinates[4].transform.position;
			coordinatesBoundingBox[5] = coordinates[5].transform.position;
			coordinatesBoundingBox[6] = coordinates[6].transform.position;
			coordinatesBoundingBox[7] = coordinates[7].transform.position;
		} else {
			minima = GetBoundingBoxMinima(false);
			maxima = GetBoundingBoxMaxima(false); 

			// set all points
			coordinatesBoundingBox[0] = new Vector3(maxima.x, maxima.y, maxima.z);
			coordinatesBoundingBox[1] = new Vector3(maxima.x, maxima.y, minima.z);
			coordinatesBoundingBox[2] = new Vector3(minima.x, maxima.y, minima.z);
			coordinatesBoundingBox[3] = new Vector3(minima.x, maxima.y, maxima.z);

			coordinatesBoundingBox[4] = new Vector3(maxima.x, minima.y, maxima.z);
			coordinatesBoundingBox[5] = new Vector3(maxima.x, minima.y, minima.z);
			coordinatesBoundingBox[6] = new Vector3(minima.x, minima.y, minima.z);
			coordinatesBoundingBox[7] = new Vector3(minima.x, minima.y, maxima.z);
		}


	}

	public Vector3 GetBoundingBoxMinima(bool local)
	{
		Vector3 minima = new Vector3 (9999f, 9999f, 9999f);

		for (int i = 0; i < coordinates.Length; i++)
		{
			Vector3 current = coordinates[i].transform.position;

			if (local) {
				current = coordinates[i].transform.localPosition;
			}

			if (current.x < minima.x) {
				minima.x = current.x;
			}

			if (current.y < minima.y) {
				minima.y = current.y;
			}

			if (current.z < minima.z) {
				minima.z = current.z;
			}

		}
		return minima;
	}


	public Vector3 GetBoundingBoxMaxima(bool local)
	{
		Vector3 maxima = new Vector3 (-9999f, -9999f, -9999f);

		for (int i = 0; i < coordinates.Length; i++)
		{
			Vector3 current = coordinates[i].transform.position;

			if (local) {
				current = coordinates[i].transform.localPosition;
			}

			if (current.x > maxima.x) {
				maxima.x = current.x;
			}

			if (current.y > maxima.y) {
				maxima.y = current.y;
			}

			if (current.z > maxima.z) {
				maxima.z = current.z;
			}

		}
			
		return maxima;
	}

	public void SwitchToLocalSpace(){

	}

	public void SwitchToWorldSpace(){

	}


	public Vector3 GetTopCenter(){
		Vector3 topCenter = 0.25f * coordinatesBoundingBox [0] + 0.25f * coordinatesBoundingBox [1] + 0.25f * coordinatesBoundingBox [2] + 0.25f * coordinatesBoundingBox [3];
		return topCenter;
	}

	public Vector3 GetBottomCenter(){
		Vector3 bottomCenter = 0.25f * coordinatesBoundingBox [4] + 0.25f * coordinatesBoundingBox [5] + 0.25f * coordinatesBoundingBox [6] + 0.25f * coordinatesBoundingBox [7];
		return bottomCenter;
	}

	public Vector3 GetCenter(){
		Vector3 center = GetTopCenter () * 0.5f + GetBottomCenter () * 0.5f;
		return center;
	}

	public Vector3 CenterPosXDirection(bool local){
		Vector3 newPos = new Vector3(GetBoundingBoxMaxima(local).x,GetCenter().y, GetCenter().z);
		return newPos;
	}

	public Vector3 CenterNegXDirection(bool local){
		Vector3 newPos = new Vector3(GetBoundingBoxMinima(local).x,GetCenter().y, GetCenter().z);
		return newPos;
	}

	public Vector3 CenterPosZDirection(bool local){
		Vector3 newPos = new Vector3(GetCenter().x,GetCenter().y, GetBoundingBoxMaxima(local).z);
		return newPos;
	}

	public Vector3 CenterNegZDirection(bool local){
		Vector3 newPos = new Vector3(GetCenter().x,GetCenter().y, GetBoundingBoxMinima(local).z);
		return newPos;
	}


	public int closestCorner(Vector3 position){

		Vector3 closestVertex = coordinatesBoundingBox [0];
		float shortestDistance = 999999f;
		int idOfVertex = 0;

		for (int i = 0; i < 4; i++)
		{
			Vector3 newCoordinate = coordinatesBoundingBox[i];

			if (Vector3.Distance(position, newCoordinate) < shortestDistance)
			{
				closestVertex = newCoordinate;
				shortestDistance = Vector3.Distance(position, newCoordinate);
				idOfVertex = i;
			}
		}

		return idOfVertex;
	}

    public void UpdateBoundingBox()
    {
        // use Function from MA
    }

}