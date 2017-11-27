using UnityEngine;
using System.Collections;

public class FinishAttack : MonoBehaviour {
	public KeyCode fireKey;
	PlayerEnum _owningPlayer = PlayerEnum.none;
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != PlayerEnum.none) {
				thisPlayer = GameManager.S.players[(int)value];
				target = GameManager.S.OtherPlayerShip(thisPlayer.character).GetClosestShip(transform.position).transform;
				startingGradientValue = FindClosestGradientValue(thisPlayer.playerColor);
			}
		}
	}
	Player thisPlayer;
	Transform target;
	public Gradient beamPulse;
	float startingGradientValue = 0;
	ParticleSystem charge;
	ParticleSystem background;
	ParticleSystem explosion;

	public bool hasBeenFired;

	float chargeTime = 2f;

	float startSpeed = -10f;
	float endSpeed = 0.1f;
	//float startRadius = 10f;
	//float endRadius = 2f;
	float startSize = 10f;
	float endSize = 4f;
	float startEmissiveRate = 750f;
	float endEmissiveRate = 250f;
	float backgroundStartLengthScale = 1000f;
	float backgroundEndLengthScale = 20f;

	float numPulses = 2f;
	float pulseTime = 0.75f;

	bool hasReachedDestination = false;
	float laserSpeed = 10f;

	float explosionDuration = 2f;

	// Use this for initialization
	void Start () {
		AudioSource[] sounds = thisPlayer.gameObject.GetComponentsInChildren<AudioSource>();

		charge = transform.Find("Charge").GetComponent<ParticleSystem>();
		background = transform.Find("BackgroundEffect").GetComponent<ParticleSystem>();
		explosion = transform.Find("MassiveExplosion").GetComponent<ParticleSystem>();
	}

	float FindClosestGradientValue(Color targetColor) {
		float delta = 0.02f;
		float closestGradientValue = 0;
		float minDistance = Mathf.Infinity;

		//Find the closest color by distance (what?)
		Vector3 c2 = new Vector3(targetColor.r, targetColor.g, targetColor.b);
		for (float i = 0; i < 1; i += delta) {
			Color curColor = beamPulse.Evaluate(i);
			Vector3 c1 = new Vector3(curColor.r, curColor.g, curColor.b);
			//Distance between 2 colors
			float distance = (c1-c2).magnitude;
			if (distance < minDistance) {
				minDistance = distance;
				closestGradientValue = i;
			}
		}
		return closestGradientValue;
	}

	// Update is called once per frame
	void Update () {
		if (owningPlayer == PlayerEnum.none || hasBeenFired) {
			return;
		}

		if (thisPlayer.device == null) {
			if (Input.GetKeyDown(fireKey)) {
				StartCoroutine(FinalAttack());
			}
		}
		//Controller input
		else {
			//Emergency bumper controls
			if (GameManager.emergencyBumperControls) {
				if (thisPlayer.device.RightBumper.IsPressed && thisPlayer.device.LeftBumper.IsPressed) {
					StartCoroutine(FinalAttack());
				}
			}
			//Normal controls
			else if (thisPlayer.device.RightTrigger.IsPressed && thisPlayer.device.LeftTrigger.IsPressed) {
				StartCoroutine(FinalAttack());
			}
		}
	}

	IEnumerator FinalAttack() {
		hasBeenFired = true;

		GameManager.S.gameState = GameStates.finalAttack;
		Player attackingPlayer = GameManager.S.players[(int)owningPlayer];
		attackingPlayer.character.ApplyToAllShips(ship => ship.movement.movementDisabled = true);
		attackingPlayer.finishAttackPrompt.SetActive(false);

		//Move the attack into the right position before beginning
		transform.position = attackingPlayer.character.transform.position + attackingPlayer.character.transform.up * 4.5f;

		//Tell the camera to start following this projectile
		CameraEffects.S.followObj = transform;

		StartCoroutine(Rumble());
		charge.Play();
		background.Play();
		float timeElapsed = 0;

		Color startColor = Color.black;
		Color endColor = beamPulse.Evaluate(startingGradientValue);

		ParticleSystemRenderer backgroundRenderer = background.GetComponent<ParticleSystemRenderer>();
		
		SoundManager.instance.Play("FinalShotCharge");

		//Charge the laser
		while (timeElapsed < chargeTime) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / chargeTime;

			charge.startSpeed = Mathf.Lerp(startSpeed, endSpeed, percent);
			charge.emissionRate = Mathf.Lerp(startEmissiveRate, endEmissiveRate, percent);
			charge.startColor = Color.Lerp(startColor, endColor, percent);
			charge.startSize = Mathf.Lerp(startSize, endSize, percent);

			backgroundRenderer.lengthScale = Mathf.Lerp(backgroundStartLengthScale, backgroundEndLengthScale, Mathf.Sqrt(percent));
			//background.emissionRate = Mathf.Lerp(startEmissiveRate, endEmissiveRate, percent);
			background.startColor = Color.Lerp(startColor, endColor, percent);

			//This is a hack to get around a non-modifiable property in the particle system settings
			//UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(charge);
			//so.FindProperty("ShapeModule.radius").floatValue = Mathf.Lerp(startRadius, endRadius, percent);
			//so.ApplyModifiedProperties();

			yield return null;
		}

		//UnityEditor.SerializedObject so2 = new UnityEditor.SerializedObject(charge);
		//UnityEditor.SerializedProperty it = so2.GetIterator();
		//while (it.Next(true))
		//	Debug.Log(it.propertyPath);

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(PulsateLaser());
	}

	IEnumerator Rumble() {
		float currentChargePercent = 0f;

		//Charging and traveling to target
		while (!hasReachedDestination) {
			VibrateManager.S.RumbleVibrate(PlayerEnum.player1, 0.25f, currentChargePercent*0.25f, true);
			VibrateManager.S.RumbleVibrate(PlayerEnum.player2, 0.25f, currentChargePercent*0.25f, true);

			CameraEffects.S.CameraShake(0.25f, 1f, true);

			currentChargePercent = Mathf.Min(1, currentChargePercent + 0.25f / chargeTime);
			yield return new WaitForSeconds(0.25f);
		}
		
		//Waiting on target
		VibrateManager.S.RumbleVibrate(PlayerEnum.player1, pulseTime, 0.5f, true);
		VibrateManager.S.RumbleVibrate(PlayerEnum.player2, pulseTime, 0.5f, true);
		CameraEffects.S.CameraShake(pulseTime, 1f, true);
		yield return new WaitForSeconds(pulseTime);

		//Exploding on target
		VibrateManager.S.RumbleVibrate(PlayerEnum.player1, explosionDuration, 1, true);
		VibrateManager.S.RumbleVibrate(PlayerEnum.player2, explosionDuration, 1, true);
		CameraEffects.S.CameraShake(explosionDuration, 10f, true);
		yield return new WaitForSeconds(explosionDuration);

		//Aftershocks
		VibrateManager.S.RumbleVibrate(PlayerEnum.player1, explosionDuration/2f, 0.5f, true);
		VibrateManager.S.RumbleVibrate(PlayerEnum.player2, explosionDuration/2f, 0.5f, true);
		CameraEffects.S.CameraShake(explosionDuration/2f, 1f, true);

		yield return new WaitForSeconds(explosionDuration/2f);
		VibrateManager.S.RumbleVibrate(PlayerEnum.player1, 0.75f*explosionDuration, 0.1f, true);
		VibrateManager.S.RumbleVibrate(PlayerEnum.player2, 0.75f*explosionDuration, 0.1f, true);
		CameraEffects.S.CameraShake(explosionDuration / 2f, 0.5f, true);
	}

	IEnumerator PulsateLaser() {
		float t = 0;
		//Pulse the laser different colors of the rainbow
		for (int i = 0; i < numPulses; i++) {
			t = 0;
			while (t < pulseTime) {
				t += Time.deltaTime;

				charge.startColor = beamPulse.Evaluate(((t / pulseTime) + startingGradientValue)%1f);
				background.startColor = beamPulse.Evaluate(((t / pulseTime) + startingGradientValue) % 1f);
				yield return null;
			}
		}
		StartCoroutine(FireLaser());
		while (!hasReachedDestination) {
			t = 0;
			while (t < pulseTime) {
				t += Time.deltaTime;

				charge.startColor = beamPulse.Evaluate(((t / pulseTime) + startingGradientValue) % 1f);
				background.startColor = beamPulse.Evaluate(((t / pulseTime) + startingGradientValue) % 1f);
				yield return null;
			}
		}
	}

	IEnumerator FireLaser() {
		Vector3 differenceVector = target.position - transform.position;
		while (!hasReachedDestination) {
			if (differenceVector.magnitude <= 0.4f) {
				hasReachedDestination = true;
			}
			differenceVector = target.position - transform.position;

			transform.Translate(differenceVector.normalized * Time.deltaTime * laserSpeed, Space.World);
			yield return null;
		}
		StartCoroutine(RidiculousDamageValues());
		yield return new WaitForSeconds(pulseTime);
		StartCoroutine(Explode());
		yield return new WaitForSeconds(explosionDuration / 2f);
		StartCoroutine(Aftermath());

		if (target != null) {
			Destroy(target.gameObject);
		}
	}

	IEnumerator RidiculousDamageValues() {
		float timeElapsed = 0;
		float totalTime = pulseTime + explosionDuration;
		PlayerEnum otherPlayer = (owningPlayer == PlayerEnum.player1) ? PlayerEnum.player2 : PlayerEnum.player1;

		float minDamageTick = 1f;
		float maxDamageTick = 20000f;

        while (timeElapsed < totalTime) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / totalTime;

			GameManager.S.DisplayDamage(otherPlayer, Mathf.Lerp(minDamageTick, maxDamageTick, Mathf.Pow(percent, 8)));

			yield return null;
		}
		
	}

	IEnumerator Explode() {
		SoundManager.instance.Play("FinalExplosion");

		explosion.Play();

		float startEmissiveRate = explosion.emissionRate;
		float timeElapsed = 0;
		while (timeElapsed < explosionDuration) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / explosionDuration;

			explosion.emissionRate = Mathf.Lerp(startEmissiveRate, 0, percent);
			charge.emissionRate = Mathf.Lerp(startEmissiveRate, 0, percent*2);
			background.emissionRate = Mathf.Lerp(startEmissiveRate, 0, percent*2);

			yield return null;
		}
	}
	IEnumerator Aftermath() {
		explosion.Play();

		float startEmissiveRate = explosion.emissionRate/20f;
		float timeElapsed = 0;
		while (timeElapsed < explosionDuration) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed / explosionDuration;

			explosion.emissionRate = Mathf.Lerp(startEmissiveRate, 0, percent);
			background.emissionRate = Mathf.Lerp(startEmissiveRate, 0, percent * 2);

			yield return null;
		}
		yield return new WaitForSeconds(2.5f);
		explosion.Stop();
		CameraEffects.S.followObj = null;
		GameManager.S.EndRound(owningPlayer);
	}
}
