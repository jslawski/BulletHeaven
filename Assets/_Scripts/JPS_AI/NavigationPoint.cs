using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationPoint : MonoBehaviour {

	private SpriteRenderer myRenderer;
	private string threatLayerName = "Bullet";

	public Vector3 coordinates;

	public List<NavigationPoint> adjacentPoints;
	public float[] previousDangerScores;

	private float _dangerScore;
	public float dangerScore {
		get{ return this._dangerScore; }
		set{ 
			this._dangerScore = value;

			if (AIManager.debugMode == true) {
				this.UpdateColors();
			}
		}
	}
		
	public void InitializePoint(Vector3 pointPosition, int scanIterations){
		this.coordinates = pointPosition; 
		this._dangerScore = 0;
		this.adjacentPoints = new List<NavigationPoint>();
		this.previousDangerScores = new float[scanIterations];
	}

	// Use this for initialization
	void Awake () {
		myRenderer = GetComponent<SpriteRenderer>();

		if (AIManager.debugMode == false) {
			myRenderer.enabled = false;
		}
	}
	
	private void UpdateColors() {
		myRenderer.color = Color.green;	
		if (this._dangerScore >= 0.3f) {
			myRenderer.color = Color.yellow;
		}
		if (this._dangerScore >= 0.4f) {
			myRenderer.color = Color.magenta;	
		}
		if (this._dangerScore >= 1) {
			myRenderer.color = Color.red;
		}
	}

	/*void OnTriggerEnter(Collider other){
		if (this.isBorderPoint == true) {
			return;
		}

		if (other.gameObject.layer == LayerMask.NameToLayer(this.threatLayerName)) {
			this.dangerScore = 1;
		}		
	}

	void OnTriggerExit(Collider other){
		if () th
		if (other.gameObject.layer == LayerMask.NameToLayer(this.threatLayerName)) {
			this.dangerScore = 0;
		}	
	}*/
}
