using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageValues : MonoBehaviour {
	[SerializeField]
	Gradient damageGradient;
	Text textField;
	StretchText damageStretch;

	float timeUntilReset = 0;
	float resetTimeout = 2.5f;
	float curDamage = 0;

	float minDamageColor = 2f;
	float maxDamageColor = 40f;
	float minFrequency = 0.2f;
	float maxFrequency = 2f;
	float minAmplitude = 0f;
	float maxAmplitude = 0.1f;

	// Use this for initialization
	void Awake () {
		textField = GetComponent<Text>();
		damageStretch = GetComponent<StretchText>();
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilReset -= Time.deltaTime;
		Color curColor = textField.color;
		curColor.a = timeUntilReset / resetTimeout;
		textField.color = curColor;
		if (timeUntilReset < 0) {
			curDamage = 0;
			textField.text = "0";
		}
	}

	public void DisplayDamage(float damageIn) {
		timeUntilReset = resetTimeout;
		curDamage += damageIn;
		textField.text = Mathf.RoundToInt(-10f*curDamage).ToString();

		float percent = Mathf.InverseLerp(minDamageColor, maxDamageColor, curDamage);
        textField.color = damageGradient.Evaluate(percent);
		damageStretch.amplitude = Mathf.Lerp(minAmplitude, maxAmplitude, percent);
		damageStretch.frequency = Mathf.Lerp(minFrequency, maxFrequency, percent);
	}
}
