using UnityEngine;
using System.Collections;

public class Bullet : PooledObj {
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
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "KillZone") {
			ReturnToPool();
		}
		else if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			if (player.player != owningPlayer) {
				//Do damage to the player hit
			}
			ReturnToPool();
		}
	}
}
