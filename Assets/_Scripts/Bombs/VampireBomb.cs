using UnityEngine;
using System.Collections;

public class VampireBomb : Bomb {
	GameObject shockwavePrefab;
	TrailShot trailShotPrefab;
	RotatingCircleShot rotatingCircleShotPrefab;
	LifeSapZone lifeSapZonePrefab;
	Reflector reflectorPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		trailShotPrefab = Resources.Load<TrailShot>("Prefabs/TrailShot");
		rotatingCircleShotPrefab = Resources.Load<RotatingCircleShot>("Prefabs/RotatingCircleShot");
		lifeSapZonePrefab = Resources.Load<LifeSapZone>("Prefabs/LifeSapZone");
		reflectorPrefab = Resources.Load<Reflector>("Prefabs/Reflector");
	}

	void Start() {
		if (buttonUI != null) {
			buttonUI.SetButtons(true, true, true, false);
		}
	}

	public override void Detonate(AttackButtons attackToPerform) {
		switch (attackToPerform) {
			//Trail Shot
			case AttackButtons.A:
				TrailShot trailShot = Instantiate(trailShotPrefab, transform.position, new Quaternion()) as TrailShot;
				trailShot.owningPlayer = owningPlayer;
				trailShot.FireBurst();
				break;
			//Rotating Circle shot
			case AttackButtons.B:
				RotatingCircleShot rotatingCircleShot = Instantiate(rotatingCircleShotPrefab, transform.position, new Quaternion()) as RotatingCircleShot;
				rotatingCircleShot.owningPlayer = owningPlayer;
				break;
			//LifeSapZone attack
			case AttackButtons.X:
				LifeSapZone lifeSapZone = Instantiate(lifeSapZonePrefab, transform.position, new Quaternion()) as LifeSapZone;
				lifeSapZone.owner = GameManager.S.players[(int)owningPlayer];
				break;
			//Reflektor
			case AttackButtons.Y:
				return;
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
