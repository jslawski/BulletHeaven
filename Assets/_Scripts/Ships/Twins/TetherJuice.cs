using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherJuice : MonoBehaviour {
	List<Ship> ships;

	ParticleSystem ps;
	ParticleSystem.MainModule mainModule;
	ParticleSystem.ColorOverLifetimeModule colorModule;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
		mainModule = ps.main;
		colorModule = ps.colorOverLifetime;

		ships = transform.parent.GetComponentInParent<Twins>().ships;
	}

	public void BrightenCollisionSpot(Vector3 collisionPos) {
		float pSpeed = mainModule.startSpeed.constant; //m/s
		float pLifetime = mainModule.startLifetime.constant; //s

		float tetherCollisionPos = (ships[0].transform.position - collisionPos).magnitude; //m
		float timeUntilParticleReachesCollisionPos = tetherCollisionPos / pSpeed; //s

		float gradientPos = timeUntilParticleReachesCollisionPos / pLifetime;

		StartCoroutine(BrightenGradient(gradientPos));
	}

	IEnumerator BrightenGradient(float gradientPos) {
		GradientAlphaKey[] startAlphaKeys = colorModule.color.gradient.alphaKeys;
		if (startAlphaKeys.Length == 8) yield break;
		List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>(startAlphaKeys);
		print(gradientPos);
		alphaKeys.Add(new GradientAlphaKey(1, gradientPos));
		colorModule.color.gradient.alphaKeys = alphaKeys.ToArray();
		yield return new WaitForSeconds(1f);
		colorModule.color.gradient.alphaKeys = startAlphaKeys;
	}
}
