using UnityEngine;
using System.Collections;

public class Bullet : PooledObj {
	public GameObject explosionPrefab;
	public float damage;

	float transparencyCheckCooldown = 0.4f;

	SpriteRenderer sprite;
	public PhysicsObj physics;
	public SphereCollider hitbox;
	Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				PlayerShip playerShip = GameManager.S.players[(int)value];
                sprite.color = playerShip.playerColor;
				owningPlayerMovement = playerShip.playerMovement;
				Player other = (value == Player.player1) ? Player.player2 : Player.player1;
				otherPlayer = GameManager.S.players[(int)other].playerMovement;
			}
		}
	}
	ShipMovement otherPlayer;
	bool _transparent = false;
	bool transparent {
		get {
			return _transparent;
		}
		set {
			SetTransparency(value);
		}
	}

	public bool absorbedByMasochist = false;

	float transparencyAlpha = 71f/255f;
	ShipMovement owningPlayerMovement;

	void Awake() {
		physics = GetComponent<PhysicsObj>();
		sprite = GetComponent<SpriteRenderer>();
		hitbox = GetComponent<SphereCollider>();
		damage = 1;
	}

	void OnEnable() {
		physics.acceleration = Vector3.zero;
		physics.velocity = Vector3.zero;
		transparent = false;
		Invoke("StartTransparencyCheckCoroutine", 0.02f);
	}
	void StartTransparencyCheckCoroutine() {
		if (gameObject.activeSelf) {
			StartCoroutine(TransparencyCheck());
		}
	}

	IEnumerator TransparencyCheck() {
		while (true) {
			if (!sprite.isVisible) {
				ReturnToPool();
				yield break;
			}

			if (!transparent && InOwnPlayersTerritory() && !absorbedByMasochist) {
				SetTransparency(true);
			}
			else if (transparent && !InOwnPlayersTerritory()) {
				SetTransparency(false);
			}

			yield return new WaitForSeconds(transparencyCheckCooldown);
		}
	}

	void OnTriggerEnter(Collider other) {
		//Destroy bullets upon hitting a killzone
		if (other.tag == "KillZone") {
			ReturnToPool();
		}
		//Deal damage to any other player hit
		else if (other.tag == "Player") {
			PlayerShip playerHit = other.gameObject.GetComponentInParent<PlayerShip>();

			if (playerHit.player != owningPlayer) {
				//Masochists with the shield up are immune to incoming bullets
				if (playerHit.typeOfShip == ShipType.masochist) {
					Masochist masochistHit = playerHit as Masochist;
					if (masochistHit.shieldUp) {
						return;
					}
				}

				//Do damage to the player hit
				if (owningPlayer != Player.none && GameManager.S.players[(int)owningPlayer] is Masochist) {  //kinky...
					Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
					playerHit.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
				}
				else {
					playerHit.TakeDamage(damage);
				}

				GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				ReturnToPool();
			}
		}
		//Deal damage to any ProtagShip hit
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			if (owningPlayer != Player.none && GameManager.S.players[(int)owningPlayer] is Masochist) {
				Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
				otherShip.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
			}
			else {
				otherShip.TakeDamage(damage);
			}

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
			ReturnToPool();
		}
	}

	void SetTransparency(bool isTransparent) {
		_transparent = isTransparent;

		if (transparent) {
			Color curColor = sprite.color;
			curColor.a = transparencyAlpha;
			sprite.color = curColor;
			sprite.sortingOrder = -1;
		}
		else {
			Color curColor = sprite.color;
			curColor.a = 1;
			sprite.color = curColor;
			sprite.sortingOrder = 0;
		}
	}

	bool InOwnPlayersTerritory() {
		//Don't do this check on the title screen
		if (Application.loadedLevelName == GameManager.S.titleSceneName) {
			return false;
		}

		//Player1 check
		if (owningPlayer == Player.player1) {
			if (transform.position.x < otherPlayer.worldSpaceMinX) {
				return true;
			}
		}
		//Player2 check
		else if (owningPlayer == Player.player2) {
			if (transform.position.x > otherPlayer.worldSpaceMaxX) {
				return true;
			}
		}
		return false;
	}
}
