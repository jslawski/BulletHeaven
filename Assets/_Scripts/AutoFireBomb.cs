using UnityEngine;
using System.Collections;

public class AutoFireBomb : MonoBehaviour {

	PlayerShip thisPlayer;
	public Bomb bombPrefab;
	Bomb curBomb;

	float bombSpread = 0.1f;

	float minSpeed = 15f;
	float maxSpeed = 18;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<PlayerShip>();
		StartCoroutine(FireRandomBombs());
	}
	
	IEnumerator FireRandomBombs() {
		yield return new WaitForSeconds(Random.Range(5, 10));
		FireBomb();
		StartCoroutine(DetonateBomb());
		StartCoroutine(FireRandomBombs());
	}

	IEnumerator DetonateBomb() {
		yield return new WaitForSeconds(Random.Range(1, 4));
		curBomb.Detonate((Attack)Random.Range(0, 4));
	}

	void FireBomb() {
		curBomb = Instantiate(bombPrefab, transform.position, new Quaternion()) as Bomb;
		PhysicsObj bombPhysics = curBomb.GetComponent<PhysicsObj>();

		//Set the owner of the fired bomb
		curBomb.owningPlayer = thisPlayer.player;

		//Set the initial speed of the fired bomb
		float speed = Random.Range(minSpeed, maxSpeed);
		Vector3 aimDirection = ShootBomb.ApplySpread(transform.up, bombSpread);
		bombPhysics.velocity = speed * aimDirection;
	}
}
