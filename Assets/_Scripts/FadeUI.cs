using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {

	Image uiImage;
	float fadeDuration = 30f;
	float fadeThreshold = 0.25f;

	// Use this for initialization
	void Start () {
		uiImage = GetComponent<Image>();
		StartCoroutine(Fade());
	}
	
	IEnumerator Fade() {
		for (float i = 1; i > fadeThreshold; i -= Time.fixedDeltaTime / fadeDuration) {
			uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, i);
			yield return new WaitForFixedUpdate();
		}
	}
}
