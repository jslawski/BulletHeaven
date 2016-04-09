using UnityEngine;
using System.Collections;

public class PhysicsObj : MonoBehaviour {
	public bool still = true;
	public bool affectedByGravity = true;
	public Vector3 acceleration, velocity, posNow, posNext;

	// Use this for initialization
	void Start () {
		PhysicsEngine.physicsObjects.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
