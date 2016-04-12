﻿using UnityEngine;
using System.Collections;

public class FinishAttack : MonoBehaviour {
	public Transform target;
	public Gradient beamPulse;
	ParticleSystem charge;
	ParticleSystem background;
	ParticleSystem explosion;

	float chargeTime = 2f;

	float startSpeed = -10f;
	float endSpeed = 0.1f;
	float startRadius = 10f;
	float endRadius = 2f;
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
	float fireDelay = 1.5f;

	float explosionDuration = 2f;

	// Use this for initialization
	void Start () {
		charge = transform.FindChild("Charge").GetComponent<ParticleSystem>();
		background = transform.FindChild("BackgroundEffect").GetComponent<ParticleSystem>();
		explosion = transform.FindChild("MassiveExplosion").GetComponent<ParticleSystem>();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("return")) {
			StartCoroutine(FinalAttack());
		}
	}

	IEnumerator FinalAttack() {
		StartCoroutine(CameraRumble());
		charge.Play();
		background.Play();
		float timeElapsed = 0;

		Color startColor = Color.black;
		Color endColor = charge.startColor;

		ParticleSystemRenderer backgroundRenderer = background.GetComponent<ParticleSystemRenderer>();

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
			UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(charge);
			so.FindProperty("ShapeModule.radius").floatValue = Mathf.Lerp(startRadius, endRadius, percent);
			so.ApplyModifiedProperties();

			yield return null;
		}

		//UnityEditor.SerializedObject so2 = new UnityEditor.SerializedObject(charge);
		//UnityEditor.SerializedProperty it = so2.GetIterator();
		//while (it.Next(true))
		//	Debug.Log(it.propertyPath);

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(PulsateLaser());
	}

	IEnumerator CameraRumble() {
		while (!hasReachedDestination) {
			CameraEffects.S.CameraShake(0.25f, 1f, true);
			yield return new WaitForSeconds(0.25f);
		}
		yield return new WaitForSeconds(pulseTime);
		CameraEffects.S.CameraShake(explosionDuration, 10f, true);
		yield return new WaitForSeconds(explosionDuration);
		CameraEffects.S.CameraShake(explosionDuration/2f, 1f, true);
		yield return new WaitForSeconds(explosionDuration/2f);
		CameraEffects.S.CameraShake(explosionDuration / 2f, 0.5f, true);
	}

	IEnumerator PulsateLaser() {
		float t = 0;
		//Pulse the laser different colors of the rainbow
		for (int i = 0; i < numPulses; i++) {
			t = 0;
			while (t < pulseTime) {
				t += Time.deltaTime;

				charge.startColor = beamPulse.Evaluate(t / pulseTime);
				background.startColor = beamPulse.Evaluate(t / pulseTime);
				yield return null;
			}
		}
		StartCoroutine(FireLaser());
		while (!hasReachedDestination) {
			t = 0;
			while (t < pulseTime) {
				t += Time.deltaTime;

				charge.startColor = beamPulse.Evaluate(t / pulseTime);
				background.startColor = beamPulse.Evaluate(t / pulseTime);
				yield return null;
			}
		}
	}

	IEnumerator FireLaser() {
		Vector3 differenceVector = target.position - transform.position;
		while (!hasReachedDestination) {
			if (differenceVector.magnitude <= 0.1f) {
				hasReachedDestination = true;
			}
			differenceVector = target.position - transform.position;

			transform.Translate(differenceVector.normalized * Time.deltaTime * laserSpeed);
			yield return null;
		}
		yield return new WaitForSeconds(pulseTime);
		StartCoroutine(Explode());
		yield return new WaitForSeconds(explosionDuration / 2f);
		StartCoroutine(Aftermath());

		if (target != null) {
			Destroy(target.gameObject);
		}
	}

	IEnumerator Explode() {
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
		yield return new WaitForSeconds(0.5f);
		explosion.Stop();
	}
}
