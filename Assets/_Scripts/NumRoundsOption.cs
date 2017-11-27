using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumRoundsOption : OptionMenuItem {
	int minValue = 1;
	int maxValue = 11;
	Image leftArrow, rightArrow;
	Text numRoundsTextField;

	float arrowColorLerpSpeed = 0.05f;
	Color pressedColor = new Color(0.1f, 0.1f, 0.1f);

	[SerializeField]
	Text label;
	Coroutine textColorAnimation;
	Color highlightedTextColor = new Color(73/255f, 126/255f, 246f/255f);

	Color numRoundsTextColorDefault;

	// Use this for initialization
	void Start () {
		leftArrow = transform.Find("LeftArrow").GetComponent<Image>();
		rightArrow = transform.Find("RightArrow").GetComponent<Image>();
		numRoundsTextField = GetComponentInChildren<Text>();
		numRoundsTextColorDefault = numRoundsTextField.color;
	}
	
	// Update is called once per frame
	void Update () {
		leftArrow.color = Color.Lerp(leftArrow.color, Color.white, arrowColorLerpSpeed);
		rightArrow.color = Color.Lerp(rightArrow.color, Color.white, arrowColorLerpSpeed);

		if (!selected && textColorAnimation != null) {
			//Stop the text color coroutine
			StopCoroutine(textColorAnimation);
			textColorAnimation = null;
			label.color = Color.white;
			numRoundsTextField.color = numRoundsTextColorDefault;
		}
	}

	public override void SetOptionValue() {
		numRoundsTextField.text = Options.numRounds.ToString();
	}

	public override void SetOptionValue(int value) {
		numRoundsTextField.text = value.ToString();
		Options.numRounds = value;
	}

	public override void PlayHighlightedAnimation() {
		textColorAnimation = StartCoroutine(PulseLabelText());
	}

	public override void IncreaseOptionValue() {
		int curValue = int.Parse(numRoundsTextField.text);
		if (curValue + 2 <= maxValue) {
			curValue += 2;
			numRoundsTextField.text = curValue.ToString();
			Options.numRounds = curValue;

			rightArrow.color = pressedColor;
		}
	}
	public override void DecreaseOptionValue() {
		int curValue = int.Parse(numRoundsTextField.text);
		if (curValue - 2 >= minValue) {
			curValue -= 2;
			numRoundsTextField.text = curValue.ToString();
			Options.numRounds = curValue;

			leftArrow.color = pressedColor;
		}
	}
	IEnumerator PulseLabelText() {
		float pulsePeriod = 1.5f;

		float timeElapsed = 0;
		Color startColor = Color.Lerp(Color.white, highlightedTextColor, 0.25f);
		while (selected) {
			timeElapsed += Time.deltaTime;

			float t = 0.5f + 0.5f*Mathf.Sin(2*Mathf.PI*timeElapsed/pulsePeriod);
			label.color = Color.Lerp(startColor, highlightedTextColor, t);
			numRoundsTextField.color = Color.Lerp(startColor, highlightedTextColor, t);

			yield return null;
		}

		label.color = Color.white;
	}
}
