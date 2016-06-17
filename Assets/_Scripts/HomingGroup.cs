﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PolarCoordinates;

public class HomingGroup : MonoBehaviour {
	public Player owningPlayer;
	public Transform target;
	NonPooledBullet bulletPrefab;
	PhysicsObj physics;

	//Group settings
	float startVelocity = 5;
	float endVelocity = 20;
	float timeToReachFullSpeed = 1f;
	float homingForce = 30f;
	float bulletDamage = 1.5f;
	float maxLifespan = 10f;

	//Alright well I call each group of bullets a shell so just deal with that
	int numShells = 4;
	int bulletPerShellIncrease = 8;
	float timeBetweenEachShellForm = 0.5f;
	float distanceBetweenShells = 1.5f;
	int[] bulletsPerShell;
	List<NonPooledBullet> childrenBullets = new List<NonPooledBullet>();

	// Use this for initialization
	void Awake () {
		bulletPrefab = Resources.Load<NonPooledBullet>("Prefabs/NonPooledBullet");
		physics = GetComponent<PhysicsObj>();
		bulletsPerShell = new int[numShells];
		for (int i = 0; i < numShells; i++) {
			bulletsPerShell[i] = Mathf.Max(1, i * bulletPerShellIncrease);
		}
	}
	
	IEnumerator Start() {
		StartCoroutine(CreateBullets());
		Destroy(gameObject, maxLifespan);

		yield return new WaitForSeconds(timeBetweenEachShellForm * numShells);

		while (childrenBullets.Count > 0) {
			yield return new WaitForSeconds(1f);
		}
		Destroy(gameObject);
	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator CreateBullets() {
		//Each shell
		for (int i = 0; i < numShells; i++) {
			float timeInShell = 0;
			float radDelta = (2f*Mathf.PI)/bulletsPerShell[i];

			float curAngle = 0;
			float bulletSpeed = (i*distanceBetweenShells)/timeBetweenEachShellForm;
			//Instantiating each bullet
			while (curAngle < 2 * Mathf.PI - 0.01f) {
				PolarCoordinate direction = new PolarCoordinate(1, curAngle);
				NonPooledBullet curBullet = Instantiate(bulletPrefab, transform.position, new Quaternion()) as NonPooledBullet;
				curBullet.parentedBullet = true;
				curBullet.damage = bulletDamage;
				curBullet.owningPlayer = owningPlayer;
				curBullet.transform.SetParent(transform);
				curBullet.transform.position = gameObject.transform.position;
				curBullet.physics.velocity = bulletSpeed * direction.PolarToCartesian().normalized;

				childrenBullets.Add(curBullet);

				curAngle += radDelta;
			}

			//Moving the bullets
			while (timeInShell < timeBetweenEachShellForm) {
				timeInShell += Time.deltaTime;
				float percent = timeInShell/timeBetweenEachShellForm;

				List<NonPooledBullet> bulletsToRemoveFromGroup = new List<NonPooledBullet>();
				foreach (var bullet in childrenBullets) {
					if (bullet.parentedBullet) {
						bullet.physics.velocity = (bullet.physics.velocity.normalized) * Mathf.Lerp(bulletSpeed, 0, percent);
					}
					else {
						bulletsToRemoveFromGroup.Add(bullet);
					}
                }
				foreach (var bullet in bulletsToRemoveFromGroup) {
					childrenBullets.Remove(bullet);
				}

				transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
				yield return null;
			}
		}

		StartCoroutine(FireTowardsTarget());
	}

	IEnumerator FireTowardsTarget() {
		//Give the group an initial velocity
		if (target != null) {
			Vector3 targetVector = (target.position - transform.position).normalized;
			physics.velocity = targetVector * startVelocity;
		}

		float timeElapsed = 0;
		while (true) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/timeToReachFullSpeed;
			physics.velocity = physics.velocity.normalized * Mathf.Lerp(startVelocity, endVelocity, percent);

			if (target != null) {
				Vector3 targetVector = (target.position - transform.position).normalized;
				physics.acceleration = targetVector * homingForce;
			}
			transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));

			yield return null;
		}
	}
}
