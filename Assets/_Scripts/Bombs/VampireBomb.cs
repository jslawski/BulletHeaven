using UnityEngine;
using System.Collections;

public class VampireBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	RotatingCircleShot rotatingCircleShotPrefab;
	LifeSapZone lifeSapZonePrefab;
	Reflector reflectorPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		rotatingCircleShotPrefab = Resources.Load<RotatingCircleShot>("Prefabs/RotatingCircleShot");
		lifeSapZonePrefab = Resources.Load<LifeSapZone>("Prefabs/LifeSapZone");
		reflectorPrefab = Resources.Load<Reflector>("Prefabs/Reflector");
	}

	public override void Detonate(AttackButtons attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		SoundManager.instance.Play("BombExplode");

		switch (attackToPerform) {
			//Leading shot
			case AttackButtons.A:
				LeadingShot newShot = Instantiate(leadingShotPrefab, transform.position, new Quaternion()) as LeadingShot;
				newShot.owningPlayer = owningPlayer;
				newShot.FireBurst();
				break;
			//Spiral shot
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
				Reflector reflectorShot = Instantiate(reflectorPrefab, transform.position, new Quaternion()) as Reflector;
				reflectorShot.owningPlayer = owningPlayer;
				break;
			default:
				Debug.LogError("Attack button " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}
}
