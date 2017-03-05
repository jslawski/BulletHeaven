using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public PlayerEnum playerEnum;
	public Color playerColor;
	public PlayerShip ship;
	public InputDevice device;

	private Player _otherPlayer = null;
	public Player otherPlayer {
		get {
			if (_otherPlayer == null) {
				_otherPlayer = GameManager.S.OtherPlayer(this);
			}
			return _otherPlayer;
		}
	}

	//Player-related UI elements
	public DurationBar durationBar;
	public GameObject finishAttackPrompt;
	public Ammo[] ammoImages;

	//Player movement bounds
	public float viewportMinX;
	public float viewportMaxX;
	public float viewportMinY;
	public float viewportMaxY;

	//Prefabs for instantiating ships
	public Generalist generalistPrefab;
	public GlassCannon glassCannonPrefab;
	public Masochist masochistPrefab;
	public TankyShip tankPrefab;
	public VampireShip vampirePrefab;

	public PlayerShip InstantiateShip(ShipInfo shipInfo) {
		Debug.Assert(shipInfo.selectingPlayer == playerEnum, "Asked " + playerEnum.ToString() + " to instantiate a ship for " + shipInfo.selectingPlayer.ToString());
		
		PlayerEnum otherPlayerEnum = GameManager.S.OtherPlayerEnum(playerEnum);
		Color newShipColor = shipInfo.shipColor;
		//If both players are running the same type of ship, use the secondary color for the ship instead
		if (shipInfo.selectingPlayer != PlayerEnum.player1 && shipInfo.typeOfShip == GameManager.S.players[(int)otherPlayerEnum].ship.typeOfShip) {
			newShipColor = shipInfo.shipSecondaryColor;
		}

		PlayerShip prefab;
		switch (shipInfo.typeOfShip) {
			case ShipType.generalist:
				prefab = generalistPrefab;
				break;
			case ShipType.glassCannon:
				prefab = glassCannonPrefab;
				break;
			case ShipType.masochist:
				prefab = masochistPrefab;
				break;
			case ShipType.tank:
				prefab = tankPrefab;
				break;
			case ShipType.vampire:
				prefab = vampirePrefab;
				break;
			default:
				Debug.LogError("Ship type " + shipInfo.typeOfShip.ToString() + " not found!");
				return null;
		}

		ShipMovement prefabMovement = prefab.GetComponent<ShipMovement>();
		prefabMovement.viewportMinX = viewportMinX;
		prefabMovement.viewportMaxX = viewportMaxX;
		prefabMovement.viewportMinY = viewportMinY;
		prefabMovement.viewportMaxY = viewportMaxY;

		Quaternion startRot = (playerEnum == PlayerEnum.player1) ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 90);
		this.ship = Instantiate<PlayerShip>(prefab, this.transform.position, startRot, this.transform);
		this.ship.playerEnum = this.playerEnum;
		this.ship.shooting.SetBombType(shipInfo.typeOfShip);
		
		//Set colors for the player
		this.playerColor = newShipColor;
		foreach (var ammo in this.ammoImages) {
			ammo.GetComponent<Image>().color = playerColor;
		}
		this.durationBar.SetColor(newShipColor);

		return this.ship;
	}

	// Use this for initialization
	IEnumerator Start () {
		while (!GameManager.S.shipsReady) {
			yield return null;
		}

		if (durationBar != null) {
			durationBar.SetPercent(0);
			durationBar.target = ship.transform;
		}
	}
}
