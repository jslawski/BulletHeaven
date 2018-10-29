using UnityEngine;
using System.Collections;

public class GeneralistBomb : Bomb {
	GameObject shockwavePrefab;
	LeadingShot leadingShotPrefab;
	SpiralShot spiralShotPrefab;
	Beam beamShotPrefab;
	Reflector reflectorPrefab;

	protected override void Awake() {
		base.Awake();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
		leadingShotPrefab = Resources.Load<LeadingShot>("Prefabs/LeadingShot");
		spiralShotPrefab = Resources.Load<SpiralShot>("Prefabs/SpiralShot");
		beamShotPrefab = Resources.Load<Beam>("Prefabs/Beam");
		reflectorPrefab = Resources.Load<Reflector>("Prefabs/Reflector");
	}

	void Start() {
		if (buttonUI != null) {
			buttonUI.SetButtons(true, true, true, true);
		}
	}

	public override void Detonate(AttackButtons attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		if (GameManager.S.inGame) {
			SoundManager.instance.Play("BombExplode");
		}

		switch (attackToPerform) {
			//Leading shot
			case AttackButtons.A:
				LeadingShot newShot = Instantiate(leadingShotPrefab, transform.position, new Quaternion()) as LeadingShot;
				newShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					newShot.thisPlayer = thisPlayer;
				}
                newShot.targetShip = targetPlayer.character.ship;
                newShot.FireBurst();
				break;
			//Spiral shot
			case AttackButtons.B:
				SpiralShot spiralShot = Instantiate(spiralShotPrefab, transform.position, new Quaternion()) as SpiralShot;
				spiralShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame || GameManager.S.gameState == GameStates.shipSelect) {
					spiralShot.thisPlayer = thisPlayer;
				}
				spiralShot.FireBurst();
				break;
			//Beam attack
			case AttackButtons.X:
				Beam beamShot = Instantiate(beamShotPrefab, transform.position, new Quaternion()) as Beam;
				beamShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame || GameManager.S.gameState == GameStates.shipSelect) {
					beamShot.SetColor(thisPlayer.playerColor);
				}
				break;
			//Reflektor
			case AttackButtons.Y:
				Reflector reflectorShot = Instantiate(reflectorPrefab, transform.position, new Quaternion()) as Reflector;
				reflectorShot.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame || GameManager.S.gameState == GameStates.shipSelect) {
					reflectorShot.thisPlayer = thisPlayer;
					reflectorShot.SetColor(thisPlayer.playerColor);
				}
				reflectorShot.otherPlayer = targetPlayer;
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
