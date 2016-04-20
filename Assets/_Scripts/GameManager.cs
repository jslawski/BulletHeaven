﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InControl;

public enum Player {
	player1,
	player2,
	none
}

public enum GameStates {
	controllerSelect,
	playing,
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

	void Awake() {
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
	}
	
	// Update is called once per frame
	void Update () {
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
	}

	public void StartGame() {
		gameState = GameStates.playing;
		gameHasBegun = true;
	}

	public void EndGame(Player winner) {
		gameState = GameStates.winnerScreen;
		WinnerPanel.S.DisplayWinner(winner);
	}
}
