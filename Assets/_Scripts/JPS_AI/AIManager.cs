using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Point
{
	public Point(Vector3 pointPosition){
		coordinates = pointPosition; 
		dangerScore = 0;
		adjacentPoints = new List<Point>();
		previousDangerScores = new List<float>();
	}

	public Vector3 coordinates;
	public float dangerScore;
	public List<Point> adjacentPoints;
	public List<float> previousDangerScores;
}

public class AIManager : MonoBehaviour {

	public bool debugMode = true;

	[SerializeField]
	private int framesBetweenScans = 1;

	private int gridLength = 50;
	private float pointSeparation;

	private GameObject debuggingPoint;

	private ShipMovement playerShipMovement;

	private List<Point> grid;

	// Use this for initialization
	void Start () {
		this.playerShipMovement = gameObject.GetComponent<ShipMovement>();
		this.debuggingPoint = Resources.Load<GameObject>("Prefabs/DebuggingPoint");

		this.grid = new List<Point>();

		//Determine point separation based on the longer side (horizontal length)
		this.pointSeparation = Mathf.Abs(this.playerShipMovement.viewportMaxX - this.playerShipMovement.viewportMinX) / this.gridLength;

		this.BuildGrid();
		this.PopulateAdjacentPoints();
	}

	private void BuildGrid(){
		for (float xCoordinate = this.playerShipMovement.viewportMinX; xCoordinate <= this.playerShipMovement.viewportMaxX; xCoordinate += this.pointSeparation) { 
			for (float yCoordinate = this.playerShipMovement.viewportMinY; yCoordinate <= this.playerShipMovement.viewportMaxY; yCoordinate += this.pointSeparation) {
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, yCoordinate, 0));
				Point currentPoint = new Point(currentPointPosition);
				this.grid.Add(currentPoint);

				//Show Debug points
				if (this.debugMode == true) {
					GameObject pointObject = Instantiate(debuggingPoint, currentPointPosition, new Quaternion()) as GameObject;
					DebugPoint debugPoint = pointObject.GetComponent<DebugPoint>();
					debugPoint.pointReference = this.grid.Last();
				}
			}
		}
	}

	private void PopulateAdjacentPoints(){
		for (int i = 0; i < this.grid.Count; i++) {
			for (int j = 0; j < 4; j++) {
				//Left Point
				if (i > 0) {
					this.grid[i].adjacentPoints.Add(this.grid[i - 1]);
				}
				//Right Point
				if (i < this.grid.Count - 1) {
					this.grid[i].adjacentPoints.Add(this.grid[i + 1]);
				}
				//Above Point
				if (i > this.gridLength) {
					this.grid[i].adjacentPoints.Add(this.grid[i - this.gridLength]);
				}
				//Below Point
				if (i < (this.gridLength * this.gridLength - 1)) {
					this.grid[i].adjacentPoints.Add(this.grid[i + this.gridLength]);
				}
			}
		}
	}

	/*private IEnumerator Scan()
	{
		//Initial Pass to find bullets
		foreach (Point currentPoint in this.grid){
			RaycastHit[] detectedObjects = Physics.SphereCastAll(currentPoint.coordinates, 0.1f, Vector3.zero);
		}
	}*/

	// Update is called once per frame
	void Update () {
		
	}
}
