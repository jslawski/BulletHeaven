using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmShip : PlayerShip {
	private List<SwarmSingleShip> swarmShips = new List<SwarmSingleShip>();

	void AddShipToSwarm(SwarmSingleShip newShip) {
		Debug.LogError("AddShipToSwarm not implemented yet");
		swarmShips.Add(newShip);
		newShip.transform.SetParent(transform.parent, true);

		for (int i = 0; i < swarmShips.Count; i++) {
			swarmShips[i].swarmShipMovement.UpdateSwarmFormation(SwarmSingleShipMovement.Formation.triangle, i);
		}
	}
	void RemoveShipFromSwarm(SwarmSingleShip shipToBeRemoved) {
		Debug.LogError("RemoveShipFromSwarm not implemented yet");
	}
	public List<SwarmSingleShip> GetSwarmShips() {
		return swarmShips;
	}

	protected override void Awake() {
		movement = GetComponent<ShipMovement>();
		shooting = GetComponent<ShootBomb>();

		//Create a new GameObject to be the parent to both the swarm container and the individual ships
		//This is so that children movement isn't tightly tied to container movement
		GameObject parent = new GameObject();
		parent.transform.position = transform.position;
		parent.transform.rotation = transform.rotation;
		parent.name = "SwarmShipParent";
		parent.transform.SetParent(transform.parent, true);
		this.transform.SetParent(parent.transform);
	}

	protected override void Start() {
		base.Start();

		foreach (SwarmSingleShip newShip in GetComponentsInChildren<SwarmSingleShip>()) {
			AddShipToSwarm(newShip);
		}
	}
}
