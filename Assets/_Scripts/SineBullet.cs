using UnityEngine;
using System.Collections;

public class SineBullet : Bullet {

	float amplitude = 10f;
	float period = 0.5f;

	public void ApplySineWave(int waveDirection) {
		//For readability in the coroutine call
		PhysicsObj thisPhysicsObj = GetComponent<PhysicsObj>();

        StartCoroutine(ApplySineWaveCoroutine(thisPhysicsObj, waveDirection));
	}

	IEnumerator ApplySineWaveCoroutine(PhysicsObj thisPhysicsObj, int waveDirection) {
		Vector3 perpendicularDirection;
		Vector3 startingVelocity = thisPhysicsObj.velocity;

		//Get perpendicular direction where new velocity will be applied
		perpendicularDirection = Vector3.Cross(thisPhysicsObj.velocity, Vector3.forward).normalized;

		//Apply variable additional velocity based on a sine pattern
		float t = 0;
		while (true) {
			if (CheckFlags()) {
				break;
			}
			t += Time.fixedDeltaTime;
			Vector3 newVelocity = perpendicularDirection * (waveDirection * amplitude * Mathf.Cos(2*Mathf.PI * t/period));
			thisPhysicsObj.velocity = startingVelocity + newVelocity;
			yield return new WaitForFixedUpdate();
		}

	}
}
