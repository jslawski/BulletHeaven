using UnityEngine;
using System.Collections;

public class TrailBullet : Bullet {
	[HideInInspector]
	public Transform target;
	[HideInInspector]
	public Bullet leadingBullet;
	float homingForce = 30f;

	float timeElapsed = 0f;
	float timeToReachFullSpeed = 0.5f;
	float startVelocity = 5f;
	float endVelocity = 20f;

	public void BeginHoming() {
		StartCoroutine(BeginHomingCoroutine());
	}

	IEnumerator BeginHomingCoroutine() {
		while (true) {
			if (curState != BulletState.none) {
				physics.acceleration = Vector3.zero;
				break;
			}

			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / timeToReachFullSpeed;
			physics.velocity = physics.velocity.normalized * Mathf.Lerp(startVelocity, endVelocity, percent);

			if (leadingBullet == null || leadingBullet.curState != BulletState.none) {
				Vector3 targetVector = (target.position - transform.position).normalized;
				physics.acceleration = targetVector * homingForce;
			}
			else {
				//Vector3 targetVector = (leadingBullet.position - transform.position).normalized;
				physics.velocity = Vector3.Lerp(physics.velocity, leadingBullet.gameObject.GetComponent<PhysicsObj>().velocity, 0.25f);
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
