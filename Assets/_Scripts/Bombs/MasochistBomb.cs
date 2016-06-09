using UnityEngine;
using System.Collections;

public class MasochistBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	SpiralShot spiralShotPrefab;
	ExplodeAttack explodeAttackPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		spiralShotPrefab = Resources.Load<SpiralShot>("Prefabs/SpiralShot");
		explodeAttackPrefab = Resources.Load<ExplodeAttack>("Prefabs/ExplodeAttack");
	}

	public override void Detonate(AttackButtons attackToPerform) {
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
			//Explode attack
			case AttackButtons.X:
				ExplodeAttack explodeAttack = Instantiate(explodeAttackPrefab, transform.position, new Quaternion()) as ExplodeAttack;
				explodeAttack.owningPlayer = owningPlayer;
				explodeAttack.ExecuteExplosion();
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
