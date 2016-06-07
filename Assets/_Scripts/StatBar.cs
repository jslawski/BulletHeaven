using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatBar : MonoBehaviour {
	Image[] statBars;
	Color fillColor = Color.green;
	Color unfilledColor = new Color(118f/255f,118f/255f,118f/255f);

	bool inStatValueCoroutine = false;
	float minWaitTime = 0.025f;
	float maxWaitTime = .125f;

	// Use this for initialization
	void Start () {
		statBars = GetComponentsInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetStatValue(int stat) {//, Color fillColor) {
		if (inStatValueCoroutine) {
			inStatValueCoroutine = false;
			StopAllCoroutines();
		}
		StartCoroutine(SetStatValueCoroutine(stat));
	}

	IEnumerator SetStatValueCoroutine(int stat) {
		inStatValueCoroutine = true;

		for (int i = 0; i < statBars.Length; i++) {
			statBars[i].color = unfilledColor;
		}

		float waitTime = minWaitTime;
		for (int i = 0; i < stat; i++) {
			statBars[i].color = fillColor;
			yield return new WaitForSeconds(waitTime);
			waitTime = Mathf.Lerp(minWaitTime, maxWaitTime, (float)i / stat);
		}

		inStatValueCoroutine = false;
	}
}
