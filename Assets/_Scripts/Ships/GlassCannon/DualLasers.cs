using UnityEngine;
using System.Collections;

public class DualLasers : MonoBehaviour {
	PlayerEnum _owningPlayer = PlayerEnum.none;
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (GameManager.S.inGame) {
				thisPlayer = GameManager.S.players[(int)value];
				SetColor(thisPlayer.playerColor);
			}
		}
	}
	public Player thisPlayer;

	GameObject explosionPrefab;
	public ParticleSystem[] lasers;
	public BoxCollider[] hitboxes;

	float damage;
	float maxDamage = 0.45f;
	float minDamage = 0.1f;
	float useSlowingFactor = 0.4f;
	float slowingFactor = 0.65f;
	float chargeTime = 0.75f;
	float maxDuration = 4f;

	float maxStartSize = 2f;
	float minStartSize = 0.45f;
	float maxHitboxSize = 1.35f;
	float minHitboxSize = 0.6f;

	KeyCode X;
	bool hasEnded = false;

	// Use this for initialization
	void Awake () {
		explosionPrefab = Resources.Load<GameObject>("Prefabs/Explosion");
	}

	IEnumerator Start() {
		X = (owningPlayer == PlayerEnum.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		damage = maxDamage;

		//Set this as a child of the player
		if (thisPlayer != null) {
			transform.SetParent(thisPlayer.character.transform, false);
			transform.localPosition = Vector3.zero;
		}
		//Wait while charging
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("BeamDetonate");
		}
		for (float i = 0; i < chargeTime; i += Time.fixedDeltaTime) {
			//Only check the end conditions when we're in game (preview should fire without button press)
			if (GameManager.S.inGame) {
				if (Input.GetKeyUp(X)) {
					SoundManager.instance.Stop("BeamDetonate");
					yield break;
				}
				else if (thisPlayer.device != null && thisPlayer.device.Action3.WasReleased) {
					SoundManager.instance.Stop("BeamDetonate");
					yield break;
				}
			}

			yield return new WaitForFixedUpdate();
		}
		//yield return new WaitForSeconds(chargeTime);
		StartCoroutine(LoseEnergy());
		thisPlayer.character.ship.shooting.ExpendAttackSlot();
		//Turn on the hitboxes
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = !hasEnded;
		}
		//Slow the player while firing the beam
		float timeFired = 0;
		while (thisPlayer != null && !hasEnded && timeFired < maxDuration) {
			timeFired += Time.deltaTime;

			thisPlayer.character.ship.movement.SlowPlayer(useSlowingFactor, 0.2f, true);
			if (thisPlayer.durationBar != null) {
				thisPlayer.durationBar.SetPercent(1 - timeFired / maxDuration);
			}

			yield return null;
		}
		EndLaserAttack();
	}

	IEnumerator LoseEnergy() {
		float timeElapsed = 0;
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("DualLasers");
		}
		while (!hasEnded) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/maxDuration;

			if (GameManager.S.inGame) {
				SoundManager.instance.SetPitch("DualLasers", 1 - percent);
			}

			foreach (var laser in lasers) {
				laser.startSize = Mathf.Lerp(maxStartSize, minStartSize, percent);
			}
			foreach (var hitbox in hitboxes) {
				Vector3 size = hitbox.size;
				size.x = Mathf.Lerp(maxHitboxSize, minHitboxSize, percent);
				hitbox.size = size;
			}
			damage = Mathf.Lerp(maxDamage, minDamage, percent);

			yield return null;
		}
		if (GameManager.S.inGame) {
			SoundManager.instance.Stop("DualLasers");
		}
	}

	void EndLaserAttack() {
		if (hasEnded) {
			return;
		}
		hasEnded = true;
		print("End Laser Attack");

		//Turn off the lasers
		foreach (var laser in lasers) {
			laser.Stop();
		}
		//Remove the hitboxes
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = false;
		}
		//Restore the player's speed
		if (owningPlayer != PlayerEnum.none) {
			thisPlayer.character.ship.movement.RestoreSpeed();
			if (thisPlayer.durationBar != null) {
				thisPlayer.durationBar.SetPercent(0);
			}
		}

		Destroy(gameObject, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameManager.S.inGame) {
			return;
		}

		if (Input.GetKeyUp(X)) {
			EndLaserAttack();
		}
		if (thisPlayer.device != null) {
			if (thisPlayer.device.Action3.WasReleased) {
				EndLaserAttack();
			}
		}
		if (thisPlayer.character.dead) {
			EndLaserAttack();
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			Ship shipHit = other.gameObject.GetComponentInParent<Ship>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (shipHit.playerEnum != owningPlayer) {
				//Do damage to the player hit
				shipHit.TakeDamage(damage);

				//Slow the player while in the beam
				playerMovement.SlowPlayer(slowingFactor);

				GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
			}
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			otherShip.TakeDamage(damage);

			GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			Character player = other.gameObject.GetComponentInParent<Character>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (player.playerEnum != owningPlayer) {
				//Restore the player's speed after leaving the beam
				playerMovement.RestoreSpeed();
			}
		}
	}

	public void SetColor(Color newColor) {
		foreach (var laser in lasers) {
			laser.startColor = newColor;
		}
	}
}
