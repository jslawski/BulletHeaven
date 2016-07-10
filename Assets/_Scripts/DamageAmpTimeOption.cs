using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageAmpTimeOption : OptionMenuItem {
	int minValue = 15;
	int maxValue = 300;
	int valueDelta = 15;

	int curValue {
		get {
			string[] splitStrings = ampTimeTextField.text.Split(':');
			int minutes = int.Parse(splitStrings[0]);
			int seconds = int.Parse(splitStrings[1]);
			return 60 * minutes + seconds;
		}
		set {
			SetOptionValue(value);
		}
	}

	Image leftArrow, rightArrow;
	Text ampTimeTextField;

	float arrowColorLerpSpeed = 0.05f;
	Color pressedColor = new Color(0.1f, 0.1f, 0.1f);

	[SerializeField]
	Text label;
	Coroutine textColorAnimation;
	Color highlightedTextColor = new Color(73/255f, 126/255f, 246f/255f);

	Color ampTimeTextColorDefault;

	// Use this for initialization
	void Start() {
		leftArrow = transform.FindChild("LeftArrow").GetComponent<Image>();
		rightArrow = transform.FindChild("RightArrow").GetComponent<Image>();
		ampTimeTextField = GetComponentInChildren<Text>();
		ampTimeTextColorDefault = ampTimeTextField.color;
	}

	// Update is called once per frame
	void Update() {
		leftArrow.color = Color.Lerp(leftArrow.color, Color.white, arrowColorLerpSpeed);
		rightArrow.color = Color.Lerp(rightArrow.color, Color.white, arrowColorLerpSpeed);

		if (!selected && textColorAnimation != null) {
			//Stop the text color coroutine
			StopCoroutine(textColorAnimation);
			textColorAnimation = null;
			label.color = Color.white;
			ampTimeTextField.color = ampTimeTextColorDefault;
		}
	}

	public override void SetOptionValue() {
		int minutes = Mathf.FloorToInt(Options.damageAmpTime/60);
		int seconds = Mathf.RoundToInt(Options.damageAmpTime%60);
		ampTimeTextField.text = minutes.ToString() + ":" + seconds.ToString() + ((seconds == 0) ? "0" : "");
	}
	public override void SetOptionValue(int value) {
		int minutes = value/60;
		int seconds = value%60;
		ampTimeTextField.text = minutes.ToString() + ":" + seconds.ToString() + ((seconds == 0) ? "0" : "");
		Options.damageAmpTime = value;
	}

	public override void PlayHighlightedAnimation() {
		textColorAnimation = StartCoroutine(PulseLabelText());
	}

	public override void IncreaseOptionValue() {
		if (curValue == maxValue) {
			return;
		}
		curValue += valueDelta;

		rightArrow.color = pressedColor;
	}
	public override void DecreaseOptionValue() {
		if (curValue == minValue) {
			return;
		}
		curValue -= valueDelta;

		leftArrow.color = pressedColor;
	}
	IEnumerator PulseLabelText() {
		float pulsePeriod = 1.5f;

		float timeElapsed = 0;
		Color startColor = Color.Lerp(Color.white, highlightedTextColor, 0.25f);
		while (selected) {
			timeElapsed += Time.deltaTime;

			float t = 0.5f + 0.5f*Mathf.Sin(2*Mathf.PI*timeElapsed/pulsePeriod);
			label.color = Color.Lerp(startColor, highlightedTextColor, t);
			ampTimeTextField.color = Color.Lerp(startColor, highlightedTextColor, t);

			yield return null;
		}

		label.color = Color.white;
	}
}
