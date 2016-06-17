using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldBar : MonoBehaviour {
	public Transform target;
	RectTransform thisRect;

	Vector2 offset = new Vector2(0, -0.1f);
	Vector2 size;

	void Awake() {
		thisRect = GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
		float width = thisRect.anchorMax.x - thisRect.anchorMin.x;
		float height = thisRect.anchorMax.y - thisRect.anchorMin.y;
		size = new Vector2(width, height);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 center = Camera.main.WorldToViewportPoint(target.position);
		thisRect.anchorMin = (Vector2)center + offset;
		thisRect.anchorMax = (Vector2)center + size + offset;
	}
}
