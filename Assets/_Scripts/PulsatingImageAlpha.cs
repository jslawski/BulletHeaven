using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PulsatingImageAlpha : MonoBehaviour {
	public float period;

	Image image;
	Color startColor;
	Color endColor;

	// Use this for initialization
	void Awake () {
		image = GetComponent<Image>();
		startColor = image.color;
		endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float t = 0.5f*Mathf.Sin(Time.realtimeSinceStartup * 2 * Mathf.PI / period) + 0.5f;
        image.color = Color.Lerp(startColor, endColor, t);
	}
}
