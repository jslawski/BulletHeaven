using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PolarCoordinates;

//TODO: this.comShipMovement.gameObject.transform.position isn't right anymore.  The real part that is changing is the child prefab of where this script is attached.  
//Use THAT object's transform instead.  Either have a reference to the prefab, or use this.comShipMovement.gameObject.transform.position.


public enum AIState{
	Evasive,
	Attacking,
	PursuingHealthPack,
	Inactive
}

/*public class Point
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
}*/

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

	//public static Player controlledPlayer = PlayerEnum.player2;
	public static bool debugMode = false;

	[SerializeField]
	private int framesBetweenScans = 5;
	private int passedFrames = 0;
	private int numberOfScanIterations = 2;

	[SerializeField]
	private LayerMask hitLayer;

	private int gridLength = 40;
	private float pointSeparation;

	private int borderWidth = 10;

	private GameObject navigationPointPrefab;

	private ShipMovement comShipMovement;

	//TODO: Have this set in code
	[SerializeField]
	private ShipMovement playerShipMovement;


	private List<NavigationPoint> grid;

	//Ship Scanning variables
	private float shipScanDegreeSeparation = 5f * Mathf.Deg2Rad;
	//TODO: Make these more dynamic, so they don't just cover the whole circle.  Otherwise they don't mean anything.
	private float startingAngle = 0f;
	private float endingAngle = 360f;
	private float sphereCastRadius = 0.3f;
	private float sphereCastMagnitude = 6f;
	private int shipScanLayerMask = 0;
	private TravelPath currentlyTravellingPath;
	private Vector3 previousDirection = Vector3.zero;	//Used for Lerping between directions to prevent jittering
	private Vector3 nextDirection = Vector3.zero;

	//AI State variables
	private AIState currentAIState = AIState.Inactive;
	private float evadeTriggerThreshold = 0.3f;
	private float secondsBetweenAttackReposition = 0.5f;
	private float offsetYRangeForProactiveState = 5f;

	// Use this for initialization
	IEnumerator Start () {
		while (!GameManager.S.shipsReady) {
			yield return null;
		}

		AIManager.debugMode = true;

		this.comShipMovement = gameObject.GetComponentInChildren<ShipMovement>();

		//TODO: Stop doing this
		this.playerShipMovement = GameObject.Find("Player1").GetComponentInChildren<ShipMovement>();

		this.navigationPointPrefab = Resources.Load<GameObject>("Prefabs/NavigationPoint");

		this.grid = new List<NavigationPoint>();
		this.currentlyTravellingPath = new TravelPath();

		//Determine point separation based on the longer side (horizontal length)
		this.pointSeparation = Mathf.Abs(this.comShipMovement.viewportMaxX - this.comShipMovement.viewportMinX) / this.gridLength;

		this.BuildGrid();
		this.BuildBorders();
		this.PopulateAdjacentPoints();

		this.shipScanLayerMask = 1 << LayerMask.NameToLayer("NavigationPoint");
	}

	#region Grid Implementation
	private void BuildGrid(){
		for (float yCoordinate = this.comShipMovement.viewportMinY; yCoordinate <= this.comShipMovement.viewportMaxY; yCoordinate += this.pointSeparation) { 
			for (float xCoordinate = this.comShipMovement.viewportMinX; xCoordinate <= this.comShipMovement.viewportMaxX; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.comShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				//Point currentPoint = new Point(adjustedPointPosition, this.numberOfScanIterations);

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.InitializePoint(adjustedPointPosition, this.numberOfScanIterations);
				this.grid.Add(navPoint);
				//navPoint.pointReference = this.grid.Last();  //JPS  Can't I just use currentPoint?
			}
		}
	}

	private void BuildBorders()
	{
		float borderLength = this.borderWidth * this.pointSeparation;

		//Top Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float xCoordinate = this.comShipMovement.viewportMinX - borderLength; xCoordinate <= this.comShipMovement.viewportMaxX + borderLength; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.comShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, this.comShipMovement.viewportMaxY + (this.pointSeparation * (rowIndex + 1)), 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				//Point currentPoint = new Point(adjustedPointPosition, 1);
				//currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.InitializePoint(adjustedPointPosition, 1);
				navPoint.dangerScore = 1;
				//navPoint.pointReference = currentPoint;	
			}
		}

		//Bottom Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float xCoordinate = this.comShipMovement.viewportMinX - borderLength; xCoordinate <= this.comShipMovement.viewportMaxX + borderLength; xCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.comShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(xCoordinate, this.comShipMovement.viewportMinY - (this.pointSeparation * (rowIndex + 1)), 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				//Point currentPoint = new Point(adjustedPointPosition, 1);
				//currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.InitializePoint(adjustedPointPosition, 1);
				navPoint.dangerScore = 1;
				//navPoint.pointReference = currentPoint;	
			}
		}

		//Left Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float yCoordinate = this.comShipMovement.viewportMinY - borderLength; yCoordinate <= this.comShipMovement.viewportMaxY + borderLength; yCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.comShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(this.comShipMovement.viewportMinX - (this.pointSeparation * (rowIndex + 1)), yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				//Point currentPoint = new Point(adjustedPointPosition, 1);
				//currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.InitializePoint(adjustedPointPosition, 1);
				navPoint.dangerScore = 1;
				//navPoint.pointReference = currentPoint;	
			}
		}

		//Right Border
		for (float rowIndex = 0; rowIndex < this.borderWidth; rowIndex++) {
			for (float yCoordinate = this.comShipMovement.viewportMinY - borderLength; yCoordinate <= this.comShipMovement.viewportMaxY + borderLength; yCoordinate += this.pointSeparation) {

				//TODO: JPS: This sets z to -10 for some reason.  Why?
				Vector3 currentPointPosition = this.comShipMovement.renderCamera.ViewportToWorldPoint(new Vector3(this.comShipMovement.viewportMaxX + (this.pointSeparation * rowIndex), yCoordinate, 0));
				Vector3 adjustedPointPosition = new Vector3(currentPointPosition.x, currentPointPosition.y, 0);

				//Point currentPoint = new Point(adjustedPointPosition, 1);
				//currentPoint.dangerScore = 1;

				GameObject pointObject = Instantiate(this.navigationPointPrefab, adjustedPointPosition, new Quaternion()) as GameObject;
				NavigationPoint navPoint = pointObject.GetComponent<NavigationPoint>();
				navPoint.InitializePoint(adjustedPointPosition, 1);
				navPoint.dangerScore = 1;
				//navPoint.pointReference = currentPoint;	
			}
		}
	}

	private void PopulateAdjacentPoints(){
		for (int i = 0; i < this.grid.Count; i++) {
			//Left Point
			if ((i > 0) && (i % (this.gridLength) != 0)) {
				this.grid[i].adjacentPoints.Add(this.grid[i - 1]);
			}
			//Right Point
			if ((i < (this.grid.Count - 1)) && ((i + 1) % this.gridLength != 0)) {
				this.grid[i].adjacentPoints.Add(this.grid[i + 1]);
			}
			//Above Point
			if (i < (this.gridLength * this.gridLength - 1)) {
				this.grid[i].adjacentPoints.Add(this.grid[i + this.gridLength]);
			}
			//Below Point
			if (i > this.gridLength) {
				this.grid[i].adjacentPoints.Add(this.grid[i - this.gridLength]);
			}
		}
	}

	private void Scan()
	{
		//TODO: JPS: Experiment having the first pass done through OnTriggerStay/Exit, and having a generic "Weapon" interface that contains the owningPlayer.
		//Initial Pass to find bullets
		foreach (NavigationPoint currentPoint in this.grid){
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
			foreach (NavigationPoint currentPoint in this.grid) {
				//Only composite scores for points with dangerScores less than 1
				if (currentPoint.dangerScore != 1) {
					float compositeDangerScore = 0;
					//Iterate through each neighbor point and composite a score
					foreach (NavigationPoint neighborPoint in currentPoint.adjacentPoints) {
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
	#endregion

	#region Evasive State
	private IEnumerator EvadeAttacks(){
		while (this.currentAIState == AIState.Evasive) {
			TravelPath chosenPath = this.currentlyTravellingPath;

			PolarCoordinate oppositeDirection = new PolarCoordinate(this.sphereCastMagnitude, -this.currentlyTravellingPath.pathVector);
			this.startingAngle = oppositeDirection.angle + (Random.Range(-30f, 30f) * Mathf.Deg2Rad);
			this.endingAngle = this.startingAngle + (360 * Mathf.Deg2Rad);

			Debug.DrawRay(this.comShipMovement.gameObject.transform.position, oppositeDirection.PolarToCartesian().normalized*this.sphereCastMagnitude, Color.green);

			//Scan from startAngle to endAngle
			for (float i = this.startingAngle; i < this.endingAngle; i += this.shipScanDegreeSeparation) {
				PolarCoordinate scanDirection = new PolarCoordinate(this.sphereCastMagnitude, i);
				PolarCoordinate offsetPosition = /*new PolarCoordinate(3 * this.sphereCastRadius, i);*/ new PolarCoordinate(this.sphereCastRadius, Vector3.zero);
				RaycastHit[] detectedObjects = Physics.SphereCastAll(this.comShipMovement.gameObject.transform.position + offsetPosition.PolarToCartesian(), 
																	this.sphereCastRadius, scanDirection.PolarToCartesian().normalized, this.sphereCastMagnitude, this.shipScanLayerMask);
				TravelPath currentCheckedPath = new TravelPath();

				Debug.DrawRay(this.comShipMovement.gameObject.transform.position + offsetPosition.PolarToCartesian(), scanDirection.PolarToCartesian(), Color.red);

				currentCheckedPath.pathVector = scanDirection.PolarToCartesian();
				currentCheckedPath.dangerScore = this.CompositeTotalDangerScore(detectedObjects);

				//Replace chosen direction if it's determined to be safer
				if (currentCheckedPath.dangerScore < this.currentlyTravellingPath.dangerScore) {
					chosenPath.pathVector = currentCheckedPath.pathVector;
					chosenPath.dangerScore = currentCheckedPath.dangerScore;
				}
			}
				
			this.nextDirection = chosenPath.pathVector;

			yield return new WaitForFixedUpdate();
		}
	}
	#endregion

	#region Proactive State
	private IEnumerator AttackPlayer(){
		while (this.currentAIState == AIState.Attacking) {
			Vector3 chosenVector = this.DetermineAttackingDirection();
			this.nextDirection = chosenVector;
			yield return new WaitForSeconds(this.secondsBetweenAttackReposition);
		}
	}

	//Move either towards or away from the player character in the X direction, depending on which direction is least dangerous
	//Ensures that the COM still moves towards the player in the Y direction.
	private Vector3 DetermineAttackingDirection(){
		RaycastHit[] detectedObjects;
		float towardsDangerScore;
		float awayDangerScore;

		Vector3 directionVector = this.playerShipMovement.transform.position - this.comShipMovement.transform.position;

		//Don't move directly horizontally towards the player. Looks weird if the player doesn't move.
		//Offset Y slightly to avoid this.
		directionVector = new Vector3(directionVector.x, directionVector.y + Random.Range(-this.offsetYRangeForProactiveState, this.offsetYRangeForProactiveState), 0);

		Debug.DrawRay(this.comShipMovement.gameObject.transform.position, directionVector.normalized*this.sphereCastMagnitude, Color.green);

		//Towards player
		detectedObjects = Physics.SphereCastAll(this.comShipMovement.gameObject.transform.position, this.sphereCastRadius, directionVector.normalized, this.sphereCastMagnitude, this.shipScanLayerMask);
		towardsDangerScore = this.CompositeTotalDangerScore(detectedObjects);

		//Away from player
		Vector3 mirroredVector = new Vector3(-directionVector.x, directionVector.y, 0);
		detectedObjects = Physics.SphereCastAll(this.comShipMovement.gameObject.transform.position, this.sphereCastRadius, mirroredVector.normalized, this.sphereCastMagnitude, this.shipScanLayerMask);
		awayDangerScore = this.CompositeTotalDangerScore(detectedObjects);

		if (AIManager.debugMode == true) {
			Debug.DrawRay(this.comShipMovement.gameObject.transform.position, mirroredVector.normalized * this.sphereCastMagnitude, Color.green);
			Debug.LogError("DirectionVector DangerScore: " + towardsDangerScore + " MirroredVector DangerScore: " + awayDangerScore);
		}

		if (towardsDangerScore < awayDangerScore) {
			return directionVector;
		}
		else if (awayDangerScore < towardsDangerScore) {
			return mirroredVector;
		}
		else {
			return this.currentlyTravellingPath.pathVector;
		}
	}
	#endregion

	#region Pursuing Health Pack State

	#endregion

	private float CompositeTotalDangerScore(RaycastHit[] detectedObjects){
		float totalDangerScore = 0;
		foreach (RaycastHit hitInfo in detectedObjects) {
			NavigationPoint currentNavPoint = hitInfo.collider.gameObject.GetComponent<NavigationPoint>();
			totalDangerScore += currentNavPoint.dangerScore;
		}

		return totalDangerScore;
	}

	//Lerp to the correct magnitude to allow for natural-looking movement
	private void UpdateCurrentlyTravellingPathVector(){
		this.currentlyTravellingPath.pathVector = Vector3.Lerp(this.previousDirection, this.nextDirection.normalized, Time.fixedDeltaTime * this.comShipMovement.shipLerpSpeed);
		this.previousDirection = this.currentlyTravellingPath.pathVector;
	}

	private void UpdateCurrentPathDangerScore(){
		if (this.currentlyTravellingPath.pathVector == Vector3.zero) {
			return;
		}

		RaycastHit[] detectedObjects = Physics.SphereCastAll(this.comShipMovement.gameObject.transform.position, this.sphereCastRadius, this.currentlyTravellingPath.pathVector.normalized, this.sphereCastMagnitude, this.shipScanLayerMask);

		this.currentlyTravellingPath.dangerScore = this.CompositeTotalDangerScore(detectedObjects);
	}

	private void DetermineAIState(){
		Collider[] detectedObjects = Physics.OverlapSphere(new Vector3(this.comShipMovement.gameObject.transform.position.x, this.comShipMovement.gameObject.transform.position.y, 0), this.sphereCastMagnitude, this.shipScanLayerMask);

		float totalSurroundingDangerScore = 0;

		foreach (Collider pointCollider in detectedObjects) {
			NavigationPoint currentNavPoint = pointCollider.gameObject.GetComponent<NavigationPoint>();

			totalSurroundingDangerScore += currentNavPoint.dangerScore;
		}
			
		float averageSurroundingDangerScore = totalSurroundingDangerScore / (float)detectedObjects.Count();

		if ((averageSurroundingDangerScore >= this.evadeTriggerThreshold) && (this.currentAIState != AIState.Evasive)) {
			this.currentAIState = AIState.Evasive;
			StartCoroutine(this.EvadeAttacks());
		}
		else if ((averageSurroundingDangerScore < this.evadeTriggerThreshold) && (this.currentAIState != AIState.Attacking)) {
			this.currentAIState = AIState.Attacking;
			StartCoroutine(this.AttackPlayer());
		}
	}

	// Update is called once per frame
	void Update () {
		if (this.comShipMovement == null) {
			return;
		}

		Vector3 moveDirection = Vector3.zero;
		if (this.passedFrames < this.framesBetweenScans) {
			this.Scan();
			this.passedFrames += 1;
			this.UpdateCurrentPathDangerScore();
			this.DetermineAIState();
		}
		else {
			this.passedFrames = 0;
		}

		this.UpdateCurrentlyTravellingPathVector();
		this.comShipMovement.Move(this.currentlyTravellingPath.pathVector);
	}
}
