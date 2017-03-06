using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSingleShip : PlayerShip {
	public SwarmSingleShipMovement swarmShipMovement;
	KeyCode X, Y;

	new void Awake() {
		base.Awake();
		swarmShipMovement = GetComponent<SwarmSingleShipMovement>();

		maxHealth = 180f;
	}

	// Use this for initialization
	new void Start () {
		base.Start();

		X = (playerEnum == PlayerEnum.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		Y = (playerEnum == PlayerEnum.player1) ? KeyCode.Alpha4 : KeyCode.Keypad4;

		health = maxHealth;
	}
	
}
