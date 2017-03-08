using UnityEngine;
using System.Collections;

//TODO 3/6/17: Either fix this to work for characters with multiple ships or scrap it altogether and come up with a better way to preview abilities
public class PreviewGameManager : MonoBehaviour {
	public PlayerEnum sceneOwner = PlayerEnum.none;
	public Ship target;
	public PreviewCharacter[] players;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayAbilityPreview(int slot) {
		players[(int)sceneOwner].ApplyToAllShips(ship => ship.shooting.DestroyAllBombs());
		target.shooting.DestroyAllBombs();
		StopAllCoroutines();
		switch (players[(int)sceneOwner].characterType) {
			//Generalist ship
			case CharactersEnum.generalist:	
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
			
			//Glass Cannon ship
			case CharactersEnum.glassCannon:
				switch (slot) {
					case 0:
						StartCoroutine(GlassCannon0Preview());
						break;
					case 1:
						StartCoroutine(GlassCannon1Preview());
						break;
					case 2:
						StartCoroutine(GlassCannon2Preview());
						break;
					case 3:
						StartCoroutine(GlassCannon3Preview());
						break;
				}
				break;

			//Masochist ship
			case CharactersEnum.masochist:
				switch (slot) {
					case 0:
						StartCoroutine(Masochist0Preview());
						break;
					case 1:
						StartCoroutine(Masochist1Preview());
						break;
					case 2:
						StartCoroutine(Masochist2Preview());
						break;
					case 3:
						StartCoroutine(Masochist3Preview());
						break;
				}
				break;
			
			//Tank ship
			case CharactersEnum.tank:
				switch (slot) {
					case 0:
						StartCoroutine(TankShip0Preview());
						break;
					case 1:
						StartCoroutine(TankShip1Preview());
						break;
					case 2:
						StartCoroutine(TankShip2Preview());
						break;
					case 3:
						StartCoroutine(TankShip3Preview());
						break;
				}
				break;

			//Vampire ship
			case CharactersEnum.vampire:
				switch (slot) {
					case 0:
						StartCoroutine(Vampire0Preview());
						break;
					case 1:
						StartCoroutine(Vampire1Preview());
						break;
					case 2:
						StartCoroutine(Vampire2Preview());
						break;
					case 3:
						StartCoroutine(Vampire3Preview());
						break;
				}
				break;
			default:
				Debug.LogError("Ship type " + players[(int)sceneOwner].characterType + " not handled in PreviewGameManager");
				return;
		}
	}

	//Generalist Ability Previews
	IEnumerator Generalist0Preview() {
		float startDelay = 0.25f;
		float repeatTime = 3f;
		float detonateDelay = 1f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist0Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist1Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;
		float detonateDelay = 1.5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist1Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist2Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;
		float detonateDelay = 1f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist2Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.X);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Generalist3Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist3Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			target.shooting.Shoot();
			yield return new WaitForSeconds(0.75f);
			target.shooting.DetonateBomb(AttackButtons.A);
			yield return new WaitForSeconds(0.15f);
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(0.5f);
			player.ship.shooting.DetonateBomb(AttackButtons.Y);

			yield return new WaitForSeconds(repeatTime);
		}
	}

	//Glass Cannon Ability Previews
	IEnumerator GlassCannon0Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 0.75f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("GlassCannon0Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator GlassCannon1Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 1.75f;
		float repeatTime = 7f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;

		//print("GlassCannon1Preview");
		Character player = players[(int)sceneOwner];
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator GlassCannon2Preview() {
		float startDelay = 0.25f;
		float sustainTime = 5f;
		float repeatTime = 1f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = false;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = true;

		//print("GlassCannon2Preview");
		PreviewShip player = players[(int)sceneOwner].ship as PreviewShip;

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.FireDualLasers();

			yield return new WaitForSeconds(sustainTime + repeatTime);
		}
	}
	IEnumerator GlassCannon3Preview() {
		float startDelay = 0.25f;
		float chargeTime = 3f;
		float repeatTime = 1f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = false;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("GlassCannon3Preview");
		PreviewShip player = players[(int)sceneOwner].ship as PreviewShip;

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.FireChargeShot();

			yield return new WaitForSeconds(chargeTime + repeatTime);
		}
	}

	//Masochist Ability Previews
	IEnumerator Masochist0Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 1.4f;
		float repeatTime = 4f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist0Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Masochist1Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 2.5f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist1Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Masochist2Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 3.5f;
		float repeatTime = 2f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = false;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist2Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.X);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Masochist3Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist3Preview");
		PreviewShip player = players[(int)sceneOwner].ship as PreviewShip;

		yield return new WaitForSeconds(startDelay);
		while (true) {
			playerMove.autoMove = false;
			target.shooting.Shoot();
			yield return new WaitForSeconds(1.4f);
			target.shooting.DetonateBomb(AttackButtons.A);
			yield return new WaitForSeconds(1.65f);
			player.UseMasochistShield();
			yield return new WaitForSeconds(1.5f);
			playerMove.autoMove = true;
			yield return new WaitForSeconds(repeatTime);
		}
	}

	//Tank Ship Ability Previews
	IEnumerator TankShip0Preview() {
		float startDelay = 0.25f;
		float repeatTime = 3f;
		float detonateDelay = 1f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Tank0Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator TankShip1Preview() {
		float startDelay = 0.25f;
		float repeatTime = 6.5f;
		float detonateDelay = 2f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Tank1Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator TankShip2Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 3f;
		float repeatTime = 2f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Tank2Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.X);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator TankShip3Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist3Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			target.shooting.Shoot();
			yield return new WaitForSeconds(1f);
			target.shooting.DetonateBomb(AttackButtons.B);
			yield return new WaitForSeconds(0.15f);
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(1f);
			player.ship.shooting.DetonateBomb(AttackButtons.Y);

			yield return new WaitForSeconds(repeatTime);
		}
	}

	//Vampire Ability Previews
	IEnumerator Vampire0Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 1.5f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("GlassCannon0Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.A);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Vampire1Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;
		float detonateDelay = 1.5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Generalist1Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();
			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.B);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Vampire2Preview() {
		float startDelay = 0.25f;
		float detonateDelay = 2.75f;
		float repeatTime = 6f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist2Preview");
		Character player = players[(int)sceneOwner];

		yield return new WaitForSeconds(startDelay);
		while (true) {
			player.ship.shooting.Shoot();

			yield return new WaitForSeconds(detonateDelay);

			player.ship.shooting.DetonateBomb(AttackButtons.X);

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator Vampire3Preview() {
		float startDelay = 0.25f;
		float repeatTime = 5f;

		PreviewShipMovement targetMove = target.movement as PreviewShipMovement;
		targetMove.autoMove = true;
		PreviewShipMovement playerMove = players[(int)sceneOwner].ship.movement as PreviewShipMovement;
		playerMove.autoMove = false;

		//print("Masochist3Preview");
		PreviewShip player = players[(int)sceneOwner].ship as PreviewShip;

		yield return new WaitForSeconds(startDelay);
		while (true) {
			playerMove.autoMove = false;
			target.shooting.Shoot();
			yield return new WaitForSeconds(1.4f);
			target.shooting.DetonateBomb(AttackButtons.A);
			yield return new WaitForSeconds(1.65f);
			player.UseVampireShield();
			yield return new WaitForSeconds(repeatTime);
		}
	}
}
