using UnityEngine;
using System.Collections;

public class Masochist : Character {
	[HideInInspector]
	public MasochistShip masochistShip;
	
	protected override void Awake() {
		base.Awake();

		characterType = CharactersEnum.masochist;
		masochistShip = ship as MasochistShip;
	}

	void Start() {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, true);
	}
	
	void Update() {
		if (ship.shooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		//Masochist shield
		if (player.device != null) {
			if (player.device.Action4.WasPressed) {
				masochistShip.InstantiateShield();
			}
		}
		else if (player.device == null) {
			if (Input.GetKeyDown(Y)) {
				masochistShip.InstantiateShield();
			}
		}
	}
	
	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
