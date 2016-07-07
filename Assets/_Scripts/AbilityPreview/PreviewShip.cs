using UnityEngine;
using System.Collections;

public class PreviewShip : PlayerShip {
	bool inFireChargeShotCoroutine = false;
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;
	MasochistShield masochistShieldPrefab;
	VampireShield vampireShieldPrefab;

	// Use this for initialization
	protected override void Awake () {
		dualLaserPrefab = Resources.Load<DualLasers>("Prefabs/DualLasers");
		chargeShotPrefab = Resources.Load<ChargeShot>("Prefabs/ChargeShot");
		masochistShieldPrefab = Resources.Load<MasochistShield>("Prefabs/MasochistShield");
		vampireShieldPrefab = Resources.Load<VampireShield>("Prefabs/VampireShield");

		playerMovement = GetComponent<ShipMovement>();
		playerShooting = GetComponent<ShootBomb>();
		shipSprite = GetComponentInChildren<SpriteRenderer>();
		healthPickupParticles = transform.FindChild("HealthPickupParticleSystem").GetComponent<ParticleSystem>();
	}

	public override void TakeDamage(float damageIn) {
		if (invincible || dead) {
			return;
		}

		timeSinceTakenDamage = 0;
		if (!inDamageFlashCoroutine) {
			StartCoroutine(FlashOnDamage(damageIn));
		}
	}

	public void InitializeShip(ShipInfo shipInfo) {
		print("Start initializing " + shipInfo.selectingPlayer);
		player = shipInfo.selectingPlayer;
		typeOfShip = shipInfo.typeOfShip;
		playerColor = shipInfo.shipColor;

		SetSprite();

		//Set up the hitboxes appropriately
		SphereCollider hitbox = GetComponentInChildren<SphereCollider>();
		hitbox.radius = shipInfo.hitBoxRadius;
		Vector3 hitboxPos = hitbox.transform.localPosition;
		hitboxPos.y = shipInfo.hitBoxOffset;
		hitbox.transform.localPosition = hitboxPos;

		//Set the type of bomb and fix old references
		playerShooting.SetBombType(typeOfShip);
	}

	void SetSprite() {
		switch (typeOfShip) {
			case ShipType.generalist:
				GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GeneralistShip/GShip6");
				GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
					"Images/GeneralistShip/GShipAnimationController");
				break;
			case ShipType.glassCannon:
				GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GlassCannonShip/GCShip6");
				GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
					"Images/GlassCannonShip/GCShipAnimationController");
				break;
			case ShipType.masochist:
				GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/MasochistShip/MShip6");
				GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
					"Images/MasochistShip/MShipAnimationController");
				break;
			case ShipType.tank:
				GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/TankyShip/TShip6");
				GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
					"Images/TankyShip/TShipAnimationController");
				break;
			case ShipType.vampire:
				GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/VampireShip/VampireShip6");
				GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
					"Images/VampireShip/VampireShipAnimationController");
				break;
			default:
				Debug.LogError("ShipType " + typeOfShip + " not handled in SetSprite()");
				return;
		}
	}

	public void FireDualLasers() {
		DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
		dualLaser.owningPlayer = player;
		dualLaser.SetColor(playerColor);
		dualLaser.thisPlayer = this;
	}

	public void FireChargeShot() {
		if (!inFireChargeShotCoroutine) {
			StartCoroutine(FireChargeShotCoroutine());
		}
	}
	IEnumerator FireChargeShotCoroutine() {
		inFireChargeShotCoroutine = true;
		ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
		chargeShot.owningPlayer = player;
		chargeShot.player = this;

		yield return new WaitForSeconds(3f);

		chargeShot.Fire();

		inFireChargeShotCoroutine = false;
	}

	public void UseMasochistShield() {
		MasochistShield newShield = Instantiate(masochistShieldPrefab, transform.position, new Quaternion()) as MasochistShield;
		newShield.transform.parent = gameObject.transform;
		newShield.thisPlayer = this;
		newShield.owningPlayer = player;
		newShield.ActivateShield();
	}
	
	public void UseVampireShield() {
		VampireShield newShield = Instantiate(vampireShieldPrefab, transform.position, new Quaternion()) as VampireShield;
		newShield.transform.parent = gameObject.transform;
		newShield.thisPlayer = this;
		newShield.hitboxOffset = transform.FindChild("Hitbox").localPosition.y;
		newShield.owningPlayer = player;
		newShield.ActivateShield();
	}
}
