using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PolarCoordinates;

public class Point
{
	public Point(Vector3 pointPosition, int scanIterations){
		coordinates = pointPosition; 
		dangerScore = 0;
		adjacentPoints = new List<Point>();
		previousDangerScores = new float[scanIterations];
	}

	public Vector3 coordinates;
	public float dangerScore;
	public List<Point> adjacentPoints;
	public float[] previousDangerScores;
}

public class TravelPath
{
	public TravelPath(){
		pathVector = Vector3.zero;
		dangerScore = int.MaxValue;
	}

	public Vector3 pathVector;
	public float dangerScore;
}

//TODO: NavPoints have colliders anyway.  Do we need to scan them with overlap spheres?  Nope.
public class AIManager : MonoBehaviour {

	public static Player controlledPlayer = Player.player2;
	public static bool debugMode = false;

	[SerializeField]
	private int framesBetweenScans = 5;
	private int passedFrames = 0;
	private int numberOfScanIterations = 5;

	[SerializeField]
	private LayerMask hitLayer;

	private int gridLength = 40;
	private float pointSeparation;

	private int borderWidth = 10;

	private GameObject navigationPointPrefab;

	private ShipMovement playerShipMovement;

	private List<Point> grid;

	//Ship Scanning variables
	private float shipScanDegreeSeparation = 5f;
	//TODO: Make these more dynamic, so they don't just cover the whole circle.  Otherwise they don't mean anything.
	private float startingAngle = 0f;
	private float endingAngle = 360f;
	private float sphereCastRadius = 0.3f;
	private float sphereCastMagnitude = 3f;
	private int shipScanLayerMask = 0;
	private TravelPath currentlyTravellingPath;
	private Vector3 previousDirection = Vector3.zero;	//Used for Lerping between directions to prevent jittering
	private float lerpSpeed = 0.3f;

	// Use this for initialization
	void Start () {
		AIManager.debugMode = true;

		this.playerShipMovement = gameObject.GetComponent<ShipMovement>();
		this.navigationPointPrefab = Resources.Load<GameObject>("Prefabs/NavigationPoint");

		this.grid = new List<Point>();
		this.currentlyTravellingPath = new TravelPath();

		//Determine point separation based on the longer side (horizontal length)
		this.pointSeparation = Mathf.Abs(this.playerShipMovement.viewportMaxX - this.playerShipMovement.viewportMinX) / this.gridLength;

		this.BuildGrid();
		this.BuildBorders();
		this.PopulateAdjacentPoints();

		this.shipScanLayerMask = 1 << LayerMask.NameToLayer("NavigationPoint");
	}

	private void BuildGrid(){
		for (float yCoordinate = this.playerShipMovement.viewportMinY; yCoordinate <= this.playerShipMovement.viewportMaxY; yCoordinate += this.pointSeparation) { 
			for (float xCoordinate = this.playerShipMovement.viewportMinX; xCoordinate <= this.playerShipMovement.viewportMaxX; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				Point currentPoint = new Point(adjustedPointPosition, this.numberOfScanIterations);
				this.grid.Add(currentPoint);

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.pointReference = this.grid.Last();  //JPS  Can't I just use currentPoint?
			}
		}
	}

