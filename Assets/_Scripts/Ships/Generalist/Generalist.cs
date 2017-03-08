using UnityEngine;
using System.Collections;

public class Generalist : Character {
	protected override void Awake() {
		base.Awake();

		characterType = CharactersEnum.generalist;
	}

	void Start() {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
	}

	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
