using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour {
	public static Countdown S;

	Image panelBackground;
	Image countdownBackground;
	Text countdown;

	bool inCountdownCoroutine = false;
	
	Color endColor = new Color(0,0,0,0);

	int maxFontSize = 300;
	int minFontSize = 1;

	void Awake() {
		S = this;

		panelBackground = GetComponent<Image>();
		countdownBackground = transform.GetChild(0).GetComponent<Image>();
		countdown = GetComponentInChildren<Text>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void BeginCountdown() {
		if (inCountdownCoroutine) {
			return;
		}
		StartCoroutine(CountdownCoroutine());
	}

	IEnumerator CountdownCoroutine() {
		inCountdownCoroutine = true;
		GameManager.S.gameState = GameStates.countdown;

		float maxCountdown = 3f;
		Color startColor = Color.white;

		for (float i = maxCountdown; i > 0; i-=Time.deltaTime) {
			float percent = 1-(i%1)/1f;
			int curCount = Mathf.FloorToInt(i+1);
            countdown.text = curCount.ToString();

			switch (curCount) {
				case 3:
					startColor = Color.red;
					break;
				case 2:
					startColor = Color.yellow;
					break;
				case 1:
					startColor = Color.green;

					Color backgroundCol = panelBackground.color;
					backgroundCol.a = Mathf.Lerp(1f, 0f, 1 - (i / 1f));
					panelBackground.color = backgroundCol;
					break;
			}

			countdown.fontSize = (int)Mathf.Lerp(maxFontSize, minFontSize, percent);
			countdown.color = Color.Lerp(startColor, endColor, percent);
			countdownBackground.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, percent);
			countdownBackground.color = countdown.color;


			yield return null;
		}

		panelBackground.enabled = false;
		countdown.enabled = false;

		GameManager.S.StartGame();

		inCountdownCoroutine = false;
	}
}
