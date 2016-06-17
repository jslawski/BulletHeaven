using UnityEngine;
using System.Collections;

public class NonPooledBullet : Bullet {
	protected override void OnTriggerEnter(Collider other) {
		//Destroy bullets upon hitting a killzone
		if (other.tag == "KillZone") {
			Destroy(gameObject);
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
				Destroy(gameObject);
			}
			//If the bullet was absorbed by the vampire with it's shield up, heal slightly instead of doing damage
			else if (owningPlayer != Player.none && GameManager.S.players[(int)owningPlayer] is VampireShip) {
				VampireShip vampireOwningPlayer = GameManager.S.players[(int)owningPlayer] as VampireShip;
				if (absorbedByVampire == true) {
					absorbedByVampire = false;
					damage *= -0.25f;
					playerHit.TakeDamage(damage);
					Destroy(gameObject);
				}
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
			Destroy(gameObject);
		}
	}
}
