using UnityEngine;
using System.Collections;

public class GlassCannonBomb : Bomb {
	GameObject shockwavePrefab;
	AltCircleShot altCircleShotPrefab;
	HomingGroupShot homingGroupShotPrefab;

	new void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		altCircleShotPrefab = Resources.Load<AltCircleShot>("Prefabs/AltCircleShot");
		homingGroupShotPrefab = Resources.Load<HomingGroupShot>("Prefabs/HomingGroupShot");
	}

	void Start() {
		if (buttonUI != null) {
			buttonUI.SetButtons(true, true, false, false);
		}
	}

	public override void Detonate(AttackButtons attackToPerform) {
		switch (attackToPerform) {
			//Homing group shot
			case AttackButtons.A:
				HomingGroupShot homingGroupShot = Instantiate(homingGroupShotPrefab, transform.position, new Quaternion()) as HomingGroupShot;
				homingGroupShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					homingGroupShot.target = thisPlayer.otherPlayer.character.transform;
					homingGroupShot.thisPlayer = thisPlayer;
				}
				break;
			//AltCircleShot Shot
			case AttackButtons.B:
				AltCircleShot altCircleShot = Instantiate(altCircleShotPrefab, transform.position, new Quaternion()) as AltCircleShot;
				altCircleShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					altCircleShot.thisPlayer = thisPlayer;
				}
				altCircleShot.FireBurst();
				break;
			//Dual lasers
			case AttackButtons.X:
				return;
			//Charge attack
			case AttackButtons.Y:
				return;
			default:
				Debug.LogError("Attack button " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		if (GameManager.S.inGame) {
			SoundManager.instance.Play("BombExplode");
		}

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}
}
