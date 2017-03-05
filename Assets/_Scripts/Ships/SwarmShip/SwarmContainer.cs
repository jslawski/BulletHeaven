using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmContainer : Player {
	private List<SwarmShip> swarmShips;

	void AddShipToSwarm(SwarmShip newShip) {
		Debug.LogError("AddShipToSwarm not implemented yet");
	}
	void RemoveShipFromSwarm(SwarmShip shipToBeRemoved) {
		Debug.LogError("RemoveShipFromSwarm not implemented yet");
	}
	public List<SwarmShip> GetSwarmShips() {
		return swarmShips;
	}
}
