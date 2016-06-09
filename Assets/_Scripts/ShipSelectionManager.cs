using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;

public enum SelectionPosition {
	offscreenLeft,
	left,
	selected,
	right,
	offscreenRight,
	invisibleCenter,
	numPositions
}

[System.Serializable]
public struct PositionInfo {
	public PositionInfo(Vector3 _position, Color _alphaColor, int _sortingOrder, Vector3 _scale) {
		position = _position;
		alphaColor = _alphaColor;
		orderInLayer = _sortingOrder;
		scale = _scale;
	}

	public Vector3 position;
	public Vector3 scale;
	public Color alphaColor;
	public int orderInLayer;
}

public class ShipSelectionManager : MonoBehaviour {
	static List<ShipSelectionManager> selectionMenus;
	public Player player;
	public InputDevice device;

	ShipInfo[] ships;
	public ShipInfo selectedShip {
		get {
			return _selectedShip;
		}
		set {
			_selectedShip = value;
			SetStatsForShip(value);
		}
	}
	ShipInfo _selectedShip;
	public bool playerReady = false;

	public PositionInfo[] positionInfos;		//Information about each selection position (off-screen left, left, selected, etc.)
												//such as the world position, alpha value, and orderInLayer value

	public Text selectedShipNameField;
	[Header("Stat Bar References")]
	public StatBar offenseStat;
	public StatBar defenseStat;
	public StatBar speedStat;
	public StatBar maxHealthStat;
	public StatBar fireRateStat;
	public Text miscStatLabel;
	public StatBar miscStat;

	bool inChooseRandomShipCoroutine = false;
	float minWaitTime = 0.075f;
	float maxWaitTime = 0.075f;

	void Awake() {
		DontDestroyOnLoad(this.gameObject);
		if (selectionMenus == null) {
			selectionMenus = new List<ShipSelectionManager>();
		}
		selectionMenus.Add(this);
	}

	// Use this for initialization
	void Start () {
		ships = GetComponentsInChildren<ShipInfo>();
		foreach (var ship in ships) {
			ship.selectingPlayer = player;
		}
		//ships = new ShipInfo[(int)SelectionPosition.numPositions];
	}
	
	// Update is called once per frame
	void Update () {
		//Don't allow input while randoming ships
		if (inChooseRandomShipCoroutine) {
			return;
		}

		//Keyboard support
		if (Input.GetKeyDown(KeyCode.RightArrow) && !playerReady) {
			Scroll(true);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow) && !playerReady) {
			Scroll(false);
		}

		//Ready up
		if (Input.GetKeyDown(KeyCode.Alpha1) && !playerReady) {
			if (selectedShip.typeOfShip == ShipType.random) {
				StartCoroutine(RandomShip());
			}
			else {
				print(player + " is ready.");
				playerReady = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) && playerReady) {
			print(player + " is no longer ready.");
			playerReady = false;
		}

		if (Input.GetKeyDown(KeyCode.Return) && AllPlayersReady()) {
			Application.LoadLevel("_Scene_Main");
		}

		//Controller support
		if (device != null) {
			if ((device.LeftStick.Right.WasPressed || device.DPadRight.WasPressed) && !playerReady) {
				Scroll(true);
			}
			else if ((device.LeftStick.Left.WasPressed || device.DPadLeft.WasPressed) && !playerReady) {
				Scroll(false);
			}

			//Ready up
			if (device.Action1.WasPressed && !playerReady) {
				if (selectedShip.typeOfShip == ShipType.random) {
					StartCoroutine(RandomShip());
				}
				else {
					print(player + " is ready.");
					playerReady = true;
				}
			}
			else if (device.Action2.WasPressed && playerReady) {
				print(player + " is no longer ready.");
				playerReady = false;
			}

			if (device.MenuWasPressed && AllPlayersReady()) {
				Application.LoadLevel("_Scene_Main");
			}
		}
	}

	public void Scroll(bool toTheRight) {
		foreach (var ship in ships) {
			if (ship == null) {
				continue;
			}
			//Scroll each ship in the correct direction
			ship.Scroll(toTheRight);
		}
		//print(selectedShip);
	}

	void SetStatsForShip(ShipInfo shipInfo) {
		//Gracefully handle the case of no ship selected
		if (shipInfo == null) {
			selectedShipNameField.text = "";

			offenseStat.SetStatValue(0, Color.white);
			defenseStat.SetStatValue(0, Color.white);
			speedStat.SetStatValue(0, Color.white);
			maxHealthStat.SetStatValue(0, Color.white);
			fireRateStat.SetStatValue(0, Color.white);
			miscStatLabel.text = "Miscellaneous Stat";
			miscStat.SetStatValue(0, Color.white);
			return;
		}
		//Animate the random ship selection
		else if (shipInfo.typeOfShip == ShipType.random) {
			selectedShipNameField.text = shipInfo.shipName;
			miscStatLabel.text = shipInfo.miscLabel;

			offenseStat.AnimateRandomStats();
			defenseStat.AnimateRandomStats();
			speedStat.AnimateRandomStats();
			maxHealthStat.AnimateRandomStats();
			fireRateStat.AnimateRandomStats();
			miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
		}

		//Set the stat values for non-special case ships
		selectedShipNameField.text = shipInfo.shipName;

		offenseStat.SetStatValue(shipInfo.offense, shipInfo.shipColor);
		defenseStat.SetStatValue(shipInfo.defense, shipInfo.shipColor);
		speedStat.SetStatValue(shipInfo.speed, shipInfo.shipColor);
		maxHealthStat.SetStatValue(shipInfo.maxHealth, shipInfo.shipColor);
		fireRateStat.SetStatValue(shipInfo.fireRate, shipInfo.shipColor);
		miscStatLabel.text = shipInfo.miscLabel;
		miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
	}

	IEnumerator OnLevelWasLoaded(int levelIndex) {
		print("Level with index " + levelIndex + " was loaded");

		//Wait for GameManager to initialize itself
		while (GameManager.S == null) {
			yield return null;
		}

		//Initialize this player's information
		GameManager.S.InitializePlayerShip(player, selectedShip.typeOfShip, selectedShip.shipColor, device);

		Destroy(gameObject);
	}

	IEnumerator RandomShip() {
		inChooseRandomShipCoroutine = true;
		int numPositions = positionInfos.Length;
		int randNumScrolls;
		//Choose a random number of times to scroll until we have something that won't end back on random
		do {
			randNumScrolls = Random.Range(numPositions, 2 * numPositions);
		}
		while (randNumScrolls % numPositions == 0);

		//Randomly choose to scroll left or right
		bool toTheRight = (Random.Range(0, 2) == 0);

		//Scroll to the randomly selected ship
		float waitTime = minWaitTime;
		for (int i = 0; i < randNumScrolls; i++) {
			Scroll(toTheRight);
			yield return new WaitForSeconds(waitTime);
			waitTime = Mathf.Lerp(minWaitTime, maxWaitTime, (float)i / randNumScrolls);
		}
		inChooseRandomShipCoroutine = false;
	}

	bool AllPlayersReady() {
		foreach (var player in selectionMenus) {
			if (!player.playerReady) {
				return false;
			}
		}
		return true;
	}
}
