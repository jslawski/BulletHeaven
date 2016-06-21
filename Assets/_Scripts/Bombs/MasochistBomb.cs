using UnityEngine;
using System.Collections;

public class MasochistBomb : Bomb {
	GameObject shockwavePrefab;
	SineShot sineShotPrefab;
	SpreadShot spreadShotPrefab;
	ExplodeAttack explodeAttackPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		sineShotPrefab = Resources.Load<SineShot>("Prefabs/SineShot");
		spreadShotPrefab = Resources.Load<SpreadShot>("Prefabs/SpreadShot");
		explodeAttackPrefab = Resources.Load<ExplodeAttack>("Prefabs/ExplodeAttack");
	}

	void Start() {
		if (buttonUI != null) {
			buttonUI.SetButtons(true, true, true, false);
		}
	}

	public override void Detonate(AttackButtons attackToPerform) {
		switch (attackToPerform) {
			//Sine shot
			case AttackButtons.A:
				SineShot newShot = Instantiate(sineShotPrefab, transform.position, new Quaternion()) as SineShot;
				newShot.owningPlayer = owningPlayer;
				newShot.masochistPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
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
				explodeAttack.FireBurst();
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
