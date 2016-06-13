using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerAndDamageAmpDisplay : MonoBehaviour {
	Text timer;
	Text damageAmpText;
	StretchText damageAmpStretch;

	public Gradient damageAmpGradient;
	int seconds = 0;

	float minFrequency = 0.2f;
	float maxFrequency = 2f;
	float minAmplitude = 0f;
	float maxAmplitude = 0.1f;

	// Use this for initialization
	void Start () {
		timer = transform.FindChild("Timer").GetComponent<Text>();
		damageAmpText = transform.FindChild("DamageAmp").GetComponent<Text>();
		damageAmpStretch = damageAmpText.GetComponent<StretchText>();

		StartCoroutine(UpdateTimer());
		StartCoroutine(UpdateDamageAmp());
	}
	
	// Update is called once per frame
	void Update () {

	}

	IEnumerator UpdateTimer() {
		while (GameManager.S.gameState != GameStates.playing) {
			yield return null;
		}
		while (GameManager.S.gameState == GameStates.playing) {
			timer.text = seconds / 60 + ":" + ((seconds % 60 < 10) ? "0" : "") + seconds % 60;
			seconds++;
			yield return new WaitForSeconds(1);
		}
		Color endColor = timer.color;
		endColor.a = 0.4f;
		timer.color = endColor;
	}

	IEnumerator UpdateDamageAmp() {
		while (GameManager.S.gameState != GameStates.playing) {
			yield return null;
		}
		while (GameManager.S.gameState == GameStates.playing && GameManager.S.curDamageAmplification < GameManager.S.maxDamageAmplification) {
			float percent = (GameManager.S.curDamageAmplification-1) / (GameManager.S.maxDamageAmplification-1);

			damageAmpStretch.amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, percent);
			damageAmpStretch.frequency = Mathf.Lerp(minFrequency, maxFrequency, percent);

            damageAmpText.color = damageAmpGradient.Evaluate(percent);
			damageAmpText.text = "DMG X " + GameManager.S.curDamageAmplification.ToString("F1");
			yield return null;
		}
		while (GameManager.S.gameState == GameStates.playing) {
			yield return null;
		}
		Color endColor = damageAmpText.color;
		endColor.a = 0.4f;
		damageAmpText.color = endColor;
	}
}
