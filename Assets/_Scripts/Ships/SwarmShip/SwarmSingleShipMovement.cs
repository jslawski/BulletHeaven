using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSingleShipMovement : MonoBehaviour {
	public enum Formation {
		triangle
	}
	static float horizontalSpacing = 2f;
	static float verticalSpacing = 3f;

	SwarmShip swarm;
	Vector3 offset;

	// Use this for initialization
	void Awake () {
		swarm = GetComponentInParent<SwarmShip>();
	}

	public void UpdateSwarmFormation(Formation formation, int index) {
		int swarmSize = swarm.GetSwarmShips().Count;

		print("i: " + index + "\noffset: " + offset);
	}
}
