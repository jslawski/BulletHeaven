using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class LeadingShot : MonoBehaviour, BombAttack {
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	public Transform target;
	public int bulletsPerBurst = 100;
	float spread = 4.5f;
	float spreadIncrementPerBullet = 1.5f;
	public Bullet bulletPrefab;

	bool inCoroutine = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A) && !inCoroutine) {
			FireBurst();
		}
	}

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == Player.none) {
			Debug.LogError("Leading shot does not have owning player set");
			yield break;
		}
		inCoroutine = true;

		Player targetPlayer = (owningPlayer == Player.player1) ? Player.player2 : Player.player1;
		target = GameManager.S.players[(int)targetPlayer].transform;

		PolarCoordinate startDirection = new PolarCoordinate(1, target.position - gameObject.transform.position);
		PolarCoordinate curDirection = new PolarCoordinate(startDirection.radius, startDirection.angle);

		float degreeOfSpread =  spread * Mathf.Deg2Rad;

		float degreeIncrement = spreadIncrementPerBullet * Mathf.Deg2Rad;

		

		int degreeScalar = 1;
		float distanceToPlayer = (target.position - transform.position).magnitude;
		//Leads more when the explosion happens closer to the player, less when exploded far away
		float leadingAmount = 0;// Mathf.Lerp(0.1f, 0f, Mathf.InverseLerp(4, 20, distanceToPlayer));

		Vector3 targetPlayerVelocity = Vector3.zero;
		//Don't try to lead velocity on the title screen
		if (GameManager.S.gameState != GameStates.titleScreen) {
			targetPlayerVelocity = leadingAmount * GameManager.S.players[(int)targetPlayer].playerMovement.GetVelocity();
		}

		for (int i = 0; i < bulletsPerBurst; i++) {
			if (Mathf.Abs(startDirection.angle - curDirection.angle) > degreeOfSpread) {
				degreeScalar *= -1;
			}

			float sprayRange = 0.45f;
			Vector3 sprayVector = new Vector3(Random.Range(-sprayRange, sprayRange), Random.Range(-sprayRange, sprayRange), 0);

			Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
			curBullet.owningPlayer = owningPlayer;
			curBullet.transform.position = gameObject.transform.position;
			//GameObject curBullet = Instantiate(bulletPrefab, gameObject.transform.position, new Quaternion()) as GameObject;
			curBullet.GetComponent<PhysicsObj>().velocity = 10*(curDirection.PolarToCartesian().normalized + targetPlayerVelocity + sprayVector).normalized;
			curDirection.angle += degreeIncrement * degreeScalar;

			yield return new WaitForFixedUpdate();
		}

		inCoroutine = false;

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}
}
