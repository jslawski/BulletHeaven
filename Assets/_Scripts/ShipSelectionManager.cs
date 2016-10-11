using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.SceneManagement;

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
	public PersistentShipInfo persistentInfoPrefab;

	static List<ShipSelectionManager> selectionMenus;
	static bool reverseScrollDirection = true;
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

	public AbilityPreviewScreen abilityPreview;
	public bool hasFocus = true;

	public PositionInfo[] positionInfos;        //Information about each selection position (off-screen left, left, selected, etc.)
												//such as the world position, alpha value, and orderInLayer value

	public OptionsMenu optionsMenu;
	public Text selectedShipNameField;
	public Text selectedShipDescriptionField;
	[Header("Stat Bar References")]
	public StatBar offenseStat;
	public StatBar defenseStat;
	public StatBar speedStat;
	public StatBar maxHealthStat;
	public StatBar difficultyStat;
	public Text miscStatLabel;
	public StatBar miscStat;

	bool inChooseRandomShipCoroutine = false;
	float minWaitTime = 0.075f;
	float maxWaitTime = 0.075f;

	[HideInInspector]
	public KeyCode left,right,A,B,Y,start;

	void Awake() {
		persistentInfoPrefab = Resources.Load<PersistentShipInfo>("Prefabs/ShipInfo");

		DontDestroyOnLoad(this.gameObject);
		if (selectionMenus == null) {
			selectionMenus = new List<ShipSelectionManager>();
		}
		selectionMenus.Add(this);
	}

	// Use this for initialization
	void Start () {
		if (player == Player.player1) {
			left = KeyCode.A;
			right = KeyCode.D;
			A = KeyCode.Alpha1;
			B = KeyCode.Alpha2;
			Y = KeyCode.Alpha4;
			start = KeyCode.Space;
		}
		else if	(player == Player.player2) {
			left = KeyCode.LeftArrow;
			right = KeyCode.RightArrow;
			A = KeyCode.Keypad1;
			B = KeyCode.Keypad2;
			Y = KeyCode.Keypad4;
			start = KeyCode.KeypadEnter;
		}

		ships = GetComponentsInChildren<ShipInfo>();
		foreach (var ship in ships) {
			ship.selectingPlayer = player;
		}
		//ships = new ShipInfo[(int)SelectionPosition.numPositions];
	}
	
	// Update is called once per frame
	void Update () {
		//Don't allow input while it doesn't have focus
		if (!hasFocus || OptionsMenu.hasFocus) {
			return;
		}
		//Don't allow input while randoming ships
		if (inChooseRandomShipCoroutine || GameManager.S.gameState != GameStates.shipSelect) {
			return;
		}

		//Keyboard support
		if (Input.GetKeyDown(right) && !playerReady) {
			Scroll(true);
		}
		else if (Input.GetKeyDown(left) && !playerReady) {
			Scroll(false);
		}

		//Ready up
		if (Input.GetKeyDown(A) && !playerReady) {
			if (selectedShip.typeOfShip == ShipType.random) {
				StartCoroutine(RandomShip());
			}
			else {
				SoundManager.instance.Play("ShipConfirm", 1);
				print(player + " is ready.");
				playerReady = true;
			}
		}
		else if (Input.GetKeyDown(B) && playerReady) {
			SoundManager.instance.Play("ShipCancel");
			print(player + " is no longer ready.");
			playerReady = false;
		}
		else if (Input.GetKeyDown(Y) && selectedShip.typeOfShip != ShipType.random) {
			abilityPreview.SetAbilityPreview(selectedShip);
		}

		if (GameManager.S.gameState == GameStates.shipSelect && Input.GetKeyDown(start)) {
			if (AllPlayersReady()) {
				SoundManager.instance.Play("StartGame");
				GameManager.S.gameState = GameStates.countdown;
				GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
			}
			else {
				optionsMenu.OpenOptionsMenu(device);
			}
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
					SoundManager.instance.Play("ShipConfirm", 1);
					print(player + " is ready.");
					playerReady = true;
				}
			}
			else if (device.Action2.WasPressed && playerReady) {
				SoundManager.instance.Play("ShipCancel");
				print(player + " is no longer ready.");
				playerReady = false;
			}
			else if (device.Action4.WasPressed && selectedShip.typeOfShip != ShipType.random) {
				abilityPreview.SetAbilityPreview(selectedShip);
			}

			if (GameManager.S.gameState == GameStates.shipSelect && device.MenuWasPressed) {
				if (AllPlayersReady()) {
					SoundManager.instance.Play("StartGame");
					GameManager.S.gameState = GameStates.countdown;
					GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
				}
				else {
					optionsMenu.OpenOptionsMenu(device);
				}
			}
		}
	}

	public void Scroll(bool toTheRight) {
		if (reverseScrollDirection) {
			toTheRight = !toTheRight;
		}
		foreach (var ship in ships) {
			if (ship == null) {
				continue;
			}
			//Scroll each ship in the correct direction
			ship.Scroll(toTheRight);
			if (toTheRight) {
				SoundManager.instance.Play("ShipScroll", 1.1f);
			}
			else {
				SoundManager.instance.Play("ShipScroll", 1f);
			}
		}
		//print(selectedShip);
	}

	void SetStatsForShip(ShipInfo shipInfo) {
		//Gracefully handle the case of no ship selected
		if (shipInfo == null) {
			selectedShipNameField.text = "";
			selectedShipDescriptionField.text = "";

			offenseStat.SetStatValue(0, Color.white);
			defenseStat.SetStatValue(0, Color.white);
			speedStat.SetStatValue(0, Color.white);
			maxHealthStat.SetStatValue(0, Color.white);
			difficultyStat.SetStatValue(0, Color.white);
			miscStatLabel.text = "Miscellaneous Stat";
			miscStat.SetStatValue(0, Color.white);
			return;
		}
		//Animate the random ship selection
		else if (shipInfo.typeOfShip == ShipType.random) {
			selectedShipNameField.text = shipInfo.shipName;
			selectedShipDescriptionField.text = shipInfo.description;
			miscStatLabel.text = shipInfo.miscLabel;

			offenseStat.AnimateRandomStats();
			defenseStat.AnimateRandomStats();
			speedStat.AnimateRandomStats();
			maxHealthStat.AnimateRandomStats();
			difficultyStat.AnimateRandomStats();
			miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
			return;
		}

		//Set the stat values for non-special case ships
		selectedShipNameField.text = shipInfo.shipName;
		selectedShipDescriptionField.text = shipInfo.description;

		offenseStat.SetStatValue(shipInfo.offense, shipInfo.shipColor);
		defenseStat.SetStatValue(shipInfo.defense, shipInfo.shipColor);
		speedStat.SetStatValue(shipInfo.speed, shipInfo.shipColor);
		maxHealthStat.SetStatValue(shipInfo.maxHealth, shipInfo.shipColor);
		difficultyStat.SetStatValue(shipInfo.difficulty, shipInfo.shipColor);
		miscStatLabel.text = shipInfo.miscLabel;
		miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
	}

	IEnumerator OnLevelWasLoaded(int levelIndex) {
		if (SceneManager.GetActiveScene().name != "_Scene_Main") {
			yield break;
		}

		//Wait for GameManager to initialize itself
		while (GameManager.S == null) {
			yield return null;
		}

		//Wait 2 frames, since GameManager takes 1 frame to re-assign player references
		yield return null;
		yield return null;
		//Wait an additional frame to initialize player 2's ship to guarantee initialization order
		if (selectedShip.selectingPlayer == Player.player2) {
			yield return null;
		}

		//Create the object that will pass the information to GameManager
		PersistentShipInfo persistentShipInfo = Instantiate(persistentInfoPrefab);
		persistentShipInfo.gameObject.name = "P" + ((int)selectedShip.selectingPlayer + 1) + "ShipInfo";
		persistentShipInfo.Initialize(selectedShip, device);

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

	public static bool AllPlayersReady() {
		foreach (var player in selectionMenus) {
			if (!player.playerReady) {
				return false;
			}
		}
		return true;
	}
}
