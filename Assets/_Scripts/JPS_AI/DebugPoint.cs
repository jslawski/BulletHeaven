using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPoint : MonoBehaviour {

	public Point pointReference;

	private SpriteRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.pointReference != null) {
			if (this.pointReference.dangerScore == 0) {
				myRenderer.color = Color.green;	
			}
			if (this.pointReference.dangerScore >= 0.2f) {
				myRenderer.color = Color.yellow;
			}
			if (this.pointReference.dangerScore >= 0.5f) {
				myRenderer.color = new Color(255f, 165f, 0f);	
			}
			if (this.pointReference.dangerScore >= 1) {
				myRenderer.color = Color.red;
			}
		}
	}
}
