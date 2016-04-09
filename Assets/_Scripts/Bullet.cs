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
			}
		}
	}

	void Awake() {
		sprite = GetComponent<SpriteRenderer>();
		damage = 1;
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
}
