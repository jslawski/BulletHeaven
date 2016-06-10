﻿using UnityEngine;
using System.Collections;

public class TankBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	SpiralShot spiralShotPrefab;
	ClusterBomb clusterBombPrefab;
	Reflector reflectorPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		spiralShotPrefab = Resources.Load<SpiralShot>("Prefabs/SpiralShot");
		clusterBombPrefab = Resources.Load<ClusterBomb>("Prefabs/ClusterBomb");
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
				SpiralShot spiralShot = Instantiate(spiralShotPrefab, transform.position, new Quaternion()) as SpiralShot;
				spiralShot.owningPlayer = owningPlayer;
				spiralShot.FireBurst();
				break;
			//ClusterBomb attack
			case AttackButtons.X:
				ClusterBomb clusterBomb = Instantiate(clusterBombPrefab, transform.position, new Quaternion()) as ClusterBomb;
				clusterBomb.owningPlayer = owningPlayer;
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