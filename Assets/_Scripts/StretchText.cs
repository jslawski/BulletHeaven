using UnityEngine;
using System.Collections;

public class StretchText : MonoBehaviour {

	public float frequency = 0.5f;
	public float amplitude = 0.1f;

	// Use this for initialization
	void Start() {
		StartCoroutine(StretchTextCoroutine());
	}

	IEnumerator StretchTextCoroutine() {
		float timeElapsed = 0;

		Vector3 startSize = transform.localScale;

		while (true) {
			timeElapsed += Time.deltaTime;

			if (amplitude == 0) {
				yield return 0;
				continue;
			}
			transform.localScale = startSize * amplitude * (Mathf.Sin(2 * Mathf.PI * timeElapsed * frequency) + 1 + (1/amplitude));
			

			yield return 0;
		}
	}
}
