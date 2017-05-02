using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmCharacter : Character {
	protected override void Awake() {
		base.Awake();

		characterType = CharactersEnum.swarm;
		bulletShape = BulletShapes.diamond;
	}

	void Start() {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
	}
}
