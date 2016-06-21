﻿using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class SineShot : MonoBehaviour, BombAttack{
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	public Masochist masochistPlayer;

	public Transform target;
	int bulletsPerBurst = 100;
	float amplitudeScalar = 20f;
	public SineBullet bulletPrefab;
	float baseVelocity;

	public void FireBurst() {
		baseVelocity = masochistPlayer.damageMultiplier == 1 ? 10f : 15f;
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == Player.none) {
			Debug.LogError("Sine shot does not have owning player set");
			yield break;
		}

		Player targetPlayer = (owningPlayer == Player.player1) ? Player.player2 : Player.player1;
		target = GameManager.S.players[(int)targetPlayer].transform;

		PolarCoordinate direction = new PolarCoordinate(1, target.position - gameObject.transform.position);

		float t = 0;
		for (int i = 0; i < bulletsPerBurst; i++) {
			t += Time.deltaTime;

			//Generate 2 waves
			GenerateBullet(direction, -1);
			GenerateBullet(direction, 1);
			
			yield return new WaitForFixedUpdate();
		}

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}

	void GenerateBullet(PolarCoordinate direction, int waveDirection) {
		SineBullet curBullet = bulletPrefab.GetPooledInstance<SineBullet>();
		curBullet.owningPlayer = owningPlayer;
		curBullet.transform.position = gameObject.transform.position;
		curBullet.GetComponent<PhysicsObj>().velocity = baseVelocity * direction.PolarToCartesian().normalized;

		//Make amplitude wider during aura mode
		if (masochistPlayer != null && masochistPlayer.damageMultiplier > 1) {
			curBullet.amplitude = amplitudeScalar;
		}
		else {
			curBullet.amplitude = 10f;
		}

		curBullet.ApplySineWave(waveDirection);
	}

}
