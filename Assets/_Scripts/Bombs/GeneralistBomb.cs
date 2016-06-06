using UnityEngine;
using System.Collections;

public class GeneralistBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	SpiralShot spiralShotPrefab;
	Beam beamShotPrefab;
	Reflector reflectorPrefab;

	void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		spiralShotPrefab = Resources.Load<SpiralShot>("Prefabs/SpiralShot");
		beamShotPrefab = Resources.Load<Beam>("Prefabs/Beam");
		reflectorPrefab = Resources.Load<Reflector>("Prefabs/Reflector");
	}

	public override void Detonate(Attack attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		SoundManager.instance.Play("BombExplode");

		switch (attackToPerform) {
			case Attack.leadingShot:
				LeadingShot newShot = Instantiate(leadingShotPrefab, transform.position, new Quaternion()) as LeadingShot;
				newShot.owningPlayer = owningPlayer;
				newShot.FireBurst();
				break;
			case Attack.spiral:
				SpiralShot spiralShot = Instantiate(spiralShotPrefab, transform.position, new Quaternion()) as SpiralShot;
				spiralShot.owningPlayer = owningPlayer;
				spiralShot.FireBurst();
				break;
			case Attack.beam:
				Beam beamShot = Instantiate(beamShotPrefab, transform.position, new Quaternion()) as Beam;
				beamShot.owningPlayer = owningPlayer;
				break;
			case Attack.reflector:
				Reflector reflectorShot = Instantiate(reflectorPrefab, transform.position, new Quaternion()) as Reflector;
				reflectorShot.owningPlayer = owningPlayer;
				break;
			default:
				Debug.LogError("Attack type " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}
}
