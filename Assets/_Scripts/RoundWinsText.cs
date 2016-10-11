using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoundWinsText : MonoBehaviour {
	public Player player;

	int _roundWins = 0;
	public int roundWins {
		get {
			return _roundWins;
		}
		set {
			_roundWins = value;
			roundWinsText.text = value.ToString();
		}
	}
	Text roundWinsText;
	bool roundHasEnded = false;

	void Awake() {
		roundWinsText = GetComponent<Text>();
	}

	// Use this for initialization
	IEnumerator Start () {
		if (Options.numRounds == 1) {
			gameObject.SetActive(false);
		}

		//Wait for players to be initialized in GameManager
		yield return new WaitForSeconds(0.1f);
		roundWinsText.color = GameManager.S.players[(int)player].playerColor;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.S.gameState == GameStates.transitioning) {
			if (!roundHasEnded) {
				EndRound();
			}
			return;
		}
		else if (!roundHasEnded && GameManager.S.gameState == GameStates.winnerScreen) {
			EndRound();
		}

		if (roundWins != GameManager.roundsWon[(int)player]) {
			roundWins = GameManager.roundsWon[(int)player];
		}
	}

	void EndRound() {
		roundHasEnded = true;

		Color curColor = roundWinsText.color;
		curColor.a *= 0.4f;
		roundWinsText.color = curColor;
	}
}
