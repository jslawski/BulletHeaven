using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WinnerPanel : MonoBehaviour {
	public static WinnerPanel S;
	Text winningPlayerText;
	Text titleText;
	Text championText;
	Text pressStartText;

	float fadeInDuration = 1f;

	// Use this for initialization
	void Start () {
		S = this;

		winningPlayerText = transform.FindChild("WinnerText").GetComponent<Text>();
		titleText = transform.FindChild("WinnerTitleText").GetComponent<Text>();
		championText = transform.FindChild("ChampionText").GetComponent<Text>();
		pressStartText = transform.FindChild("PressStartText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayWinner(Player winner) {
		foreach (var childTextComponent in transform.GetComponentsInChildren<Text>()) {
			childTextComponent.enabled = true;
		}
		Color winnerColor = GameManager.S.players[(int)winner].playerColor;

		if (winner == Player.player1) {
			winningPlayerText.text = "Player 1 is the";
		}
		else if (winner == Player.player2) {
			winningPlayerText.text = "Player 2 is the";
		}

		StartCoroutine(FadeInWinner(winnerColor));
	}

	IEnumerator FadeInWinner(Color winnerColor) {
		Color startColor = winnerColor;
		startColor.a = 0;

		float timeElapsed = 0;
		while (timeElapsed < fadeInDuration) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / fadeInDuration;

			winningPlayerText.color = Color.Lerp(startColor, winnerColor, percent);
			titleText.color = Color.Lerp(startColor, winnerColor, percent);
			championText.color = Color.Lerp(startColor, winnerColor, percent);

			yield return 0;
		}

		GameManager.S.gameState = GameStates.winnerScreen;
	}
}
