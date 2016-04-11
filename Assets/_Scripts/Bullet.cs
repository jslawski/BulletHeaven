using UnityEngine;
using System.Collections;

public class Bullet : PooledObj {
	public GameObject explosionPrefab;
	public float damage;

	SpriteRenderer sprite;
	Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				sprite.color = GameManager.S.players[(int)value].playerColor;
				owningPlayerMovement = GameManager.S.players[(int)value].playerMovement;
			}
		}
	}
	bool transparent = false;
	float transparencyAlpha = 71f/255f;
	ShipMovement owningPlayerMovement;

	void Awake() {
		sprite = GetComponent<SpriteRenderer>();
		damage = 1;
	}

	void Update() {
		if (!transparent && InOwnPlayersTerritory()) {
			SetTransparency(true);
		}
		else if (transparent && !InOwnPlayersTerritory()) {
			SetTransparency(false);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "KillZone") {
			ReturnToPool();
		}
		else if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			if (player.player != owningPlayer) {
				//Do damage to the player hit
				player.TakeDamage(damage);

				GameObject explosion = Instantiate(explosionPrefab, transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				ReturnToPool();
			}
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			otherShip.TakeDamage(damage);

			GameObject explosion = Instantiate(explosionPrefab, transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
			ReturnToPool();
		}
	}

	void SetTransparency(bool isTransparent) {
		transparent = isTransparent;

		if (transparent) {
			Color curColor = sprite.color;
			curColor.a = transparencyAlpha;
			sprite.color = curColor;
		}
		else {
			Color curColor = sprite.color;
			curColor.a = 1;
			sprite.color = curColor;
		}
	}

	bool InOwnPlayersTerritory() {
		Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
		//Player1 check
		if (owningPlayer == Player.player1) {
			if (viewportPos.x < owningPlayerMovement.viewportMaxX) {
				return true;
			}
		}
		//Player2 check
		else if (owningPlayer == Player.player2) {
			if (viewportPos.x > owningPlayerMovement.viewportMinX) {
				return true;
			}
		}
		return false;
	}
}
