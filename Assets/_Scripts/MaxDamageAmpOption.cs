using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaxDamageAmpOption : OptionMenuItem {
	float minValue = 0.5f;
	float maxValue = 5f;
	float valueDelta = 0.1f;
	Image leftArrow, rightArrow;
	Text maxDamageAmpTextField;

	float arrowColorLerpSpeed = 0.05f;
	Color pressedColor = new Color(0.1f, 0.1f, 0.1f);

	public float curValue {
		get {
			return float.Parse(maxDamageAmpTextField.text);
		}
		set {
			if (value < minDamageAmpOption.curValue) {
				minDamageAmpOption.SetOptionValue(value);
			}
			SetOptionValue(value);
		}
	}

	[SerializeField]
	Text label;
	Coroutine textColorAnimation;
	Color highlightedTextColor = new Color(73/255f, 126/255f, 246f/255f);
	
	[SerializeField]
	MinDamageAmpOption minDamageAmpOption;

	Color maxDamageAmpTextColorDefault;

	// Use this for initialization
	void Start() {
		leftArrow = transform.FindChild("LeftArrow").GetComponent<Image>();
		rightArrow = transform.FindChild("RightArrow").GetComponent<Image>();
		maxDamageAmpTextField = GetComponentInChildren<Text>();
		maxDamageAmpTextColorDefault = maxDamageAmpTextField.color;
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
			maxDamageAmpTextField.color = maxDamageAmpTextColorDefault;
		}
	}

	public override void SetOptionValue() {
		maxDamageAmpTextField.text = Options.maxDamageAmp.ToString("F1");
	}
	public override void SetOptionValue(float value) {
		maxDamageAmpTextField.text = value.ToString("F1");
		Options.maxDamageAmp = value;
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
			maxDamageAmpTextField.color = Color.Lerp(startColor, highlightedTextColor, t);

			yield return null;
		}

		label.color = Color.white;
	}
}
