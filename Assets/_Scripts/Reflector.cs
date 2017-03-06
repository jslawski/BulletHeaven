using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class Reflector : MonoBehaviour, BombAttack {
	ParticleSystem[] reflectorParticles;
	float reflectorDuration = 4f;
	float reflectionVelocity = 10f;

	public Player thisPlayer;
	PlayerEnum _owningPlayer = PlayerEnum.none;
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != PlayerEnum.none) {
				//if (value == Player.player1) {
				//	reflectorParticles[0].Play();
				//}
				//else {
				//	reflectorParticles[1].Play();
				//}
				if (GameManager.S.inGame) {
					PlayerEnum other = (value == PlayerEnum.player1) ? PlayerEnum.player2 : PlayerEnum.player1;
					otherPlayer = GameManager.S.players[(int)other];
					SetColor(GameManager.S.players[(int)value].playerColor);
				}
			}
		}
	}
	public Player otherPlayer;

	void Awake() {
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("Reflector");
		}
		reflectorParticles = GetComponentsInChildren<ParticleSystem>();
		StartCoroutine(DestroyReflector());
	}

	public void FireBurst() {
		//This does nothing to appease the interface
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

			if (!otherBullet.IsInteractable()) {
				return;
			}

			//Get the opponent player value of the bullet
			if (otherPlayer != null) {
				otherBullet.curState = BulletState.reflected;

				Vector3 otherPlayerPosition = otherPlayer.ship.transform.position;

				//Determine reflection vector
				Vector3 sprayVector = new Vector3(0, Random.Range(-sprayRange, sprayRange), 0);
				Vector3 reflectionVector = (otherPlayerPosition - bulletPosition + sprayVector).normalized;

				//Reflect the bullet back at the opponent
				otherBullet.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					otherBullet.SetColor(thisPlayer.playerColor);
				}
				otherBullet.gameObject.GetComponent<PhysicsObj>().velocity = reflectionVector * reflectionVelocity;
			}
		}
	}

	public void SetColor(Color newColor) {
		reflectorParticles[0].startColor = newColor;
		reflectorParticles[0].Play();
	}
}
