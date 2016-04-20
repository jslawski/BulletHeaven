using UnityEngine;
using System.Collections;

public class StretchText : MonoBehaviour {

	// Use this for initialization
	void Start() {
		StartCoroutine(StretchTextCoroutine());
	}

	IEnumerator StretchTextCoroutine() {
		float timeBeforeScaleChange = 1.5f;
		float timeElapsed = 0;

		float magnitude = 0.002f;

		while (true) {
			timeElapsed += Time.deltaTime;

			transform.localScale = new Vector3(transform.localScale.x + magnitude, transform.localScale.y + magnitude, transform.localScale.z + magnitude);

			if (timeElapsed >= timeBeforeScaleChange) {
				magnitude *= -1;
				timeElapsed = 0;
			}

			yield return 0;
		}
	}
}
