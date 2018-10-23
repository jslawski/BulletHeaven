using UnityEngine;
using System.Collections;

public class ChargeShot : MonoBehaviour {
	public PlayerEnum owningPlayer;
	public Ship playerShip;

	enum ChargeState {
		charging,
		charged,
		fired,
		cancelled
	}

	ParticleSystem chargeParticle;
	ParticleSystem shotParticle;

	float shotSpeed = 500f;
	float shotWidth = 0.3f;
	float chargeTime = 1.5f;
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

	KeyCode Y;

	// Use this for initialization
	void Start () {
		shotParticle = transform.Find("ShotParticles").GetComponent<ParticleSystem>();
		chargeParticle = transform.Find("ChargeParticles").GetComponent<ParticleSystem>();

		if (playerShip != null) {
			transform.SetParent(playerShip.transform, false);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.Euler(Vector3.zero);
		}

		Y = (owningPlayer == PlayerEnum.player1) ? KeyCode.Alpha4 : KeyCode.Keypad4;
		StartCoroutine(Charge());
	}
	
	// Update is called once per frame
	void Update () {

		if (playerShip.dead) {
			state = ChargeState.cancelled;
		}

		if (!GameManager.S.inGame) {
			return;
		}

		if (Input.GetKeyUp(Y) || (playerShip != null && playerShip.player.device != null && playerShip.player.device.Action4.WasReleased)) {
			//Cancel the charge if we're not ready yet
			if (state == ChargeState.charging) {
				CancelCharge();
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
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("ChargeAttackCharge");
		}
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

			playerShip.movement.SlowPlayer(Mathf.Lerp(1, maxChargeSlow, percent), 0, true);

			yield return null;
		}

		if (state != ChargeState.cancelled) {
			if (GameManager.S.inGame) {
				SoundManager.instance.Play("FullyCharged");
			}
			state = ChargeState.charged;
		}

		while (state == ChargeState.charged) {
			playerShip.movement.SlowPlayer(maxChargeSlow, 0, true);
			yield return null;
		}

		if (state == ChargeState.cancelled) {
			CancelCharge();
		}
	}

	void CancelCharge() {
		if (GameManager.S.inGame) {
			SoundManager.instance.Stop("ChargeAttackCharge");
			SoundManager.instance.Stop("FullyCharged");
		}
		state = ChargeState.cancelled;
		if (chargeParticle != null) {
			Destroy(chargeParticle.gameObject);
		}
		Destroy(gameObject, 2f);
		playerShip.movement.RestoreSpeed();
	}

	public void Fire() {
		state = ChargeState.fired;
		playerShip.shooting.ExpendAttackSlot();
		shotParticle.transform.SetParent(null, true);
		shotParticle.Play();

		Ray shot = new Ray(transform.position, transform.up);
		//See if we would hit anything by firing a bullet in this direction
		RaycastHit[] hitscans = Physics.SphereCastAll(shot, shotWidth, 50f);
		Debug.DrawRay(shot.origin, shot.direction * 50f, Color.blue, 10f);

		if (GameManager.S.inGame) {
			SoundManager.instance.Play("ChargeAttackShoot");
			SoundManager.instance.Stop("FullyCharged");
		}

		foreach (var hitscan in hitscans) {
			//If we hit anything with the hitscan
			if (hitscan.collider != null) {
				//Connect with players
				if (hitscan.collider.gameObject.tag == "Player") {
					Ship hitShip = hitscan.collider.gameObject.GetComponentInParent<Ship>();
					if (hitShip.playerEnum != owningPlayer) {
						StartCoroutine(DealDamageCoroutine(hitShip));
					}
				}
				//Connect with protag ships
				else if (hitscan.collider.gameObject.tag == "ProtagShip") {
					ProtagShip hitShip = hitscan.collider.gameObject.GetComponentInParent<ProtagShip>();
					StartCoroutine(DealDamageCoroutine(hitShip));
				}
			}
		}

		Destroy(chargeParticle.gameObject);
		Destroy(gameObject, 2f);
		playerShip.movement.RestoreSpeed();
	}

	IEnumerator DealDamageCoroutine(Ship hitShip) {
		float distance = (hitShip.transform.position - transform.position).magnitude;
		yield return new WaitForSeconds(distance / shotSpeed);

		hitShip.TakeDamage(damage);
	}
	IEnumerator DealDamageCoroutine(ProtagShip hitShip) {
		float distance = (hitShip.transform.position - transform.position).magnitude;
		yield return new WaitForSeconds(distance / shotSpeed);

		hitShip.TakeDamage(damage);
	}
}
