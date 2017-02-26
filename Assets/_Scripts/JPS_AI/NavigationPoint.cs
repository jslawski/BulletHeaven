using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour {

	public Point pointReference;

	private SpriteRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<SpriteRenderer>();

		if (AIManager.debugMode == false) {
			myRenderer.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (AIManager.debugMode == true) {
			if (this.pointReference != null) {
				myRenderer.color = Color.green;	
				if (this.pointReference.dangerScore >= 0.3f) {
					myRenderer.color = Color.yellow;
				}
				if (this.pointReference.dangerScore >= 0.4f) {
					myRenderer.color = Color.magenta;	
				}
				if (this.pointReference.dangerScore >= 1) {
					myRenderer.color = Color.red;
				}
			}
		}
	}

	/*void OnTriggerEnter(Collider other){
		if (other.gameObject.GetComponent<Bullet>().owningPlayer != AIManager.controlledPlayer) {
			this.pointReference.dangerScore = 1;
		}		
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.GetComponent<Bullet>().owningPlayer != AIManager.controlledPlayer) {
			this.pointReference.dangerScore = 0;
		}
	}*/
}
