using UnityEngine;
using System.Collections;

public class PreviewShip : PlayerShip {

	// Use this for initialization
	protected override void Awake () {
		playerMovement = GetComponent<ShipMovement>();
		playerShooting = GetComponent<ShootBomb>();
		shipSprite = GetComponentInChildren<SpriteRenderer>();
		healthPickupParticles = transform.FindChild("HealthPickupParticleSystem").GetComponent<ParticleSystem>();
	}

	public override void TakeDamage(float damageIn) {
		if (invincible || dead) {
			return;
		}

		if (damageIn > 0) {
			SoundManager.instance.Play("TakeDamage");
		}

		timeSinceTakenDamage = 0;
		if (!inDamageFlashCoroutine) {
			StartCoroutine(FlashOnDamage(damageIn));
		}
	}


}
