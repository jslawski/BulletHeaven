using UnityEngine;
using System.Collections;

public class ChargeShot : MonoBehaviour {
	public Player owningPlayer;
	public PlayerShip player;

	enum ChargeState {
		charging,
		charged,
		fired,
		cancelled
	}

	ParticleSystem chargeParticle;
	ParticleSystem shotParticle;

	float chargeTime = 3f;
	float maxChargeAngle = 30f;
	float minChargeAngle = 0f;
	float minChargeRotationSpeed = 1f;
	float maxChargeRotationSpeed = 10f;
	float maxStartSize = 1f;
	float minStartSize = 0.2f;
	float maxChargeSlow = 0.35f;
	Color startColor = new Color(1,1,1, 6f/255f);
	Color endColor = new Color(1, 86f/255f, 86f/255f, 82f/255f);

	ChargeState state;

	float damage = 40f;

	KeyCode B;

	// Use this for initialization
	void Start () {
		shotParticle = transform.FindChild("ShotParticles").GetComponent<ParticleSystem>();
		chargeParticle = transform.FindChild("ChargeParticles").GetComponent<ParticleSystem>();

		if (player != null) {
			transform.SetParent(player.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.Euler(Vector3.zero);
		}

		B = (owningPlayer == Player.player1) ? KeyCode.Alpha2 : KeyCode.Keypad2;
		StartCoroutine(Charge());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(B) || (player != null && player.device != null && player.device.Action2.WasReleased)) {
			//Cancel the charge if we're not ready yet
			if (state == ChargeState.charging) {
				state = ChargeState.cancelled;
				Destroy(chargeParticle.gameObject);
				Destroy(gameObject, 2f);
				player.playerMovement.RestoreSpeed();
			}
			else if (state == ChargeState.charged) {
				Fire();
			}
		}
	}

	IEnumerator Charge() {
		state = ChargeState.charging;
		float timeElapsed = 0f;
		ParticleSystem.ShapeModule shape = chargeParticle.shape;

		//Set initial values before playing
		shape.radius = 1f;
		shape.angle = maxChargeAngle;
		chargeParticle.startSize = maxStartSize;
		chargeParticle.startColor = startColor;

		chargeParticle.Play();
		while (timeElapsed < chargeTime && state == ChargeState.charging) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/chargeTime;

			//Animate the charge effect
			shape.radius = Mathf.Lerp(1, 0, percent);
			shape.angle = Mathf.Lerp(maxChargeAngle, minChargeAngle, percent*percent);
			Vector3 curRot = chargeParticle.transform.localRotation.eulerAngles;
			curRot.z += Mathf.Lerp(minChargeRotationSpeed, maxChargeRotationSpeed, percent*percent);
			chargeParticle.transform.localRotation = Quaternion.Euler(curRot);

			chargeParticle.startSize = Mathf.Lerp(maxStartSize, minStartSize, percent * percent);
			chargeParticle.startColor = Color.Lerp(startColor, endColor, percent * percent);

			player.playerMovement.SlowPlayer(Mathf.Lerp(1, maxChargeSlow, percent), 0, true);

			yield return null;
		}

		if (state != ChargeState.cancelled) {
			state = ChargeState.charged;
		}

		while (state == ChargeState.charged) {
			player.playerMovement.SlowPlayer(maxChargeSlow, 0, true);
			yield return null;
		}
	}

	void Fire() {
		state = ChargeState.fired;
		shotParticle.Play();

		Ray shot = new Ray(transform.position, transform.up);
		//See if we would hit anything by firing a bullet in this direction
		RaycastHit[] hitscans = Physics.SphereCastAll(shot, 0.25f, 50f);
		Debug.DrawRay(shot.origin, shot.direction * 50f, Color.blue, 10f);

		foreach (var hitscan in hitscans) {
			//If we hit anything with the hitscan
			if (hitscan.collider != null) {
				//Connect with players
				if (hitscan.collider.gameObject.tag == "Player") {
					PlayerShip hitPlayer = hitscan.collider.gameObject.GetComponentInParent<PlayerShip>();
					if (hitPlayer.player != owningPlayer) {
						hitPlayer.TakeDamage(damage);
					}
				}
				//Connect with protag ships
				else if (hitscan.collider.gameObject.tag == "ProtagShip") {
					ProtagShip hitShip = hitscan.collider.gameObject.GetComponentInParent<ProtagShip>();
					hitShip.TakeDamage(damage);
				}
			}
		}

		Destroy(chargeParticle.gameObject);
		Destroy(gameObject, 2f);
		player.playerMovement.RestoreSpeed();
	}
}
