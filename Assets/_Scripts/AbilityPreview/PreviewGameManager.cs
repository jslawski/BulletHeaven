using UnityEngine;
using System.Collections;

public class PreviewGameManager : MonoBehaviour {
	public Player sceneOwner = Player.none;
	public PlayerShip target;
	public PreviewShip[] players;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayAbilityPreview(int slot) {
		players[(int)sceneOwner].playerShooting.DestroyAllBombs();
		target.playerShooting.DestroyAllBombs();
		StopAllCoroutines();
		switch (players[(int)sceneOwner].typeOfShip) {
			//Generalist ship
			case ShipType.generalist:
				switch (slot) {
					case 0:
						StartCoroutine(Generalist0Preview());
						break;
					case 1:
						StartCoroutine(Generalist1Preview());
						break;
					case 2:
						StartCoroutine(Generalist2Preview());
						break;
					case 3:
						StartCoroutine(Generalist3Preview());
						break;
				}
				break;
		}
	}

	IEnumerator Generalist0Preview() {
		float startDelay = 0.25f;
		float repeatTime = 3f;
		float detonateDelay = 1f;

		print("Generalist0Preview");
		PlayerShip player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.playerShooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.playerShooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist1Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;
		float detonateDelay = 1.5f;

		print("Generalist1Preview");
		PlayerShip player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.playerShooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.playerShooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist2Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;
		float detonateDelay = 1f;

		print("Generalist2Preview");
		PlayerShip player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.playerShooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.playerShooting.DetonateBomb(AttackButtons.X);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist3Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;

		print("Generalist3Preview");
		PlayerShip player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			target.playerShooting.Shoot();
			yield return new WaitForSeconds(0.75f);
			target.playerShooting.DetonateBomb(AttackButtons.A);
			yield return new WaitForSeconds(0.15f);
			player.playerShooting.Shoot();
			yield return new WaitForSeconds(0.5f);
			player.playerShooting.DetonateBomb(AttackButtons.Y);

			yield return new WaitForSeconds(repeatTime);
		}
	}
}
