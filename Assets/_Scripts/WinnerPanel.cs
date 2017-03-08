using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class WinnerPanel : MonoBehaviour {
	public static WinnerPanel S;
	Text winningPlayerText;
	Text titleText;
	Text championText;
	Text pressStartText;
	Text roundWonText;

	float fadeInDuration = 1f;

	private List<string> closeWinVerbs = new List<string> { "Sneaks in", "Barely takes", "Steals", "Underdogs", "Turns around", "Squeaks out" };
	private List<string> bigWinVerbs = new List<string> { "Crushes", "Obliterates", "Decimates", "Annihilates", "Absolutely kills", "Demolishes" };
	private float winThreshold;
	private float closenessFactor = 0.20f;

	// Use this for initialization
	void Start () {
		S = this;

		winningPlayerText = transform.FindChild("WinnerText").GetComponent<Text>();
		titleText = transform.FindChild("WinnerTitleText").GetComponent<Text>();
		championText = transform.FindChild("ChampionText").GetComponent<Text>();
		pressStartText = transform.FindChild("PressStartText").GetComponent<Text>();
		roundWonText = transform.FindChild("RoundWonText").GetComponent<Text>();
    }

	public void DisplayWinner(PlayerEnum winner) {
		foreach (var childTextComponent in transform.GetComponentsInChildren<Text>()) {
			childTextComponent.enabled = true;
		}
		Color winnerColor = GameManager.S.players[(int)winner].playerColor;

		if (winner == PlayerEnum.player1) {
			winningPlayerText.text = "Player 1 is the";
		}
		else if (winner == PlayerEnum.player2) {
			winningPlayerText.text = "Player 2 is the";
		}

		//Display a different message for a mid-round win
		if (GameManager.S.gameState == GameStates.midRoundVictory) {
			if (winner == PlayerEnum.player1) {
				roundWonText.text = "Player 1";
			}
			else if (winner == PlayerEnum.player2) {
				roundWonText.text = "Player 2";
			}

			//TODO 3/6/17: Make this work for characters with multiple ships
			//Personalized semi-random message depending on how close the match was
			//If the total remaining health from the winning player is less than <closenessFactor>% at the end of the round, it is considered a "close win"
			winThreshold = (GameManager.S.players[(int)PlayerEnum.player1].character.ship.maxHealth + 
				GameManager.S.players[(int)PlayerEnum.player2].character.ship.maxHealth) * closenessFactor;
			int winMessageIndex = Random.Range(0, closeWinVerbs.Count);
			if (Mathf.Abs(GameManager.S.players[(int)PlayerEnum.player1].character.ship.health - 
				GameManager.S.players[(int)PlayerEnum.player2].character.ship.health) > winThreshold) {
				roundWonText.text += "\n" + bigWinVerbs[winMessageIndex];
			}
			else {
				roundWonText.text += "\n" + closeWinVerbs[winMessageIndex];
			}

			roundWonText.text += "\nRound " + (GameManager.roundsWon[0] + GameManager.roundsWon[1]);

			titleText.text = string.Empty;
			winningPlayerText.text = string.Empty;
			championText.text = string.Empty;
		}

		StartCoroutine(FadeInWinner(winnerColor, winner));
	}

	IEnumerator FadeInWinner(Color winnerColor, PlayerEnum winner) {
		Color startColor = winnerColor;
		startColor.a = 0;

		float timeElapsed = 0;
		while (timeElapsed < fadeInDuration) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / fadeInDuration;

			winningPlayerText.color = Color.Lerp(startColor, winnerColor, percent);
			titleText.color = Color.Lerp(startColor, winnerColor, percent);
			championText.color = Color.Lerp(startColor, winnerColor, percent);
			roundWonText.color = Color.Lerp(startColor, winnerColor, percent);

			yield return 0;
		}

		if (GameManager.roundsWon[(int)winner] > Options.numRounds / 2) {
			GameManager.S.gameState = GameStates.winnerScreen;
		}
		else {
			GameManager.S.gameState = GameStates.midRoundVictory;
		}
	}
}
