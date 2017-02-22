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
	midRoundVictory,
	winnerScreen,
	transitioning
}

public class GameManager : MonoBehaviour {
	public static GameManager S;
	public GameStates gameState;
	public static bool emergencyBumperControls = false;
	public bool slowMo = false;

	public PlayerShip[] players;
	InputDevice[] controllers;
	public const string titleSceneName = "_Scene_Title";
	public const string shipSelectionSceneName = "_Scene_Ship_Selection_JDS";

	float minTimeInSceneForInput = 0.25f;
	float timeInScene = 0;
	
	[HideInInspector]
	public float maxDamageAmplification = 3f;
	[HideInInspector]
	public float minDamageAmplification = 1f;
	float damageAmplificationTime = 120f;       //Time it takes to reach maximum damage amplification
	public float curDamageAmplification = 1f;

	[SerializeField]
	DamageValues[] damageValues;

	Image fadePanel;
	public float fadeFromTitleDuration = 0.5f;
	public float fadeFromMainDuration = 0.5f;
	public float fadeFromShipSelectDuration = 2f;
	public string curTheme = "TitleTheme";

	public static int[] roundsWon;

	[SerializeField]
	public Text winPromptText;

	[HideInInspector]
	public bool singlePlayer;

	public bool inGame {
		get {
			return gameState == GameStates.playing || gameState == GameStates.titleScreen;
        }
	}

	void Awake() {
		if (S != null) {
			Destroy(this);
			return;
		}
		S = this;

		//**********Change this value to toggle single-player vs. multiplayer**********
		this.singlePlayer = true;
		//**********Change this value to toggle single-player vs. multiplayer**********

		DontDestroyOnLoad(this);

		Options.LoadOptionsFromPlayerPrefs();
		maxDamageAmplification = Options.maxDamageAmp;
		minDamageAmplification = Options.minDamageAmp;
		damageAmplificationTime = Options.damageAmpTime;

		curDamageAmplification = minDamageAmplification;

		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();

		roundsWon = new int[2];
		roundsWon[0] = roundsWon[1] = 0;

		controllers = new InputDevice[2];
	}

	// Use this for initialization
	void Start () {
		SoundManager.instance.Play("TitleTheme");

		if (slowMo) {
			Time.timeScale *= 0.15f;
			Time.fixedDeltaTime *= 0.15f;
		}

		if (!PressStartPrompt.promptsEnabled && gameState != GameStates.titleScreen && gameState != GameStates.shipSelect) {
			Countdown.S.BeginCountdown();
		}
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
		newPlayerShip.durationBar = oldPlayerShip.durationBar;
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
		newPlayerShip.durationBar.SetColor(playerColor);

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

			if (gameState == GameStates.winnerScreen && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				if (gameState != GameStates.transitioning) {
					TransitionScene(fadeFromMainDuration, "_Scene_Title");
					roundsWon[0] = roundsWon[1] = 0;
				}
			}
			else if (gameState == GameStates.midRoundVictory && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				if (gameState != GameStates.transitioning) {
					TransitionScene(fadeFromMainDuration, "_Scene_Main");
				}
			}
			else if (gameState == GameStates.titleScreen && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				SoundManager.instance.Play("PressStart");
				TransitionScene(fadeFromTitleDuration, shipSelectionSceneName);
			}
		}
	}

	public void TransitionScene(float fadeDuration, string nextScene) {
		//No double-starts!
		if (gameState == GameStates.transitioning) {
			return;
		}
		gameState = GameStates.transitioning;

		fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
		StartCoroutine(TransitionSceneCoroutine(fadeDuration, nextScene));
	}

	IEnumerator TransitionSceneCoroutine(float fadeDurationInSeconds, string nextScene) {
		for (float i = 0; i <= fadeDurationInSeconds; i += Time.fixedDeltaTime) {
			//Fade out screen
			Color curColor = fadePanel.color;
			curColor.a = i / fadeDurationInSeconds;
			fadePanel.color = curColor;

			//Fade out music
			SoundManager.instance.SetVolume(curTheme, 1 - i / fadeDurationInSeconds);

			yield return new WaitForFixedUpdate();
		}
		SceneManager.LoadScene(nextScene);
	}

	public void DisplayDamage(Player playerDamaged, float damageIn) {
		if (damageValues.Length == 2 && damageValues[(int)playerDamaged] != null) {
			damageValues[(int)playerDamaged].DisplayDamage(damageIn);
		}
	}

	public void StartGame() {
		winPromptText = GameObject.Find("PressStartText").GetComponent<Text>();
		gameState = GameStates.playing;
		StartCoroutine(AmplifyDamage());

		if (curTheme != "FightTheme") {
			SoundManager.instance.Play("FightTheme");
		}

		curTheme = "FightTheme";
	}

	IEnumerator AmplifyDamage() {
		float timeElapsed = 0;
		while (gameState == GameStates.playing && timeElapsed < damageAmplificationTime) {
			timeElapsed += Time.deltaTime;

			curDamageAmplification = Mathf.Lerp(minDamageAmplification, maxDamageAmplification, timeElapsed / damageAmplificationTime);

			yield return null;
		}
		if (timeElapsed > damageAmplificationTime) {
			curDamageAmplification = maxDamageAmplification;
		}
	}

	public void EndRound(Player winner) {
		roundsWon[(int)winner]++;

		if (roundsWon[(int)winner] > Options.numRounds/2) {
			winPromptText.text = TextLiterals.FINAL_VICTORY_PROMPT;
			EndGame(winner);
		}
		else {
			winPromptText.text = TextLiterals.MID_ROUND_VICTORY_PROMPT;
			gameState = GameStates.midRoundVictory;
			WinnerPanel.S.DisplayWinner(winner);
			
		}
	}

	public void EndGame(Player winner) {
		gameState = GameStates.winnerScreen;
		WinnerPanel.S.DisplayWinner(winner);
	}

	void Reset() {
		maxDamageAmplification = Options.maxDamageAmp;
		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();
		damageValues = new DamageValues[players.Length];
		if (gameState != GameStates.titleScreen) {
			damageValues[0] = GameObject.Find("Player1DamageValues").GetComponent<DamageValues>();
			damageValues[1] = GameObject.Find("Player2DamageValues").GetComponent<DamageValues>();
		}
		minDamageAmplification = Options.minDamageAmp;
		damageAmplificationTime = Options.damageAmpTime;
		curDamageAmplification = minDamageAmplification;

		timeInScene = 0;
		slowMo = false;
	}

	IEnumerator OnLevelWasLoaded(int levelIndex) {
		yield return null;
		print("Level " + SceneManager.GetActiveScene().name + " was loaded.");
		switch (SceneManager.GetActiveScene().name) {
			case "_Scene_Title":
				gameState = GameStates.titleScreen;
				SoundManager.instance.Play("TitleTheme");
				curTheme = "TitleTheme";
				Reset();
				break;
			case shipSelectionSceneName:
				gameState = GameStates.shipSelect;
				SoundManager.instance.Play("ShipSelectTheme");
				curTheme = "ShipSelectTheme";
				PassControllersToShipSelect();
				break;
			case "_Scene_Main":
				gameState = GameStates.countdown;
				Reset();
				//SoundManager.instance.Play("FightTheme");
				//curTheme = "FightTheme";
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
