using UnityEngine;
using System.Collections;

public class ProtagonistShipSpawner : MonoBehaviour {
	public GameObject[] protagonistShipPrefabs;

	float posOffsetMax = 15f;			//How far off-center the ship can spawn
	float angleOffsetMax = 3f;          //How far off-center of the screen the ship can aim towards
	float minSpeed = 20f;				//Lower limit on new ship speed
	float maxSpeed = 25f;               //Upper limit on new ship speed
	float sizeVariance = 0.85f;         //Random variance in new ship sizes

	float interactableShipThreshold = 0.2f;		//How close to sizeScale == 1 a ship has to be to be considered "in" the battlegrounds

	// Use this for initialization
	IEnumerator Start () {
		while (true) {
			SpawnNewShip();

			float sleepTime = Random.Range(0.25f, 2f);
			yield return new WaitForSeconds(sleepTime);
		}
	}

	void SpawnNewShip() {
		//Randomly select which protagonist ship to spawn
		int randIndex = Random.Range(0, protagonistShipPrefabs.Length);

		//Determine the random size (and parallax) of the ship
		float scaleFactor = Random.Range(1 - sizeVariance, 1 + sizeVariance);
		bool backgroundShip = Mathf.Abs(1 - scaleFactor) > interactableShipThreshold;

		//Give a random offset to the ship's position
		//If this ship will be a background/foreground ship, allow it to spawn in a wider range
		float randPosOffset = 0;
		if (!backgroundShip) {
			randPosOffset = Random.Range(-posOffsetMax, posOffsetMax);
		}
		else {
			randPosOffset = Random.Range(-4 * posOffsetMax, 4 * posOffsetMax);
		}
		Vector3 spawnPos = transform.position + transform.right * randPosOffset;

		GameObject newShip = Instantiate(protagonistShipPrefabs[randIndex], spawnPos, new Quaternion()) as GameObject;
		PhysicsObj newShipPhysics = newShip.GetComponent<PhysicsObj>();

		//Apply a random size (and parallax) to the ship
		Vector3 curScale = newShip.transform.localScale;
		curScale.x *= scaleFactor;
        curScale.y *= scaleFactor;
		newShip.transform.localScale = curScale;

		//Apply a sorting order based on the depth of this ship
		SpriteRenderer newShipSprite = newShip.GetComponentInChildren<SpriteRenderer>();
		newShipSprite.sortingOrder = (int)((1 - scaleFactor) * -100);
		//Dim the colors of background ships
		if (backgroundShip) {
			newShipSprite.color *= 0.75f;
		}

		//Aim the ship towards the center of the map (+/- angleOffset) if it's an interactable ship
		//or more lazily towards the center of the map if it's a background or foreground ship
		float randAngleOffset = 0;
		if (!backgroundShip) {
			randAngleOffset = Random.Range(-angleOffsetMax, angleOffsetMax);
		}
		else {
			randAngleOffset = Random.Range(-4 * angleOffsetMax, 4 * angleOffsetMax);
		}
		newShip.transform.up = -(spawnPos + Vector3.up * randAngleOffset).normalized;

		//Move the ship towards the center of the map at a random velocity in range
		float randSpeed = Random.Range(minSpeed, maxSpeed);
		newShipPhysics.velocity = newShip.transform.up * randSpeed * scaleFactor;
	}
}
