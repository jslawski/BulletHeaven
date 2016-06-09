using UnityEngine;
using System.Collections;

public class MasochistBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	SpreadShot spreadShotPrefab;
	ExplodeAttack explodeAttackPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		spreadShotPrefab = Resources.Load<SpreadShot>("Prefabs/SpreadShot");
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
			//Spread shot
			case AttackButtons.B:
				SpreadShot spreadShot = Instantiate(spreadShotPrefab, transform.position, new Quaternion()) as SpreadShot;
				spreadShot.owningPlayer = owningPlayer;
				spreadShot.masochistPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
				spreadShot.FireBurst();
				break;
			//Explode attack
			case AttackButtons.X:
				ExplodeAttack explodeAttack = Instantiate(explodeAttackPrefab, transform.position, new Quaternion()) as ExplodeAttack;
				explodeAttack.owningPlayer = owningPlayer;
				explodeAttack.ExecuteExplosion();
				break;
			//Shield
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
