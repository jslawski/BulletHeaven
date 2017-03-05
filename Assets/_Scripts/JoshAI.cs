using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoshAI : MonoBehaviour {
	PlayerShip thisShip;
	ShipMovement movement;

	Collider hitbox;

	Vector3 position {
		get {
			return hitbox.transform.position;
		}
	}

	// Use this for initialization
	IEnumerator Start () {
		while (GameManager.S.gameState != GameStates.playing) {
			yield return null;
		}
		thisShip = GetComponent<PlayerShip>();
		movement = GetComponent<ShipMovement>();
		hitbox = GetComponentInChildren<Collider>();

		StartCoroutine(Locomotion());
	}

	#region Steering behavior
	#region Movement rules
	public float period;
	abstract class MovementRule {
		protected JoshAI ai;
		public float weight = 0f;

		public abstract Vector3 Apply();

		public MovementRule(JoshAI ai, float weight) {
			this.ai = ai;
			this.weight = weight;
		}

		protected Collider[] FindAllObjectsInPlayerZone() {
			float halfX = (ai.movement.worldSpaceMaxX - ai.movement.worldSpaceMinX) / 2f;
			float halfY = (ai.movement.worldSpaceMaxY - ai.movement.worldSpaceMinY) / 2f;
			float centerX = (ai.movement.worldSpaceMaxX + ai.movement.worldSpaceMinX) / 2f;
			float centerY = (ai.movement.worldSpaceMaxY + ai.movement.worldSpaceMinY) / 2f;
			Vector3 centerBox = new Vector3(centerX, centerY);
			Vector3 half = new Vector3(halfX, halfY, 0);
			return Physics.OverlapBox(centerBox, half);
		}
	}

	class AvoidBullets : MovementRule {
		float dangerRadius = 12f;
		Dictionary<PhysicsObj, Vector3> prevBulletsPos = new Dictionary<PhysicsObj, Vector3>();

		public AvoidBullets(JoshAI ai, float weight) : base(ai, weight) { }

		public override Vector3 Apply() {
			float amplification = 10f;

			Dictionary<PhysicsObj, Vector3> nextBulletPos = new Dictionary<PhysicsObj, Vector3>();

			//Collect nearby bullets
			PhysicsObj[] bullets = GetNearbyBullets();
			Vector3 collectiveDirection = Vector3.zero;
			//For each bullet, apply an avoidance force away from the bullet's next position
			//scaled for the distance to the bullet
			for (int i = 0; i < bullets.Length; i++) {
				Vector3 vel = bullets[i].velocity;
				Vector3 diffVector = ai.position - bullets[i].posNext;

				//Handle bullets that don't use PhysicsObj to move
				if (bullets[i].GetComponent<NonPooledBullet>() != null) {
					if (prevBulletsPos.ContainsKey(bullets[i])) {
						vel = (bullets[i].transform.position - prevBulletsPos[bullets[i]]) / ai.period;
					}
					nextBulletPos[bullets[i]] = bullets[i].transform.position;
					diffVector = ai.position - (bullets[i].transform.position + vel * ai.period);
				}

				Vector3 avoidanceDir = diffVector - Vector3.Project(diffVector, vel);
				float weight = Mathf.Pow((1f / diffVector.magnitude), 3);
				//DEBUG:
				Color color = Color.red;
				if (avoidanceDir.x < 0 && avoidanceDir.y > 0) color = Color.yellow;
				else if (avoidanceDir.x < 0 && avoidanceDir.y < 0) color = Color.green;
				else if (avoidanceDir.x > 0 && avoidanceDir.y < 0) color = Color.blue;
				Debug.DrawRay(bullets[i].transform.position, avoidanceDir * weight * amplification, color, ai.period);

				collectiveDirection += avoidanceDir * weight * amplification;
			}

			prevBulletsPos = nextBulletPos;
			return WithinUnitSphere(collectiveDirection);
		}

		private PhysicsObj[] GetNearbyBullets() {
			List<PhysicsObj> bullets = new List<PhysicsObj>();
			Collider[] nearbyObjects = Physics.OverlapSphere(ai.position, dangerRadius);
			for (int i = 0; i < nearbyObjects.Length; i++) {
				//Filter out non-bullet objects
				if (nearbyObjects[i].tag == "Bullet") {
					Bullet bullet = nearbyObjects[i].GetComponent<Bullet>();
					//Filter out bullets that belong to this player
					if (bullet == null || bullet.owningPlayer == ai.thisShip.playerEnum) {
						continue;
					}
					bullets.Add(bullet.GetComponent<PhysicsObj>());
				}
			}

			return bullets.ToArray();
		}
	}

	class MoveTowardsRandTarget : MovementRule {
		float minTargetChange = 0.5f;
		float maxTargetChange = 3f;
		float closeEnough = 1.5f;

		Vector3 curTarget;

		public MoveTowardsRandTarget(JoshAI ai, float weight) : base(ai, weight) {
			curTarget = ai.position;
			ai.StartCoroutine(FindRandomTarget());
		}

		public override Vector3 Apply() {
			Vector3 diffVec =  curTarget - ai.position;
			return (diffVec.magnitude > closeEnough) ? diffVec.normalized : Vector3.zero;
		}

		IEnumerator FindRandomTarget() {
			while (true) {
				curTarget = GetNewRandomTargetLocation();
				Debug.Log("New rand target: " + curTarget);

				float timeUntilNextChange = Random.Range(minTargetChange, maxTargetChange);
				yield return new WaitForSeconds(timeUntilNextChange);
			}
		}

		private Vector3 GetNewRandomTargetLocation() {
			Vector2 unitTarget = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
			float border = AvoidBorders.actDistance/2f;
			Vector2 scaledTarget = new Vector2(
				Mathf.Lerp(ai.movement.worldSpaceMinX + border, ai.movement.worldSpaceMaxX - border, unitTarget.x),
				Mathf.Lerp(ai.movement.worldSpaceMinY + border, ai.movement.worldSpaceMaxY - border, unitTarget.y)
			);

			print("Target: " + scaledTarget);

			return new Vector3(scaledTarget.x, scaledTarget.y, 0);
		}
	}

	class AvoidBorders : MovementRule {
		public static float actDistance = 5f;

		public AvoidBorders(JoshAI ai, float weight) : base(ai, weight) { }

		public override Vector3 Apply() {
			Vector3 avoidanceVector = Vector3.zero;
			//Left-border
			if (ai.position.x - actDistance < ai.movement.worldSpaceMinX) {
				float t = Mathf.Abs((ai.position.x - ai.movement.worldSpaceMinX - actDistance)) / actDistance;
				avoidanceVector += Vector3.Lerp(Vector3.right, Vector3.zero, t*t);
			}
			//Right-border
			else if (ai.position.x + actDistance > ai.movement.worldSpaceMaxX) {
				float t = Mathf.Abs((ai.position.x - ai.movement.worldSpaceMaxX + actDistance)) / actDistance;
				avoidanceVector += Vector3.Lerp(Vector3.left, Vector3.zero, t*t);
			}
			//Bottom-border
			if (ai.position.y - actDistance < ai.movement.worldSpaceMinY) {
				float t = Mathf.Abs((ai.position.y - ai.movement.worldSpaceMinY - actDistance)) / actDistance;
				avoidanceVector += Vector3.Lerp(Vector3.up, Vector3.zero, t*t);
			}
			//Top-border
			else if (ai.position.y + actDistance > ai.movement.worldSpaceMaxY) {
				float t = Mathf.Abs((ai.position.y - ai.movement.worldSpaceMaxY + actDistance)) / actDistance;
				avoidanceVector += Vector3.Lerp(Vector3.down, Vector3.zero, t*t);
			}

			return WithinUnitSphere(avoidanceVector);
		}
	}

	class SeekHealthPack : MovementRule {
		float seekDistance = 10f;

		public SeekHealthPack(JoshAI ai, float weight) : base(ai, weight) { }

		public override Vector3 Apply() {
			Transform[] healthPacks = FindHealthPacksInPlayerZone();
			Transform closestHealthPack = NearestHealthPack(healthPacks);

			if (closestHealthPack == null) {
				return Vector3.zero;
			}
			else {
				Vector3 seekVector = (closestHealthPack.position - ai.position);
				//Attempt to get health packs more if you're low on health, and if it's close by
				float weight = Mathf.Lerp(1, 0, ai.thisShip.health / ai.thisShip.maxHealth);
				weight /= seekVector.magnitude;
				return seekVector.normalized * weight * seekDistance;
			}
		}

		private Transform NearestHealthPack(Transform[] healthPacks) {
			float minDistance = float.MaxValue;
			Transform closestHealthPack = null;
			for (int i = 0; i < healthPacks.Length; i++) {
				float distance = Vector3.Distance(healthPacks[i].position, ai.position);
				if (distance < minDistance) {
					minDistance = distance;
					closestHealthPack = healthPacks[i];
				}
			}

			return closestHealthPack;
		}

		private Transform[] FindHealthPacksInPlayerZone() {
			Collider[] allObjects = FindAllObjectsInPlayerZone();

			List<Transform> healthPacks = new List<Transform>();
			for (int i = 0; i < allObjects.Length; i++) {
				if (allObjects[i].GetComponent<HealthPickup>() != null) {
					healthPacks.Add(allObjects[i].transform);
				}
			}

			return healthPacks.ToArray();
		}
	}

	class AvoidOtherPlayerAttacks : MovementRule {
		PlayerShip otherShip;

		public AvoidOtherPlayerAttacks(JoshAI ai, float weight) : base(ai, weight) {
			otherShip = GameManager.S.OtherPlayerShip(ai.thisShip);
		}

		public override Vector3 Apply() {
			switch (otherShip.typeOfShip) {
				case ShipType.generalist:
					return ApplyGeneralist();
				case ShipType.glassCannon:
					return ApplyGlassCannon();
				case ShipType.masochist:
					return ApplyMasochist();
				case ShipType.tank:
					return ApplyTank();
				case ShipType.vampire:
					return ApplyVampire();
				default:
					return Vector3.zero;
			}
		}

		#region Generalist-specific avoidance
		private Dictionary<Collider, Vector3> beamAvoidanceDirections = new Dictionary<Collider, Vector3>();
		private Vector3 ApplyGeneralist() {
			Collider[] allObjects = FindAllObjectsInPlayerZone();

			Collider[] xBeams = FindXBeamAttacks(allObjects);
			PhysicsObj[] bombs = FindOpponentBombs(allObjects);

			Vector3 collectiveVector = Vector3.zero;

			#region Avoid X-beam attacks
			Dictionary<Collider, Vector3> nextBeamAvoidanceDirections = new Dictionary<Collider, Vector3>();
			float tooFarAwayCutoffRange = 1.5f;
			float withinBeamCutoffRange = 0.5f;
			foreach (Collider beam in xBeams) {
				Vector3 closestPointOnBeam = beam.ClosestPointOnBounds(ai.position);
				Vector3 diffVector = ai.position - closestPointOnBeam;
				float distance = diffVector.magnitude;
				//Handle the case of being inside the beam
				if (distance < withinBeamCutoffRange) {
					Vector3 avoidanceDir = Vector3.zero;
					//If we previously determined the avoidance direction for this beam, keep using that
					if (beamAvoidanceDirections.ContainsKey(beam)) {
						avoidanceDir = beamAvoidanceDirections[beam];
					}
					else {
						//Move to whichever side has more space to avoid being trapped in
						bool horizontalBeam = (beam.bounds.extents.x > beam.bounds.extents.y);
						if (horizontalBeam) {
							float midpoint = (ai.movement.worldSpaceMaxY + ai.movement.worldSpaceMinY) / 2f;
							avoidanceDir = new Vector3(0, midpoint - closestPointOnBeam.y, 0).normalized;
						}
						else {
							float midpoint = (ai.movement.worldSpaceMaxX + ai.movement.worldSpaceMinX) / 2f;
							avoidanceDir = new Vector3(midpoint - closestPointOnBeam.x, 0, 0).normalized;
						}
					}
					nextBeamAvoidanceDirections[beam] = avoidanceDir;
					collectiveVector += avoidanceDir;
					
					continue;
				}
				if (distance > tooFarAwayCutoffRange || distance <= withinBeamCutoffRange) continue;

				float weight = Mathf.Pow(1f / Mathf.Min(1, distance), 2);
				collectiveVector += diffVector * weight;
			}
			//Update the map of avoidance directions
			beamAvoidanceDirections = nextBeamAvoidanceDirections;
			#endregion

			#region Avoid cardinal direction from bombs (avoid getting caught in a potential X-beam)
			foreach (PhysicsObj bomb in bombs) {
				Vector3 diffVector = ai.position - bomb.transform.position;
				if (diffVector.x > tooFarAwayCutoffRange && diffVector.y > tooFarAwayCutoffRange) continue;

				//Closer to x-axis than y-axis
				if (Mathf.Abs(diffVector.x) > Mathf.Abs(diffVector.y)) {
					float weight = Mathf.Pow(1f / Mathf.Abs(diffVector.y), 2);
					collectiveVector += new Vector3(0, -bomb.velocity.normalized.y * weight, 0);
				}
				//Closer to y-axis than x-axis
				else {
					float weight = Mathf.Pow(1f / Mathf.Abs(diffVector.x), 2);
					collectiveVector += new Vector3(-bomb.velocity.normalized.x * weight, 0, 0);
				}
			}
			#endregion

			return WithinUnitSphere(collectiveVector);
		}

		private Collider[] FindXBeamAttacks(Collider[] allObjects) {
			List<Collider> xBeams = new List<Collider>();
			for (int i = 0; i < allObjects.Length; i++) {
				Beam thisBeam = allObjects[i].GetComponentInParent<Beam>();
				if (thisBeam != null && thisBeam.owningPlayer != ai.thisShip.playerEnum) {
					xBeams.Add(allObjects[i]);
				}
			}

			return xBeams.ToArray();
		}

		private PhysicsObj[] FindOpponentBombs(Collider[] allObjects) {
			List<PhysicsObj> bombs = new List<PhysicsObj>();
			for (int i = 0; i < allObjects.Length; i++) {
				Bomb thisBomb = allObjects[i].GetComponent<Bomb>();
				if (thisBomb != null && thisBomb.owningPlayer != ai.thisShip.playerEnum) {
					bombs.Add(thisBomb.GetComponentInParent<PhysicsObj>());
				}
			}

			return bombs.ToArray();
		}
		#endregion
		#region Glass Cannon-specific avoidance
		private Vector3 ApplyGlassCannon() {
			Vector3 collectiveVector = Vector3.zero;
			Collider[] allObjects = FindAllObjectsInPlayerZone();

			float tooFarAwayCutoff = 1.5f;

			//Avoid Dual Laser attacks
			Collider[] lasers = FindDualLaserAttacks(allObjects);
			if (lasers.Length > 0) {
				foreach (Collider laser in lasers) {
					Vector3 diffVector = ai.position - laser.transform.position;
					if (Mathf.Abs(diffVector.y) < tooFarAwayCutoff) {
						Vector3 avoidanceDir = new Vector3(0, Mathf.Sign(diffVector.y), 0);
						float weight = Mathf.Pow(1f / Mathf.Abs(diffVector.y), 2); ;
						collectiveVector += avoidanceDir * weight;
					}
				}
			}
			else {
				//Don't stay horizontal to your opponent
				PlayerShip otherPlayerShip = GameManager.S.OtherPlayerShip(ai.thisShip);
				Vector3 diffVector = ai.position - otherPlayerShip.transform.position;

				if (Mathf.Abs(diffVector.y) < tooFarAwayCutoff) {
					float weight = Mathf.Pow(1f / Mathf.Abs(diffVector.y), 2);
					collectiveVector += new Vector3(0, -otherPlayerShip.movement.GetVelocity().normalized.y * weight, 0);
				}
			}


			return WithinUnitSphere(collectiveVector);
		}

		private Collider[] FindDualLaserAttacks(Collider[] allObjects) {
			List<Collider> lasers = new List<Collider>();
			for (int i = 0; i < allObjects.Length; i++) {
				DualLasers thisLaser = allObjects[i].GetComponentInParent<DualLasers>();
				if (thisLaser != null && thisLaser.owningPlayer != ai.thisShip.playerEnum) {
					lasers.Add(allObjects[i]);
				}
			}

			return lasers.ToArray();
		}
		#endregion
		#region Masochist-specific avoidance
		private Vector3 ApplyMasochist() {
			PlayerShip otherPlayer = GameManager.S.OtherPlayerShip(ai.thisShip);
			float amplification = Mathf.Lerp(0, 1, 1 - (otherPlayer.health / otherPlayer.maxHealth));

			Vector3 collectiveDirection = Vector3.zero;

			Collider[] allObjects = FindAllObjectsInPlayerZone();
			PhysicsObj[] bombs = FindOpponentBombs(allObjects);

			foreach (PhysicsObj bomb in bombs) {
				Vector3 vel = bomb.velocity;
				Vector3 diffVector = ai.position - bomb.posNext;

				Vector3 avoidanceDir = diffVector - Vector3.Project(diffVector, vel);
				float weight = Mathf.Pow((1f / diffVector.magnitude), 2);

				collectiveDirection += avoidanceDir * weight * amplification;
			}

			return WithinUnitSphere(collectiveDirection);
		}
		#endregion
		#region Tank-specific avoidance
		private Vector3 ApplyTank() {
			Collider[] allObjects = FindAllObjectsInPlayerZone();
			Transform[] allMines = FindAllTankMines(allObjects);

			Vector3 collectiveDirection = Vector3.zero;

			//Avoid proximity mines
			float tooFarAwayDistance = 4.5f;
			float amplification = 3f;
			foreach (Transform mine in allMines) {
				Vector3 diffVector = ai.position - mine.position;

				if (diffVector.magnitude > tooFarAwayDistance) continue;

				float weight = Mathf.Pow(1f / diffVector.magnitude, 2);
				collectiveDirection += diffVector.normalized * weight * amplification;
			}

			//Avoid center of black holes
			Transform[] blackHoles = FindAllBlackHoles(allObjects);
			tooFarAwayDistance = 3f;
			foreach (Transform blackHole in blackHoles) {
				Vector3 diffVector = ai.position - blackHole.position;

				if (diffVector.magnitude > tooFarAwayDistance) continue;
				
				float weight = Mathf.Pow(1f / diffVector.magnitude, 2);
				collectiveDirection += diffVector.normalized * weight * amplification;
			}

			return WithinUnitSphere(collectiveDirection);
		}

		private Transform[] FindAllTankMines(Collider[] allObjects) {
			List<Transform> mines = new List<Transform>();
			for (int i = 0; i < allObjects.Length; i++) {
				ProximityMine thisMine = allObjects[i].GetComponent<ProximityMine>();
				if (thisMine != null && thisMine.owningPlayer != ai.thisShip.playerEnum) {
					mines.Add(thisMine.transform);
				}
			}

			return mines.ToArray();
		}

		private Transform[] FindAllBlackHoles(Collider[] allObjects) {
			List<Transform> bHoles = new List<Transform>();
			for (int i = 0; i < allObjects.Length; i++) {
				BlackHoleInner blackHole = allObjects[i].GetComponent<BlackHoleInner>();
				if (blackHole != null && blackHole.blackHole.owningPlayer != ai.thisShip.playerEnum) {
					bHoles.Add(blackHole.transform);
				}
			}

			return bHoles.ToArray();
		}
		#endregion
		#region Vampire-specific avoidance
		private Vector3 ApplyVampire() {
			Collider[] allObjects = FindAllObjectsInPlayerZone();
			SphereCollider[] lifeSapZones = FindAllLifeSapZones(allObjects);

			Vector3 collectiveDirection = Vector3.zero;

			float amplification = 1000;

			foreach (SphereCollider zone in lifeSapZones) {
				float zoneSize = zone.radius;
				float tooFarAwayCutoff = zoneSize + 2f;
				Vector3 diffVector = ai.position - zone.transform.position;

				if (diffVector.magnitude > tooFarAwayCutoff) continue;

				float weight = 1f / diffVector.magnitude;
				collectiveDirection += diffVector.normalized * weight * amplification;
			}

			return WithinUnitSphere(collectiveDirection);
		}

		private SphereCollider[] FindAllLifeSapZones(Collider[] allObjects) {
			List<SphereCollider> lifeSapZones = new List<SphereCollider>();
			for (int i = 0; i < allObjects.Length; i++) {
				LifeSapZone lifeSapZone = allObjects[i].GetComponentInParent<LifeSapZone>();
				if (lifeSapZone != null && lifeSapZone.owningPlayer != ai.thisShip.playerEnum) {
					lifeSapZones.Add((SphereCollider)allObjects[i]);
				}
			}

			return lifeSapZones.ToArray();
		}
		#endregion

	}
	#endregion

	IEnumerator Locomotion() {
		period = 1f * Time.fixedDeltaTime;
		float timeUntilNextMovementCalc = 0;

		MovementRule[] rules = new MovementRule[] {
			new AvoidBullets(this, 6f),
			new MoveTowardsRandTarget(this, 1f),
			new AvoidBorders(this, 4f),
			new SeekHealthPack(this, 8f),
			new AvoidOtherPlayerAttacks(this, 8f)
		};

		while (GameManager.S.gameState == GameStates.playing) {
			//Add different influences for movement direction
			Vector3 moveDirection = Vector3.zero;
			for (int i = 0; i < rules.Length; i++) {
				moveDirection += rules[i].Apply() * rules[i].weight;
			}

			//Reapply last movement vector every frame until we need to recalculate
			while (timeUntilNextMovementCalc > 0) {
				timeUntilNextMovementCalc -= Time.deltaTime;
				movement.Move(WithinUnitSphere(moveDirection));

				yield return null;
			}
			timeUntilNextMovementCalc = period;
		}
	}

	static Vector3 WithinUnitSphere(Vector3 inVector) {
		return (inVector.magnitude > 1) ? inVector.normalized : inVector;
	}


	#endregion
	#region Attacking behavior


	#endregion
}
