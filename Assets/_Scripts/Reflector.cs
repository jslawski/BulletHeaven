using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class Reflector : MonoBehaviour {
	ParticleSystem[] reflectorParticles;
	float reflectorDuration = 4f;
	float reflectionVelocity = 10f;

	public Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				//if (value == Player.player1) {
				//	reflectorParticles[0].Play();
				//}
				//else {
				//	reflectorParticles[1].Play();
				//}
				reflectorParticles[0].startColor = GameManager.S.players[(int)value].playerColor;
				reflectorParticles[0].Play();
			}
		}
	}

	void Awake() {
		SoundManager.instance.Play("Reflector");
		reflectorParticles = GetComponentsInChildren<ParticleSystem>();
		StartCoroutine(DestroyReflector());
	}

	IEnumerator DestroyReflector() {
		yield return new WaitForSeconds(reflectorDuration);
		Destroy(gameObject);
	}

	void OnTriggerStay(Collider other) {
		//Convert any bullet that enters the reflector, and shoot it back at the opponent
		if (other.tag == "Bullet" && other.gameObject.GetComponent<Bullet>().owningPlayer != owningPlayer) {
			float sprayRange = 3f;
			
			//Get the bullet that is getting reflected
			Bullet otherBullet = other.gameObject.GetComponent<Bullet>();
			Vector3 bulletPosition = otherBullet.gameObject.transform.position;

			//Get the opponent player value of the bullet
			Player otherPlayer = otherBullet.owningPlayer;
			if (otherPlayer != Player.none) {
				otherBullet.reflected = true;

				Vector3 otherPlayerPosition = GameManager.S.players[(int)otherPlayer].transform.position;

				//Determine reflection vector
				Vector3 sprayVector = new Vector3(0, Random.Range(-sprayRange, sprayRange), 0);
				Vector3 reflectionVector = (otherPlayerPosition - bulletPosition + sprayVector).normalized;

				//Reflect the bullet back at the opponent
				otherBullet.owningPlayer = owningPlayer;
				otherBullet.gameObject.GetComponent<PhysicsObj>().velocity = reflectionVector * reflectionVelocity;
			}
		}
	}
}
