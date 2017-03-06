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

			if (playerHit.playerEnum != owningPlayer) {
				//Masochists with the shield up are immune to incoming bullets
				if (playerHit.typeOfShip == ShipType.masochist) {
					Masochist masochistHit = playerHit as Masochist;
					if (masochistHit.shieldUp) {
						return;
					}
				}

				//Do damage to the player hit
				if (GameManager.S.inGame && owningPlayer != PlayerEnum.none && GameManager.S.players[(int)owningPlayer].ship is Masochist) {  //kinky...
					Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer].ship as Masochist;
					playerHit.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
				}
				else {
					playerHit.TakeDamage(damage);
				}

				GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				curState = BulletState.none;
				Destroy(gameObject);
			}
			//If the bullet was absorbed by the vampire with it's shield up, heal slightly instead of doing damage
			else if (owningPlayer != PlayerEnum.none && thisPlayer.ship.typeOfShip == ShipType.vampire) {
				if (curState == BulletState.absorbedByVampire) {
					curState = BulletState.none;
					playerHit.TakeDamage(damage * -vampShieldHealAmount);
					Destroy(gameObject);
				}
			}
		}
		//Deal damage to any ProtagShip hit
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			if (GameManager.S.inGame && owningPlayer != PlayerEnum.none && GameManager.S.players[(int)owningPlayer].ship is Masochist) {
				Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer].ship as Masochist;
				otherShip.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
			}
			else {
				otherShip.TakeDamage(damage);
			}

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
			curState = BulletState.none;
			Destroy(gameObject);
		}
	}
}
