using UnityEngine;
using System.Collections;

public enum Player {
	player1,
	player2,
	none
}

public class GameManager : MonoBehaviour {
	public static GameManager S;
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
			Time.timeScale *= 0.75f;
			Time.fixedDeltaTime *= 0.75f;
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
	}

	public void StartGame() {
		gameHasBegun = true;
	}
}
