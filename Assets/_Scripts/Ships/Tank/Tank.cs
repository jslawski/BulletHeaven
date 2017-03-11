using UnityEngine;
using System.Collections;

public class Tank : Character {

	protected override void Awake() {
		base.Awake();
		
		characterType = CharactersEnum.tank;
		bulletShape = BulletShapes.sun;
	}

	// Use this for initialization
	void Start () {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
	}

	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
