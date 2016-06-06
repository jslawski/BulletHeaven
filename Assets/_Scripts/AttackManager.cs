using UnityEngine;
using System.Collections;

/*************************************
STEPS TO IMPLEMENTING A NEW SHIP TYPE:
  1. Create a child script of PlayerShip that sets the typeOfShip to the type you want (EQUIP THE CHILD SCRIPT TO THE SHIP, NOT PLAYERSHIP.CS)
  2. Create a child script of Bomb that contains the implementation of all bomb-based attacks for the type
  3. In ShootBomb.cs, in the switch statement on Start(), specify the child bomb that you want to set as the bombPrefab
  4. In AttackManager.cs, add all of your bomb-based attacks to the Attack enum
  5. In AttackManager.cs, in ExecuteAttack(), add your ship type to the parent switch statement, then add a child switch statement for all of that type's attacks
*************************************/

public enum AttackButtons {
	A,
	B,
	X,
	Y,
	none
}

//Enum of ALL of the attacks in the game
public enum Attack {
	leadingShot,
	spiral,
	beam,
	reflector
}

public class AttackManager : MonoBehaviour {
	public static AttackManager S;
	
	void Awake() {
		S = this;
	}

	//Execute the correct attack based ont he ship type and button pressed
	public void ExecuteAttack(PlayerShip thisPlayerShip, AttackButtons buttonPressed) {
		//Switch statement for ship types
		switch (thisPlayerShip.typeOfShip) {
			case ShipType.generalist:
				//Cast player ship as a generalist ship
				Generalist generalistShip = thisPlayerShip as Generalist;

				//Switch statement for attacks
				switch (buttonPressed) {
					case AttackButtons.A:
						generalistShip.playerShooting.DetonateBomb(Attack.leadingShot);
						break;
					case AttackButtons.B:
						generalistShip.playerShooting.DetonateBomb(Attack.spiral);
						break;
					case AttackButtons.X:
						generalistShip.playerShooting.DetonateBomb(Attack.beam);
						break;
					case AttackButtons.Y:
						generalistShip.playerShooting.DetonateBomb(Attack.reflector);
						break;
					default:
						Debug.LogError("Attack Button " + buttonPressed + " is not valid");
						break;
				}
				break;
			default:
				Debug.LogError("Ship type " + thisPlayerShip.typeOfShip + " is not valid!");
				break;
		}

	}
}
