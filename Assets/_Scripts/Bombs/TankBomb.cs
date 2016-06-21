using UnityEngine;
using System.Collections;

public class TankBomb : Bomb {
	GameObject shockwavePrefab;
	ConeShot coneShotPrefab;
	WeaveShot weaveShotPrefab;
	ClusterBomb clusterBombPrefab;
	BlackHole blackHolePrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		coneShotPrefab = Resources.Load<ConeShot>("Prefabs/ConeShot");
		weaveShotPrefab = Resources.Load<WeaveShot>("Prefabs/WeaveShot");
		clusterBombPrefab = Resources.Load<ClusterBomb>("Prefabs/ClusterBomb");
		blackHolePrefab = Resources.Load<BlackHole>("Prefabs/BlackHole");
	}

	void Start() {
		if (buttonUI != null) {
			buttonUI.SetButtons(true, true, true, true);
		}
	}

	public override void Detonate(AttackButtons attackToPerform) {
		switch (attackToPerform) {
			//Leading shot
			case AttackButtons.A:
				ConeShot coneShot = Instantiate(coneShotPrefab, transform.position, new Quaternion()) as ConeShot;
				coneShot.owningPlayer = owningPlayer;
				coneShot.FireBurst();
				break;
			//Spiral shot
			case AttackButtons.B:
				WeaveShot weaveShot = Instantiate(weaveShotPrefab, transform.position, new Quaternion()) as WeaveShot;
				weaveShot.owningPlayer = owningPlayer;
				weaveShot.FireBurst();
				break;
			//ClusterBomb attack
			case AttackButtons.X:
				ClusterBomb clusterBomb = Instantiate(clusterBombPrefab, transform.position, new Quaternion()) as ClusterBomb;
				clusterBomb.owningPlayer = owningPlayer;
				break;
			//BlackHole attack
			case AttackButtons.Y:
				BlackHole blackHole = Instantiate(blackHolePrefab, transform.position, new Quaternion()) as BlackHole;
				blackHole.owningPlayer = owningPlayer;
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
