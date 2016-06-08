using UnityEngine;
using System.Collections;

public class RandomShipIcon : MonoBehaviour {
	SpriteRenderer parentImage;
	SpriteRenderer image;

	float iconAlphaValue = 0.5f;

	// Use this for initialization
	void Start () {
		parentImage = transform.parent.GetComponent<SpriteRenderer>();
		image = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		image.sortingOrder = parentImage.sortingOrder + 1;
		Color parentColor = parentImage.color;
		image.color = new Color(parentColor.r, parentColor.g, parentColor.b, iconAlphaValue * parentColor.a);
	}
}
