using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DurationBar : MonoBehaviour {
	public Transform target;
	RectTransform thisRect;

	Transform durationBar;
	Transform durationBarBackground;

	Vector2 offset = new Vector2(0f, -0.2f);
	Vector2 size;

	public void SetPercent(float percent) {
		DurationBarEnabled(percent > 0);
		Vector3 curScale = durationBar.localScale;
		curScale.x = percent;
		durationBar.localScale = curScale;
	}

	void DurationBarEnabled(bool enabled) {
		foreach (var image in GetComponentsInChildren<Image>()) {
			image.enabled = enabled;
		}
	}

	public void SetColor(Color playerColor) {
		durationBar.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.2f);
		durationBarBackground.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.7f);
	}

	void Awake() {
		thisRect = GetComponent<RectTransform>();
		durationBar = transform.Find("DurationBar");
		durationBarBackground = transform.Find("DurationBarBackground");
	}

	// Use this for initialization
	void Start () {
		float width = thisRect.anchorMax.x - thisRect.anchorMin.x;
		float height = thisRect.anchorMax.y - thisRect.anchorMin.y;
		size = new Vector2(width, height);
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			Vector3 center = Camera.main.WorldToViewportPoint(target.position);
			thisRect.anchorMin = (Vector2)center + offset;
			thisRect.anchorMax = (Vector2)center + size + offset;
		}
	}
}
