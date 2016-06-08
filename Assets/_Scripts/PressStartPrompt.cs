using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PressStartPrompt : MonoBehaviour {
	//Set this to false if the prompt is annoying you in development
	public static readonly bool promptsEnabled = false;
	public static bool[] playersReady;
	public Player thisPlayer;

	// Use this for initialization
	void Awake () {
		playersReady = null;
		if (playersReady == null) {
			playersReady = new bool[GameManager.S.players.Length];
		}

		if (!promptsEnabled) {
			HidePressStartPrompt();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			//If the controller setup is expecting input from this player
			if (ControllerSetup.S.curPlayer == thisPlayer) {
				//Tell it to skip over us and go to the next player (this player will be using keyboard for now)
				StartCoroutine(IncrementCurPlayer());
			}
		}
	}

	public void HidePressStartPrompt() {
		GetComponent<Image>().enabled = false;
		foreach (var child in transform.GetComponentsInChildren<Transform>()) {
			if (child == transform) {
				continue;
			}
			child.gameObject.SetActive(false);
		}

		//Remember that this player is ready to play
		playersReady[(int)thisPlayer] = true;
		if (AllPlayersReady()) {
			GameManager.S.StartGame();
		}
	}

	bool AllPlayersReady() {
		for (int i = 0; i < playersReady.Length; i++) {
			if (!playersReady[i]) {
				return false;
			}
		}
		return true;
	}
	
	IEnumerator IncrementCurPlayer() {
		yield return new WaitForEndOfFrame();
		ControllerSetup.S.curPlayer = (Player)(((int)ControllerSetup.S.curPlayer + 1) % ((int)Player.none));
		HidePressStartPrompt();
	}
}
