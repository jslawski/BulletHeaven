using UnityEngine;
using System.Collections;

public class TitleScreenBomb : Bomb {

	public GameObject[] attackPrefabs;
	public GameObject shockwavePrefab;

	public override void Detonate(AttackButtons attackToPerform) {
		//Ignore the attackToPerform, and just pick a random attack to instantiate
		int attackIndex = Random.Range(0, attackPrefabs.Length);

		//Execute attack
		GameObject thisPrefab = Instantiate(attackPrefabs[attackIndex], transform.position, new Quaternion()) as GameObject;
		BombAttack thisAttack = thisPrefab.GetComponent<BombAttack>();

		thisAttack.owningPlayer = owningPlayer;
		thisAttack.FireBurst();

		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}
}
