using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class LeadingShot : MonoBehaviour {
	public Player owningPlayer = Player.none;

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

		//PolarCoordinate debugDirection = new PolarCoordinate(1, target.position - gameObject.transform.position);

		//Debug.DrawRay(gameObject.transform.position, 5 * debugDirection.PolarToCartesian(), Color.blue);
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

		//Debug.DrawRay(gameObject.transform.position, 5 * curDirection.PolarToCartesian(), Color.red, 10);

		float degreeOfSpread =  spread * Mathf.Deg2Rad;

		float degreeIncrement = spreadIncrementPerBullet * Mathf.Deg2Rad;

		int degreeScalar = 1;

		for (int i = 0; i < bulletsPerBurst; i++) {
			if (Mathf.Abs(startDirection.angle - curDirection.angle) > degreeOfSpread) {
				degreeScalar *= -1;
			}

			Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
			curBullet.owningPlayer = owningPlayer;
			curBullet.transform.position = gameObject.transform.position;
			//GameObject curBullet = Instantiate(bulletPrefab, gameObject.transform.position, new Quaternion()) as GameObject;
			curBullet.GetComponent<Rigidbody>().velocity = 10 * curDirection.PolarToCartesian().normalized;
			curDirection.angle += degreeIncrement * degreeScalar;

			yield return new WaitForFixedUpdate();
		}

		inCoroutine = false;

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}
}
