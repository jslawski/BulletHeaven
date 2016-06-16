using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InControl;
using UnityEngine.UI;

public enum Player {
	player1,
	player2,
	none
}

public enum GameStates {
	titleScreen,
	shipSelect,
	countdown,
	playing,
	finalAttack,
	winnerScreen
}

public class GameManager : MonoBehaviour {
public static bool CHECKING_MENU = false;
	public static GameManager S;
	public GameStates gameState;
	public static bool emergencyBumperControls = false;
	public bool slowMo = false;

	public PlayerShip[] players;
	InputDevice[] controllers;
	public string titleSceneName = "_Scene_Title";

	float minTimeInSceneForInput = 0.25f;
	float timeInScene = 0;
	
	[HideInInspector]
	public float maxDamageAmplification = 3f;
	float damageAmplificationTime = 120f;       //Time it takes to reach maximum damage amplification
	public float curDamageAmplification = 1f;

	[SerializeField]
	DamageValues[] damageValues;

	void Awake() {
		if (S != null) {
			Destroy(this);
			return;
		}
		S = this;

		DontDestroyOnLoad(this);

		maxDamageAmplification = 3;
		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();
		controllers = new InputDevice[2];
	}

	// Use this for initialization
	void Start () {
		SoundManager.instance.Play("MainTheme");

		if (slowMo) {
			Time.timeScale *= 0.15f;
			Time.fixedDeltaTime *= 0.15f;
		}

		if (!PressStartPrompt.promptsEnabled && gameState != GameStates.titleScreen) {
			StartGame();
		}
		//InitializePlayerShip(Player.player1, ShipType.generalist, Color.yellow);
	}

	public void InitializePlayerShip(ShipInfo shipInfo, InputDevice device=null) {
		print("Start initializing " + shipInfo.selectingPlayer);
		Player player = shipInfo.selectingPlayer;
		Player otherPlayer = (player == Player.player1) ? Player.player2 : Player.player1;
		ShipType typeOfShip = shipInfo.typeOfShip;
		Color playerColor = shipInfo.shipColor;

		//If both players are running the same type of ship, use the secondary color for the ship instead
		print(typeOfShip + " " + players[(int)otherPlayer].typeOfShip);
		if (typeOfShip == players[(int)otherPlayer].typeOfShip) {
			playerColor = shipInfo.shipSecondaryColor;
		}

		PlayerShip oldPlayerShip = players[(int)player];
		GameObject playerShipGO = oldPlayerShip.gameObject;

		//Set up the hitboxes appropriately
		SphereCollider hitbox = playerShipGO.GetComponentInChildren<SphereCollider>();
		hitbox.radius = shipInfo.hitBoxRadius;
		Vector3 hitboxPos = hitbox.transform.localPosition;
		hitboxPos.y = shipInfo.hitBoxOffset;
		hitbox.transform.localPosition = hitboxPos;

		PlayerShip newPlayerShip;
		switch (typeOfShip) {
			case ShipType.generalist:
				newPlayerShip = playerShipGO.AddComponent<Generalist>();
				break;
			case ShipType.vampire:
				newPlayerShip = playerShipGO.AddComponent<VampireShip>();
				break;
			case ShipType.masochist:
				newPlayerShip = playerShipGO.AddComponent<Masochist>();
				break;
			case ShipType.tank:
				newPlayerShip = playerShipGO.AddComponent<TankyShip>();
				break;
			case ShipType.glassCannon:
				newPlayerShip = playerShipGO.AddComponent<GlassCannon>();
				break;
			default:
				Debug.LogError("ShipType " + typeOfShip + " not handled in InitializePlayerShip()");
				return;
		}

		//Grab some values from the old script to apply them to the new one
		newPlayerShip.player = player;
		newPlayerShip.typeOfShip = typeOfShip;
		newPlayerShip.healthBar = oldPlayerShip.healthBar;
		newPlayerShip.controllerPrompt = oldPlayerShip.controllerPrompt;
		newPlayerShip.finishAttackPrompt = oldPlayerShip.finishAttackPrompt;
		newPlayerShip.deathExplosionPrefab = oldPlayerShip.deathExplosionPrefab;
		newPlayerShip.explosionPrefab = oldPlayerShip.explosionPrefab;
		newPlayerShip.playerShooting = oldPlayerShip.playerShooting;
		newPlayerShip.playerShooting.thisPlayer = newPlayerShip;
		newPlayerShip.playerMovement = oldPlayerShip.playerMovement;
		newPlayerShip.finalAttackPrefab = oldPlayerShip.finalAttackPrefab;

		//Remove the old script and replace the GameManager reference with the new one
		Destroy(oldPlayerShip);

		//Remember the controller in PlayerShip and GameManager
		controllers[(int)player] = device;
		newPlayerShip.device = device;

		//If the controller prompt is up, hide it
		if (newPlayerShip.controllerPrompt && PressStartPrompt.promptsEnabled) {
			newPlayerShip.controllerPrompt.HidePressStartPrompt();
		}

		//Set player's color
		newPlayerShip.playerColor = playerColor;
		foreach (var ammo in newPlayerShip.playerShooting.ammoImages) {
			ammo.GetComponent<Image>().color = playerColor;
		}
		newPlayerShip.healthBar.SetColor(playerColor);

		//Set the type of bomb and fix old references
		newPlayerShip.playerShooting.SetBombType(typeOfShip);
		newPlayerShip.playerShooting.thisPlayer = newPlayerShip;
		newPlayerShip.playerMovement.thisPlayer = newPlayerShip;
		players[(int)player] = newPlayerShip;

		print("Done initializing " + player);
	}
	
