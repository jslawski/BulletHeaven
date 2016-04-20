using UnityEngine;
using System.Collections;

public class StretchText : MonoBehaviour {

	// Use this for initialization
	void Start() {
		StartCoroutine(StretchTextCoroutine());
	}

	IEnumerator StretchTextCoroutine() {
		float timeElapsed = 0;
		float frequency = 0.5f;
		float amplitude = 0.1f;
		Vector3 startSize = transform.localScale;

		while (true) {
			timeElapsed += Time.deltaTime;

			transform.localScale = startSize * amplitude * (Mathf.Sin(2 * Mathf.PI * timeElapsed * frequency) + 1 + (1/amplitude));
			

			yield return 0;
		}
	}
}
