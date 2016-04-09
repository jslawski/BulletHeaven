using UnityEngine;
using System.Collections;

public class ProtagonistShipSpawner : MonoBehaviour {
	public GameObject[] protagonistShipPrefabs;

	float posOffsetMax = 10f;			//How far off-center the ship can spawn
	float angleOffsetMax = 3f;          //How far off-center of the screen the ship can aim towards
	float minSpeed = 10f;				//Lower limit on new ship speed
	float maxSpeed = 20f;               //Upper limit on new ship speed
	float sizeVariance = 0.75f;			//Random variance in new ship sizes

	// Use this for initialization
	IEnumerator Start () {
		while (true) {
			SpawnNewShip();

			yield return new WaitForSeconds(1);
		}
	}

	void SpawnNewShip() {
		//Randomly select which protagonist ship to spawn
		int randIndex = Random.Range(0, protagonistShipPrefabs.Length);

		//Give a random offset to the ship's position
		float randPosOffset = Random.Range(-posOffsetMax, posOffsetMax);
		Vector3 spawnPos = transform.position + transform.right * randPosOffset;

		GameObject newShip = Instantiate(protagonistShipPrefabs[randIndex], spawnPos, new Quaternion()) as GameObject;
		PhysicsObj newShipPhysics = newShip.GetComponent<PhysicsObj>();

		//Apply a random size to the ship
		Vector3 curScale = newShip.transform.localScale;
		float scaleFactor = Random.Range(1 - sizeVariance, 1 + sizeVariance);
		curScale.x *= scaleFactor;
        curScale.y *= scaleFactor;
		newShip.transform.localScale = curScale;
		newShip.GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)((1 - scaleFactor) * -100);

		//Aim the ship towards the center of the map (+/- angleOffset)
		float randAngleOffset = Random.Range(-angleOffsetMax, angleOffsetMax);
		newShip.transform.up = -(spawnPos + Vector3.up * randAngleOffset).normalized;

		//Move the ship towards the center of the map at a random velocity in range
		float randSpeed = Random.Range(minSpeed, maxSpeed);
		newShipPhysics.velocity = newShip.transform.up * randSpeed * scaleFactor;
	}
}
