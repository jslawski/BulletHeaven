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

	public override void Detonate(AttackButtons attackToPerform) {
		switch (attackToPerform) {
			//Dual Lasers
			case AttackButtons.A:
				return;
			//Charge Shot
			case AttackButtons.B:
				return;
			//AltCircleShot attack
			case AttackButtons.X:
				AltCircleShot altCircleShot = Instantiate(altCircleShotPrefab, transform.position, new Quaternion()) as AltCircleShot;
				altCircleShot.owningPlayer = owningPlayer;
				altCircleShot.FireBurst();
				break;
			//Reflektor
			case AttackButtons.Y:
				HomingGroupShot homingGroupShot = Instantiate(homingGroupShotPrefab, transform.position, new Quaternion()) as HomingGroupShot;
				homingGroupShot.owningPlayer = owningPlayer;
				break;
			default:
				Debug.LogError("Attack button " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		SoundManager.instance.Play("BombExplode");

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}
}
