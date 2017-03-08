using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CharactersEnum {
	none,
	generalist,
	tank,
	masochist,
	glassCannon,
	vampire,
	swarm,
	random
}

public abstract class Character : MonoBehaviour {
	public Player player;
	public List<Ship> ships;
	//Shorthand for accessing single ship for all characters who only use one ship
	public Ship ship {
		get {
			Debug.Assert(ships != null, "ships[] array not yet initialized.");
			Debug.Assert(ships.Count == 1, "Tried to access Character.ship, but " + characterType + " has more than one ship. Use Character.ships[i] instead.");
			return ships[0];
		}
		set {
			if (ships == null) {
				ships = new List<Ship>();
				ships.Add(value);
			}
			else {
				ships[0] = value;
			}
		}
	}
	public CharactersEnum characterType;

	public FinishAttack finalAttackPrefab;
	public PlayerEnum playerEnum;

	protected bool inHeartbeatCoroutine = false;
	protected float lowOnHealthThreshold = 0.3f;
	protected float heartbeatPulseDuration = 0.1f;
	protected float timeBetweenHeartbeats = 1f;
	protected float heartbeatVibration = 0.5f;

	public bool dead = false;

	protected KeyCode A, B, X, Y;

	protected virtual void Awake() {
		player = GetComponentInParent<Player>();
		if (player != null) {
			playerEnum = player.playerEnum;
		}
		ships = new List<Ship>(GetComponentsInChildren<Ship>());

		if (playerEnum == PlayerEnum.player1) {
			A = KeyCode.Alpha1;
			B = KeyCode.Alpha2;
			X = KeyCode.Alpha3;
			Y = KeyCode.Alpha4;
		}
		else {
			A = KeyCode.Keypad1;
			B = KeyCode.Keypad2;
			X = KeyCode.Keypad3;
			Y = KeyCode.Keypad4;
		}

		SubscribeToEvents();
	}

	public virtual Ship GetClosestShip(Vector3 location) {
		//Common case handled first
		if (ships.Count == 1) return ship;

		float minDistance = float.MaxValue;
		Ship closestShip = null;
		foreach (Ship ship in ships) {
			float distance = (location - ship.transform.position).magnitude;
			if (distance < minDistance) {
				minDistance = distance;
				closestShip = ship;
			}
		}

		return closestShip;
	}
	
	void ListenForShipDeath(Ship deadShip) {
		ships.Remove(deadShip);

		if (ships.Count == 0) {
			KillCharacter();
		}
	}

	void HeartbeatOnLowHealth() {
		//TODO
	}

	void KillCharacter() {
		if (dead) {
			return;
		}

		SoundManager.instance.Play("NearDeath", 1);
		dead = true;
		
		player.otherPlayer.character.InitializeFinalAttack();
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
		if (player.durationBar != null) {
			player.durationBar.SetPercent(0);
		}
	}

	//Maybe move this to Player
	public void InitializeFinalAttack() {
		player.finishAttackPrompt.SetActive(true);
		player.finishAttackPrompt.GetComponentInChildren<Text>().color = player.playerColor;
		player.finishAttackPrompt.transform.FindChild("Plus").GetComponent<Image>().color = player.playerColor;
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
		if (player.durationBar != null) {
			player.durationBar.SetPercent(0);
		}

		Vector3 spawnPos = transform.position + transform.up * 4.5f;
        FinishAttack finalAttack = Instantiate(finalAttackPrefab, spawnPos, new Quaternion()) as FinishAttack;
		finalAttack.owningPlayer = playerEnum;
		finalAttack.fireKey = (playerEnum == PlayerEnum.player1) ? KeyCode.E : KeyCode.KeypadEnter;

		//Disable shooting so you don't fire a bomb when you perform the final attack
		ApplyToAllShips(ship => {
			ship.shooting.shootingDisabled = true;
			ship.invincible = true;
		});
	}

	public delegate void ShipApplyDelegate(Ship ship);
	public void ApplyToAllShips(ShipApplyDelegate applyFunction) {
		foreach (Ship ship in ships) {
			applyFunction(ship);
		}
	}

	public delegate bool ShipForAllDelegate(Ship ship);
	public bool ForAllShips(ShipForAllDelegate forAllPredicate) {
		foreach (Ship ship in ships) {
			if (!forAllPredicate(ship)) return false;
		}
		return true;
	}

	private void OnDestroy() {
		UnsubscribeFromEvents();
	}

	void SubscribeToEvents() {
		foreach (Ship ship in ships) {
			ship.onDeath += ListenForShipDeath;
		}
	}

	void UnsubscribeFromEvents() {
		foreach (Ship ship in ships) {
			ship.onDeath -= ListenForShipDeath;
		}
	}
}
