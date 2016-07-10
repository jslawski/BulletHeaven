using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeOption : OptionMenuItem {
	[SerializeField]
	Text label;

	public enum TypeOfVolume {
		master,
		music,
		sfx
	}
	public TypeOfVolume typeOfVolume;
	int minValue = 0;
	int maxValue = 10;

	int _curValue = 10;
	int curValue {
		get {
			return _curValue;
		}
		set {
			_curValue = value;
			SetBars(value);

			float optionsValue = (float)value/10f;
			switch (typeOfVolume) {
				case TypeOfVolume.master:
					Options.masterVolume = optionsValue;
					break;
				case TypeOfVolume.music:
					Options.musicVolume = optionsValue;
					break;
				case TypeOfVolume.sfx:
					Options.sfVolume = optionsValue;
					break;
			}
		}
	}

	Image[] volumeBar;
	Color[] defaultBarColors;

	Color inactiveColor = new Color(0.25f, 0.25f, 0.25f);
	Coroutine highlightedAnimation;
	Coroutine[] blockSwells;

	Coroutine textColorAnimation;
	Color highlightedTextColor = new Color(73/255f, 126/255f, 246f/255f);

	// Use this for initialization
	void Awake () {
		volumeBar = GetComponentsInChildren<Image>();
		defaultBarColors = new Color[volumeBar.Length];
		for (int i = 0; i < volumeBar.Length; i++) {
			defaultBarColors[i] = volumeBar[i].color;
		}
		blockSwells = new Coroutine[volumeBar.Length];
	}

	public override void SetOptionValue() {
		switch (typeOfVolume) {
			case TypeOfVolume.master:
				curValue = Mathf.RoundToInt(10f*Options.masterVolume);
				break;
			case TypeOfVolume.music:
				curValue = Mathf.RoundToInt(10f*Options.musicVolume);
				break;
			case TypeOfVolume.sfx:
				curValue = Mathf.RoundToInt(10f*Options.sfVolume);
				break;
		}
	}
	public override void SetOptionValue(float value) {
		curValue = Mathf.RoundToInt(value*10f);
	}

	public override void IncreaseOptionValue() {
		if (curValue == maxValue) {
			return;
		}

		curValue++;
	}

	public override void DecreaseOptionValue() {
		if (curValue == minValue) {
			return;
		}

		curValue--;
	}

	void SetBars(int numBarsLit) {
		//Light bars < numBarsLit
		for (int i = 0; i < numBarsLit && i < volumeBar.Length; i++) {
			volumeBar[i].color = defaultBarColors[i];
			volumeBar[i].transform.SetAsLastSibling();
		}

		//Darken bars > numBarsLit
		for (int i = numBarsLit; i < volumeBar.Length; i++) {
			volumeBar[i].color = inactiveColor;
			volumeBar[i].transform.SetAsFirstSibling();
		}
	}

	public override void PlayHighlightedAnimation() {
		highlightedAnimation = StartCoroutine(HighlightedAnimation());
	}

	void Update() {
		if (!selected && highlightedAnimation != null) {

			//Stop the parent coroutine
			StopCoroutine(highlightedAnimation);
			highlightedAnimation = null;

			//Stop the text color coroutine
			StopCoroutine(textColorAnimation);
			textColorAnimation = null;
			label.color = Color.white;

			//Stop the children coroutines
			for (int i = 0; i < blockSwells.Length; i++) {
				if (blockSwells[i] != null) {
					StopCoroutine(blockSwells[i]);
				}

				blockSwells[i] = null;
			}

			//Restore any sizes for the volume bar
			foreach (var block in volumeBar) {
				block.transform.localScale = Vector3.one;
			}
		}
	}

	IEnumerator HighlightedAnimation() {
		float repeatTime = 1f;
		float animTime = 0.5f;

		textColorAnimation = StartCoroutine(PulseLabelText());

		while (selected) {
			for (int i = 0; i < curValue; i++) {
				blockSwells[i] = (StartCoroutine(SwellBlock(volumeBar[i], i)));

				yield return new WaitForSeconds(animTime / volumeBar.Length);
			}

			yield return new WaitForSeconds(repeatTime);
		}
	}
	IEnumerator SwellBlock(Image block, int index) {
		float duration = 0.375f;
		Vector3 maxSize = Vector3.one * 1.5f;

		//Swell
		float timeElapsed = 0;
		while (timeElapsed < duration && selected) {
			timeElapsed += Time.deltaTime;

			block.transform.localScale = Vector3.Lerp(Vector3.one, maxSize, timeElapsed / duration);

			yield return null;
		}

		//De-swell
		timeElapsed = 0;
		while (timeElapsed < duration && selected) {
			timeElapsed += Time.deltaTime;

			block.transform.localScale = Vector3.Lerp(maxSize, Vector3.one, timeElapsed / duration);

			yield return null;
		}

		//Restore size in case we exited out early
		block.transform.localScale = Vector3.one;
		blockSwells[index] = null;
	}
	IEnumerator PulseLabelText() {
		float pulsePeriod = 1.5f;

		float timeElapsed = 0;
		Color startColor = Color.Lerp(Color.white, highlightedTextColor, 0.25f);
		while (selected) {
			timeElapsed += Time.deltaTime;

			float t = 0.5f + 0.5f*Mathf.Sin(2*Mathf.PI*timeElapsed/pulsePeriod);
			label.color = Color.Lerp(startColor, highlightedTextColor, t);

			yield return null;
		}

		label.color = Color.white;
	}
}
