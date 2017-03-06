using UnityEngine;
using System.Collections;

public class Masochist : PlayerShip {
	KeyCode Y;

	public MasochistShield shield;

	MasochistAura curAura;
	MasochistAura auraPrefab;
	public bool shieldUp = false;

	public float damageMultiplier = 1f;

	new void Awake() {
		base.Awake();

		maxHealth = 250f;

		typeOfShip = ShipType.masochist;
		shield = Resources.Load<MasochistShield>("Prefabs/MasochistShield");
		auraPrefab = Resources.Load<MasochistAura>("Prefabs/MasochistAura");
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/MasochistShip/MShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/MasochistShip/MShipAnimationController");
	}

	new void Start() {
		base.Start();
		Y = playerEnum == PlayerEnum.player1 ? KeyCode.Alpha4 : KeyCode.Keypad4;

		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, true);

		health = maxHealth;
	}

	//Opted for a discrete method of doing a damage multiplier
	//Open to discuss the possibility of implementing a continuous method instead
	public override void TakeDamage(float damageIn) {

		base.TakeDamage(damageIn);

		//Determine the current damage multiplier
		float remainingHealthRatio = health / maxHealth;

		//50% health remaining -> 50% damage increase
		if (remainingHealthRatio <= 0.5f && damageMultiplier == 1) {
			damageMultiplier = 1.5f;
			SoundManager.instance.Play("ActivateAura", 1);
			curAura = Instantiate(auraPrefab, transform.position, new Quaternion()) as MasochistAura;
			curAura.playerShip = this;
		}
		else if (remainingHealthRatio > 0.5f && damageMultiplier == 1.5f) {
			Destroy(curAura.gameObject);
			damageMultiplier = 1f;
		}
	}

	protected override void Update() {
		base.Update();

		if (shooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}
		if (player.device != null) {
			if (player.device.Action4.WasPressed) {
				InstantiateShield();
			}
		}
		else if (player.device == null) {
			if (Input.GetKeyDown(Y)) {
				InstantiateShield();
			}
		}
	}

	void InstantiateShield() {
		if (shooting.curAmmo != 0 && !shieldUp) {
			MasochistShield newShield = Instantiate(shield, transform.position, new Quaternion()) as MasochistShield;
			newShield.transform.parent = gameObject.transform;
			newShield.thisPlayer = player;
			newShield.owningPlayer = playerEnum;
			newShield.ActivateShield();
			shooting.ExpendAttackSlot();
		}
		else {
			SoundManager.instance.Play("OutOfAmmo", 1);
		}
	}
}
