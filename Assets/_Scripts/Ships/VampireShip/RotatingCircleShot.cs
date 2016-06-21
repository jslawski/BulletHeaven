using UnityEngine;
using System.Collections;

public class RotatingCircleShot : MonoBehaviour, BombAttack {
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	RotatingCircleWave rotatingCircleWavePrefab;

	float timeBetweenWaves = 0.2f;
	int numWaves = 3;

	public void FireBurst() {
		//This does nothing to appease the interface
	}

	// Use this for initialization
	void Awake () {
		rotatingCircleWavePrefab = Resources.Load<RotatingCircleWave>("Prefabs/RotatingCircleWave");
	}

	void Start() {
		StartCoroutine(FireWave());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator FireWave() {
		for (int i = 0; i < numWaves; i++) {
			RotatingCircleWave rotatingCircleWave = Instantiate(rotatingCircleWavePrefab, transform.position, new Quaternion()) as RotatingCircleWave;
			rotatingCircleWave.owningPlayer = owningPlayer;
			rotatingCircleWave.direction = (i % 2 == 0) ? 1 : -1;

			float waitTime = timeBetweenWaves + rotatingCircleWave.timeBetweenBursts*rotatingCircleWave.numBurstsPerWave;
            yield return new WaitForSeconds(waitTime);
		}
	}
}
