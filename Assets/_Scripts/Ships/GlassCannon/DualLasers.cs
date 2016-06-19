using UnityEngine;
using System.Collections;

public class DualLasers : MonoBehaviour {
	Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			thisPlayer = GameManager.S.players[(int)value];
			foreach (var laser in lasers) {
				laser.startColor = thisPlayer.playerColor;
			}
		}
	}
	PlayerShip thisPlayer;

	GameObject explosionPrefab;
	public ParticleSystem[] lasers;
	public BoxCollider[] hitboxes;

	float damage;
	float maxDamage = 0.45f;
	float minDamage = 0.1f;
	float useSlowingFactor = 0.4f;
	float slowingFactor = 0.35f;
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
		X = (owningPlayer == Player.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		damage = maxDamage;

		//Set this as a child of the player
		if (owningPlayer != Player.none) {
			thisPlayer = GameManager.S.players[(int)owningPlayer];
			transform.SetParent(thisPlayer.transform, false);
			transform.localPosition = Vector3.zero;
		}
		//Wait while charging
		SoundManager.instance.Play("BeamDetonate");
		for (float i = 0; i < chargeTime; i += Time.fixedDeltaTime) {
			if (Input.GetKeyUp(X)) {
				SoundManager.instance.Stop("BeamDetonate");
				yield break;	
			}
			else if (thisPlayer.device != null && thisPlayer.device.Action3.WasReleased) {
				SoundManager.instance.Stop("BeamDetonate");
				yield break;
			}

			yield return new WaitForFixedUpdate();
		}
		//yield return new WaitForSeconds(chargeTime);
		StartCoroutine(LoseEnergy());
		thisPlayer.playerShooting.ExpendAttackSlot();
		//Turn on the hitboxes
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = !hasEnded;
		}
		//Slow the player while firing the beam
		float timeFired = 0;
		while (thisPlayer != null && !hasEnded && timeFired < maxDuration) {
			timeFired += Time.deltaTime;

			thisPlayer.playerMovement.SlowPlayer(useSlowingFactor, 0.2f, true);
			thisPlayer.durationBar.SetPercent(1 - timeFired / maxDuration);

			yield return null;
		}
		EndLaserAttack();
	}

	IEnumerator LoseEnergy() {
		float timeElapsed = 0;
		SoundManager.instance.Play("DualLasers");
		while (!hasEnded) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/maxDuration;

			SoundManager.instance.SetPitch("DualLasers", 1 - percent);

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
		SoundManager.instance.Stop("DualLasers");
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
		if (owningPlayer != Player.none) {
			thisPlayer.playerMovement.RestoreSpeed();
			thisPlayer.durationBar.SetPercent(0);
		}

		Destroy(gameObject, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(X)) {
			EndLaserAttack();
		}
		if (thisPlayer.device != null) {
			if (thisPlayer.device.Action3.WasReleased) {
				EndLaserAttack();
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (player.player != owningPlayer) {
				//Do damage to the player hit
				player.TakeDamage(damage);

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
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (player.player != owningPlayer) {
				//Restore the player's speed after leaving the beam
				playerMovement.RestoreSpeed();
			}
		}
	}
}
