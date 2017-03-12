using UnityEngine;
using System.Collections;

public class BounceText : MonoBehaviour {

	public float amplitude = 5f;
	public float timeBeforeDirectionChange = 1.5f;

	private Coroutine bounceCoroutine;

	// Use this for initialization
	void Start() {
		this.bounceCoroutine = StartCoroutine(BounceTextCoroutine());
	}

	void OnDestroy(){
		StopCoroutine(this.bounceCoroutine);	
	}

	IEnumerator BounceTextCoroutine() {
		
		float timeElapsed = 0;

		Vector3 startPos = transform.position;

		while (true) {
			timeElapsed += Time.deltaTime;

			transform.position = startPos + Vector3.up * amplitude * Mathf.Sin(2 * Mathf.PI * timeElapsed / timeBeforeDirectionChange);

			yield return 0;
		}
	}
}