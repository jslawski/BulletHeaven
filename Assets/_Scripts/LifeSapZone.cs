using UnityEngine;
using System.Collections;

public class LifeSapZone : MonoBehaviour {
	LineRenderer connectingLine;
	Vector3 startPos;
	int lineResolution = 40;           //Number of discrete points on the line
	Vector3[] linePositions;

	PlayerShip targetShip;
	bool playerInSapZone = false;
	float tetherReleaseTime = 1f;       //How long after the player leaves the sap zone the player stays tethered
	float timeSincePlayerLeftZone = 0f;

	float minTetherLerp = 0.01f;
	float maxTetherLerp = 0.2f;

	// Use this for initialization
	void Start () {
		connectingLine = GetComponentInChildren<LineRenderer>();
		startPos = transform.position;
		linePositions = new Vector3[lineResolution];
	}
	
	// Update is called once per frame
	void Update () {
		if (targetShip == null) {
			return;
		}

		//Draw the line between the zone and the player
		connectingLine.SetPositions(GetPositions(targetShip.transform.position));

		//If the player leaves the sap zone, wait tetherReleaseTime seconds before releasing the tether on the player
		if (!playerInSapZone) {
			timeSincePlayerLeftZone += Time.deltaTime;
			if (timeSincePlayerLeftZone > tetherReleaseTime) {
				EndTether();
			}
		}

	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			StartTether(other.GetComponentInParent<PlayerShip>());
			playerInSapZone = true;
		}
	}

	void OnTriggerExit(Collider other) {
		playerInSapZone = false;
	}

	void StartTether(PlayerShip newTarget) {
		targetShip = newTarget;
		connectingLine.SetVertexCount(lineResolution);
	}
	void EndTether() {
		timeSincePlayerLeftZone = 0;
		targetShip = null;
		connectingLine.SetVertexCount(0);

		//Reset the line positions for next entry into the zone
		for (int i = 0; i < lineResolution; i++) {
			linePositions[i] = transform.position;
		}
	}

	Vector3[] GetPositions(Vector3 otherPos) {
		for (int i = 0; i < lineResolution; i++) {
			Vector3 prevPos = linePositions[i];
			Vector3 newPos = Vector3.Lerp(startPos, otherPos, (float)i / lineResolution);
			//Positions closer to the target player lerp more quickly to their new position than
			//positions closer to the center of the zone (leads to the swerving effect of the line)
			linePositions[i] = Vector3.Lerp(prevPos, newPos, Mathf.Lerp(minTetherLerp, maxTetherLerp, (float)i/lineResolution));
		}

		return linePositions;
	}
}
