using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class LeadingShot : MonoBehaviour {

	public Transform target;
	public int bulletsPerBurst = 100;
	public GameObject bulletPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			StartCoroutine("FireBurst");
		}

		PolarCoordinate debugDirection = new PolarCoordinate(1, target.position - gameObject.transform.position);

		//Debug.DrawRay(gameObject.transform.position, 5 * debugDirection.PolarToCartesian(), Color.blue);
	}

	IEnumerator FireBurst() {
		PolarCoordinate startDirection = new PolarCoordinate(1, target.position - gameObject.transform.position);
		PolarCoordinate curDirection = new PolarCoordinate(startDirection.radius, startDirection.angle);

		//Debug.DrawRay(gameObject.transform.position, 5 * curDirection.PolarToCartesian(), Color.red, 10);

		float degreeOfSpread =  9f * Mathf.Deg2Rad;

		float degreeIncrement = 3f * Mathf.Deg2Rad;

		int degreeScalar = 1;

		for (int i = 0; i < bulletsPerBurst; i++) {
			if (Mathf.Abs(startDirection.angle - curDirection.angle) > degreeOfSpread) {
				degreeScalar *= -1;
			}

			GameObject curBullet = Instantiate(bulletPrefab, gameObject.transform.position, new Quaternion()) as GameObject;
			curBullet.GetComponent<Rigidbody>().velocity = 10 * curDirection.PolarToCartesian().normalized;
			curDirection.angle += degreeIncrement * degreeScalar;

			yield return new WaitForFixedUpdate();
		}


	}
}
