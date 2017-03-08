using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherFollow : MonoBehaviour {
	public Transform target;

	ParticleSystem ps;
	float extraLifetime = 1.5f;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 diffVector = target.position - transform.position;
		transform.up = Vector3.Normalize(diffVector);

		ParticleSystem.MainModule main = ps.main;
		float startSpeed = main.startSpeed.constant;
		main.startLifetime = diffVector.magnitude / startSpeed + extraLifetime;
	}
}
