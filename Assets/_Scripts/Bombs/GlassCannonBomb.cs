using UnityEngine;
using System.Collections;

public class GlassCannonBomb : Bomb {
	GameObject shockwavePrefab;
	AltCircleShot altCircleShotPrefab;

	new void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		altCircleShotPrefab = Resources.Load<AltCircleShot>("Prefabs/AltCircleShot");
	}

	public override void Detonate(AttackButtons attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		SoundManager.instance.Play("BombExplode");

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
				//Reflector reflectorShot = Instantiate(reflectorPrefab, transform.position, new Quaternion()) as Reflector;
				//reflectorShot.owningPlayer = owningPlayer;
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