	private void BuildBorders()
	{
		float borderLength = this.borderWidth * this.pointSeparation;

		//Top Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float xCoordinate = this.playerShipMovement.viewportMinX - borderLength; xCoordinate <= this.playerShipMovement.viewportMaxX + borderLength; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, this.playerShipMovement.viewportMaxY + (this.pointSeparation * (rowIndex + 1)), 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				Point currentPoint = new Point(adjustedPointPosition, 1);
				currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.pointReference = currentPoint;	
			}
		}

		//Bottom Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float xCoordinate = this.playerShipMovement.viewportMinX - borderLength; xCoordinate <= this.playerShipMovement.viewportMaxX + borderLength; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, this.playerShipMovement.viewportMinY - (this.pointSeparation * (rowIndex + 1)), 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				Point currentPoint = new Point(adjustedPointPosition, 1);
				currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.pointReference = currentPoint;	
			}
		}

		//Left Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float yCoordinate = this.playerShipMovement.viewportMinY - borderLength; yCoordinate <= this.playerShipMovement.viewportMaxY + borderLength; yCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(this.playerShipMovement.viewportMinX - (this.pointSeparation * (rowIndex + 1)), yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				Point currentPoint = new Point(adjustedPointPosition, 1);
				currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.pointReference = currentPoint;	
			}
		}

		//Right Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float yCoordinate = this.playerShipMovement.viewportMinY - borderLength; yCoordinate <= this.playerShipMovement.viewportMaxY + borderLength; yCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.playerShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(this.playerShipMovement.viewportMaxX + (this.pointSeparation * rowIndex), yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				Point currentPoint = new Point(adjustedPointPosition, 1);
				currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.pointReference = currentPoint;	
			}
		}
	}

	private void PopulateAdjacentPoints(){
		for (int i = 0; i < this.grid.Count; i++) {
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

	private void Scan()
	{
		//TODO: JPS: Experiment having the first pass done through OnTriggerStay/Exit, and having a generic "Weapon" interface that contains the owningPlayer.
		//Initial Pass to find bullets
		foreach (Point currentPoint in this.grid){
			Collider[] detectedObjects = Physics.OverlapSphere(new Vector3(currentPoint.coordinates.x, currentPoint.coordinates.y, 0), 0.5f, this.hitLayer);
			if (detectedObjects.Length > 0) {
				currentPoint.dangerScore = 1;
				currentPoint.previousDangerScores[0] = currentPoint.dangerScore;
			}
			else {
				currentPoint.dangerScore = 0;
				currentPoint.previousDangerScores[0] = currentPoint.dangerScore;
			}
		}

		//Subsequent passes, for blurring the danger scores
		for (int iteration = 0; iteration < this.numberOfScanIterations; iteration++){
			//Iterate through each point
			foreach (Point currentPoint in this.grid) {
				//Only composite scores for points with dangerScores less than 1
				if (currentPoint.dangerScore != 1) {
					float compositeDangerScore = 0;
					//Iterate through each neighbor point and composite a score
					foreach (Point neighborPoint in currentPoint.adjacentPoints) {
						compositeDangerScore += (1.0f / (float)currentPoint.adjacentPoints.Count) * neighborPoint.previousDangerScores[iteration];
					}

					currentPoint.dangerScore = compositeDangerScore;
				}
				//Set previous danger score value if another iteration is happening
				if (iteration != (this.numberOfScanIterations - 1)) {
					currentPoint.previousDangerScores[iteration + 1] = currentPoint.dangerScore;
				}
			}
		}
	}

	private void UpdateCurrentPathDangerScore(){
		if (this.currentlyTravellingPath.pathVector == Vector3.zero) {
			return;
		}

		RaycastHit[] detectedObjects = Physics.SphereCastAll(this.transform.position, this.sphereCastRadius, this.currentlyTravellingPath.pathVector.normalized, this.sphereCastMagnitude, this.shipScanLayerMask);

		//Composite a total danger score for that direction
		float totalDangerScore = 0;
		foreach (RaycastHit hitInfo in detectedObjects) {
			NavigationPoint currentNavPoint = hitInfo.collider.gameObject.GetComponent<NavigationPoint>();
			totalDangerScore += currentNavPoint.pointReference.dangerScore;
		}

		this.currentlyTravellingPath.dangerScore = totalDangerScore;
	}

	private void SearchForSafeDirection(){
		TravelPath chosenPath = this.currentlyTravellingPath;

		//Scan from startAngle to endAngle
		for (float i = this.startingAngle; i < this.endingAngle; i += this.shipScanDegreeSeparation){
			PolarCoordinate scanDirection = new PolarCoordinate(this.sphereCastMagnitude, i * Mathf.Deg2Rad);
			RaycastHit[] detectedObjects = Physics.SphereCastAll(this.transform.position, this.sphereCastRadius, scanDirection.PolarToCartesian().normalized, this.sphereCastMagnitude, this.shipScanLayerMask);
			TravelPath currentCheckedPath = new TravelPath();

			Debug.DrawRay(this.transform.position, scanDirection.PolarToCartesian(), Color.red);

			//Composite a total danger score for that direction
			float totalDangerScore = 0;
			foreach (RaycastHit hitInfo in detectedObjects) {
				NavigationPoint currentNavPoint = hitInfo.collider.gameObject.GetComponent<NavigationPoint>();
				totalDangerScore += currentNavPoint.pointReference.dangerScore;
			}

			currentCheckedPath.pathVector = scanDirection.PolarToCartesian();
			currentCheckedPath.dangerScore = totalDangerScore;

			//Replace chosen direction if it's determined to be safer
			if (currentCheckedPath.dangerScore < this.currentlyTravellingPath.dangerScore) {
				chosenPath.pathVector = scanDirection.PolarToCartesian();
				chosenPath.dangerScore = totalDangerScore;
			}
		}

		this.DetermineDirectionMagnitude(chosenPath);
	}

	//Lerp to the correct magnitude to allow for natural-looking movement
	private void DetermineDirectionMagnitude(TravelPath chosenPath){
		this.currentlyTravellingPath.pathVector = Vector3.Lerp(this.previousDirection, chosenPath.pathVector.normalized, this.lerpSpeed);
		this.currentlyTravellingPath.dangerScore = chosenPath.dangerScore;
		this.previousDirection = this.currentlyTravellingPath.pathVector;
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 moveDirection = Vector3.zero;
		if (this.passedFrames < this.framesBetweenScans) {
			this.Scan();
			this.passedFrames += 1;
			this.UpdateCurrentPathDangerScore();
			this.SearchForSafeDirection();
		}
		else {
			this.passedFrames = 0;
		}

		this.playerShipMovement.Move(this.currentlyTravellingPath.pathVector);
	}
}
