using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatBar : MonoBehaviour {
	Image[] statBars;
	Color unfilledColor = new Color(118f/255f,118f/255f,118f/255f);
	Color[] randomColors;

	bool inStatValueCoroutine = false;
	bool inRandomStatCoroutine = false;
	float minWaitTime = 0.025f;
	float maxWaitTime = .125f;

	// Use this for initialization
	void Awake () {
		statBars = GetComponentsInChildren<Image>();
		randomColors = new Color[5];
		randomColors[0] = new Color(1, 11f / 255f, 11f / 255f);
		randomColors[1] = new Color(161f / 255f, 67f / 255f, 186f / 255f);
		randomColors[2] = new Color(0, 250f / 255f, 15f / 255f);
		randomColors[3] = new Color(254f / 255f, 226f / 255f, 56f / 255f);
		randomColors[4] = new Color(57f / 255f, 155f / 255f, 234f / 255f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetStatValue(int stat, Color fillColor) {
		if (inStatValueCoroutine || inRandomStatCoroutine) {
			inStatValueCoroutine = false;
			inRandomStatCoroutine = false;
			StopAllCoroutines();
		}
		StartCoroutine(SetStatValueCoroutine(stat, fillColor));
	}

	IEnumerator SetStatValueCoroutine(int stat, Color fillColor, bool useRandomColors=false) {
		inStatValueCoroutine = true;

		for (int i = 0; i < statBars.Length; i++) {
			statBars[i].color = unfilledColor;
		}

		float waitTime = minWaitTime;
		for (int i = 0; i < stat; i++) {
			if (useRandomColors) {
				statBars[i].color = randomColors[Random.Range(0, randomColors.Length)];
			}
			else {
				statBars[i].color = fillColor;
			}
			yield return new WaitForSeconds(waitTime);
			waitTime = Mathf.Lerp(minWaitTime, maxWaitTime, (float)i / stat);
		}

		inStatValueCoroutine = false;
	}

	public void AnimateRandomStats() {
		if (inStatValueCoroutine || inRandomStatCoroutine) {
			inStatValueCoroutine = false;
			inRandomStatCoroutine = false;
			StopAllCoroutines();
		}
		StartCoroutine(RandomStatsCoroutine());
	}

	IEnumerator RandomStatsCoroutine() {
		yield return null;
		inRandomStatCoroutine = true;
		while (true) {
			StartCoroutine(SetStatValueCoroutine(Random.Range(1, 11), randomColors[Random.Range(0, randomColors.Length)])); 
            //StartCoroutine(SetStatValueCoroutine(Random.Range(1, 11), Color.white, true));
			while (inStatValueCoroutine) {
				yield return null;
			}
			yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
		}
	}
}
