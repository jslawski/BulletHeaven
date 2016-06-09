using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InControl;

public enum Player {
	player1,
	player2,
	none
}

public enum GameStates {
	titleScreen,
	controllerSelect,
	playing,
	finalAttack,
	winnerScreen
}

public class GameManager : MonoBehaviour {
	public static GameManager S;
	public GameStates gameState = GameStates.controllerSelect;
	public static bool emergencyBumperControls = false;
	public bool gameHasBegun = false;
	public bool slowMo = false;

	public PlayerShip[] players;
	public string titleSceneName = "_Scene_Title";

	float minTimeInSceneForInput = 0.25f;
	float timeInScene = 0;
	
	[HideInInspector]
	public float maxDamageAmplification = 3f;
	float damageAmplificationTime = 120f;       //Time it takes to reach maximum damage amplification
	public float curDamageAmplification = 1f;

	void Awake() {
		maxDamageAmplification = 3;
		S = this;
		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();
	}

	// Use this for initialization
	void Start () {
		SoundManager.instance.Play("MainTheme");

		if (slowMo) {
			Time.timeScale *= 0.15f;
			Time.fixedDeltaTime *= 0.15f;
		}

		//InitializePlayerShip(Player.player1, ShipType.generalist, Color.yellow);
	}

	public void InitializePlayerShip(Player player, ShipType typeOfShip, Color playerColor, InputDevice device=null) {
		PlayerShip oldPlayerShip = players[(int)player];
		GameObject playerShipGO = oldPlayerShip.gameObject;
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
			default:
				Debug.LogError("ShipType " + typeOfShip + " not handled in InitializePlayerShip()");
				return;
		}

		//Grab some values from the old script to apply them to the new one
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

		//Fix old references to the oldPlayerShip and set new values for ship type and color
		newPlayerShip.device = device;
		if (newPlayerShip.controllerPrompt && PressStartPrompt.promptsEnabled) {
			newPlayerShip.controllerPrompt.HidePressStartPrompt();
		}
		newPlayerShip.playerColor = playerColor;
		newPlayerShip.playerShooting.SetBombType(typeOfShip);
		newPlayerShip.playerShooting.thisPlayer = newPlayerShip;
		newPlayerShip.playerMovement.thisPlayer = newPlayerShip;
		players[(int)player] = newPlayerShip;
	}
	
	// Update is called once per frame
	void Update () {
		//Quit the game on pressing escape
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (gameHasBegun && curDamageAmplification < maxDamageAmplification) {
			curDamageAmplification += Time.deltaTime * (maxDamageAmplification-1) / damageAmplificationTime;
			if (curDamageAmplification > maxDamageAmplification) {
				curDamageAmplification = maxDamageAmplification;
			}
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
				SceneManager.LoadScene("_Scene_Title");
			}
			else if (gameState == GameStates.titleScreen && (InputManager.ActiveDevice.MenuWasPressed || Input.GetKeyDown("space"))) {
				SceneManager.LoadScene("_Scene_Main");
			}
		}
	}

	public void StartGame() {
		gameState = GameStates.playing;
		gameHasBegun = true;
	}

	public void EndGame(Player winner) {
		WinnerPanel.S.DisplayWinner(winner);
	}
}