	// Update is called once per frame
	void Update () {
		//Quit the game on pressing escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (gameState == GameStates.playing && curDamageAmplification < maxDamageAmplification) {
			curDamageAmplification += Time.deltaTime * (maxDamageAmplification-1) / damageAmplificationTime;
			if (curDamageAmplification > maxDamageAmplification) {
				curDamageAmplification = maxDamageAmplification;
			}
		}

		//print((timeInScene < minTimeInSceneForInput) + "\n" + (gameState != GameStates.winnerScreen) + "\n" + (gameState == GameStates.titleScreen) + "\n" +
		//	  (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space")));
		if (timeInScene < minTimeInSceneForInput) {
			timeInScene += Time.deltaTime;
		}
		else {
			if (Input.GetKeyDown(KeyCode.BackQuote)) {
				emergencyBumperControls = !emergencyBumperControls;
				print("Emergency bumper mode " + (emergencyBumperControls ? "activated" : "deactivated") + ".");
			}

			if (Input.GetKeyDown(KeyCode.M)) {
				SoundManager.instance.muted = !SoundManager.instance.muted;
				print("Sound is now " + ((SoundManager.instance.muted) ? "muted." : "unmuted."));
			}

			CHECKING_MENU = true;
			bool menuWasPressed = InputManager.ActiveDevice.MenuWasPressed;
            if (gameState == GameStates.winnerScreen && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				SceneManager.LoadScene("_Scene_Title");
				gameState = GameStates.titleScreen;
			}
			else if (gameState == GameStates.titleScreen && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				SceneManager.LoadScene("_Scene_Ship_Selection");
			}
			CHECKING_MENU = false;
		}
	}

	public void DisplayDamage(Player playerDamaged, float damageIn) {
		if (damageValues.Length == 2 && damageValues[(int)playerDamaged] != null) {
			damageValues[(int)playerDamaged].DisplayDamage(damageIn);
		}
	}

	public void StartGame() {
		StartCoroutine(StartGameCoroutine());
	}

	IEnumerator StartGameCoroutine() {
		gameState = GameStates.countdown;
		for	(int i = 3; i >= 0; i--) {
			print(i);
			yield return null;
			//yield return new WaitForSeconds(1);
		}

		gameState = GameStates.playing;
	}

	public void EndGame(Player winner) {
		WinnerPanel.S.DisplayWinner(winner);
		gameState = GameStates.winnerScreen;
	}

	void Reset() {
		maxDamageAmplification = 3;
		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();
		damageValues = new DamageValues[players.Length];
		if (gameState != GameStates.titleScreen) {
			damageValues[0] = GameObject.Find("Player1DamageValues").GetComponent<DamageValues>();
			damageValues[1] = GameObject.Find("Player2DamageValues").GetComponent<DamageValues>();
		}
		curDamageAmplification = 1f;
		timeInScene = 0;
		slowMo = false;
	}

	IEnumerator OnLevelWasLoaded(int levelIndex) {
		yield return null;
		print("Level " + SceneManager.GetActiveScene().name + " was loaded.");
		switch (SceneManager.GetActiveScene().name) {
			case "_Scene_Title":
				gameState = GameStates.titleScreen;
				Reset();
				break;
			case "_Scene_Ship_Selection":
				gameState = GameStates.shipSelect;
				PassControllersToShipSelect();
				break;
			case "_Scene_Main":
				Reset();
				StartGame();
				break;
		}
	}
	
	void PassControllersToShipSelect() {
		foreach (var controller in controllers) {
			if (controller != null) {
				ControllerSetup.S.AddCurrentPlayer(controller);
			}
			//If player1 failed to have a controller, player2 shouldn't have one either
			else {
				return;
			}
		}
	}
}
