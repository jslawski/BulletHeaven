using UnityEngine;
using System.Collections;

public class ProtagonistShipSpawner : MonoBehaviour {
	public GameObject[] protagonistShipPrefabs;

	float posOffsetMax = 15f;			//How far off-center the ship can spawn
	float angleOffsetMax = 3f;          //How far off-center of the screen the ship can aim towards
	float minSpeed = 20f;				//Lower limit on new ship speed
	float maxSpeed = 25f;               //Upper limit on new ship speed

	float backgroundShipChance = 0.9f;			//Percent chance for a ship to spawn in the background
	float minBackgroundShipSize = 0.15f;		//Lower limit on size of background ships
	float maxBackgroundShipSize = 0.75f;         //Upper limit on size of background ships

	float battlegroundsShipChance = 0.065f;      //Percent chance for a ship to spawn in the battlegrounds
	float minBattlegroundShipSize = 0.95f;		//Lower limit on size of battleground ships
	float maxBattlegroundShipSize = 1.05f;      //Upper limit on size of battleground ships

	float foregroundShipChance = 0.05f;         //Percent chance for a ship to spawn in the foreground
	float minForegroundShipSize = 2.5f;			//Lower limit on size of foreground ships
	float maxForegroundShipSize = 5f;			//Upper limit on size of foreground ships

	float interactableShipThreshold = 0.05f;	//How close to sizeScale == 1 a ship has to be to be considered "in" the battlegrounds
	float backgroundShipColorDim = 0.75f;       //How much background and foreground ships' colors are dimmed


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
		float scaleFactor = GetScaleFactor();
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
			newShipSprite.color *= backgroundShipColorDim;
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

		if (!backgroundShip) {
			print("Look out, a ship has entered the battlegrounds!");
		}
	}

	float GetScaleFactor() {
		float[] weights = new float[]{foregroundShipChance, battlegroundsShipChance, backgroundShipChance};
		float sumWeights = 0;
		foreach (var weight in weights) {
			sumWeights += weight;
		}
		float randVal = Random.Range(0, sumWeights);

		//Determine where the ship will spawn
		int i;
		for (i = 0; i < weights.Length; i++) {
			if (randVal < weights[i]) {
				break;
			}
			randVal -= weights[i];
		}

		switch (i) {
			//Spawn ship in foreground
			case 0:
				return Random.Range(minForegroundShipSize, maxForegroundShipSize);
			//Spawn ship in battlegrounds
			case 1:
				return Random.Range(minBattlegroundShipSize, maxBattlegroundShipSize);
			//Spawn ship in background
			case 2:
				return Random.Range(minBackgroundShipSize, maxBackgroundShipSize);
			default:
				throw new System.Exception("randVal: " + randVal + " > sumWeights: " + sumWeights);
		}
	}
}
