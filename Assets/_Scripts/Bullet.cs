using UnityEngine;
using System.Collections;

public class Bullet : PooledObj {
	public GameObject explosionPrefab;
	public float damage;

	float transparencyCheckCooldown = 0.4f;

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
	float transparencyAlpha = 71f/255f;
	ShipMovement owningPlayerMovement;

	void Awake() {
		sprite = GetComponent<SpriteRenderer>();
		damage = 1;
	}

	void OnEnable() {
		transparent = false;
		Invoke("StartTransparencyCheckCoroutine", 0.02f);
	}
	void StartTransparencyCheckCoroutine() {
		StartCoroutine(TransparencyCheck());
	}

	IEnumerator TransparencyCheck() {
		while (true) {
			if (!sprite.isVisible) {
				ReturnToPool();
				yield break;
			}

			if (!transparent && InOwnPlayersTerritory()) {
				SetTransparency(true);
			}
			else if (transparent && !InOwnPlayersTerritory()) {
				SetTransparency(false);
			}

			yield return new WaitForSeconds(transparencyCheckCooldown);
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

				GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				ReturnToPool();
			}
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			otherShip.TakeDamage(damage);

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
