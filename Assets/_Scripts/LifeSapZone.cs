using UnityEngine;
using System.Collections;

public class LifeSapZone : MonoBehaviour {
	public PlayerShip owner;

	Transform particle;
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

	float particleTravelTime = 0.5f;
	float timeElapsed = 0;

	float damagePerTick = 3f;			//Damage ticks every particleTravelTime seconds

	// Use this for initialization
	void Start () {
		particle = transform.FindChild("Particle");
		connectingLine = GetComponentInChildren<LineRenderer>();
		startPos = transform.position;
		linePositions = new Vector3[lineResolution];
		for (int i = 0; i < lineResolution; i++) {
			linePositions[i] = transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (targetShip == null) {
			return;
		}

		//Draw the line between the zone and the player
		connectingLine.SetPositions(GetPositions(targetShip.transform.position));
		UpdateParticle();

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
		if (other.tag == "Player") {
			if (other.GetComponentInParent<PlayerShip>() == targetShip) {
				playerInSapZone = false;
			}
		}
	}

	void StartTether(PlayerShip newTarget) {
		targetShip = newTarget;
		connectingLine.SetVertexCount(lineResolution);
		particle.gameObject.SetActive(true);

		StartCoroutine(DealDamageCoroutine());
	}
	void EndTether() {
		timeSincePlayerLeftZone = 0;
		targetShip = null;
		connectingLine.SetVertexCount(0);

		//Reset the line positions for next entry into the zone
		for (int i = 0; i < lineResolution; i++) {
			linePositions[i] = transform.position;
		}

		particle.gameObject.SetActive(false);
		timeElapsed = 0;

		StopCoroutine(DealDamageCoroutine());
	}

	Vector3[] GetPositions(Vector3 otherPos) {
		for (int i = 0; i < lineResolution; i++) {
			float percent = (float)i / lineResolution;
			Vector3 prevPos = linePositions[i];
			Vector3 newPos = Vector3.Lerp(startPos, otherPos, (float)i / lineResolution);
			//Positions closer to the target player lerp more quickly to their new position than
			//positions closer to the center of the zone (leads to the swerving effect of the line)
			linePositions[i] = Vector3.Lerp(prevPos, newPos, Mathf.Lerp(minTetherLerp, maxTetherLerp, percent*percent));
		}

		return linePositions;
	}

	void UpdateParticle() {
		timeElapsed += Time.deltaTime;
		//Move the particle along the vertices based on time elapsed
		float vertex = Mathf.Lerp(lineResolution-1, 0, (timeElapsed%particleTravelTime)/particleTravelTime);
		//Lerps between vertices so that it doesn't "jump" from vertex to vertex
		particle.position = Vector3.Lerp(linePositions[(int)vertex], linePositions[(int)vertex+1], vertex%1f);

		particle.localScale = Vector3.one * Mathf.Lerp(0.1f, 3f, vertex / lineResolution);
	}

	IEnumerator DealDamageCoroutine() {
		while (targetShip != null) {
			targetShip.TakeDamage(damagePerTick);
			owner.TakeDamage(-damagePerTick);
			yield return new WaitForSeconds(particleTravelTime);
		}
	}
}
