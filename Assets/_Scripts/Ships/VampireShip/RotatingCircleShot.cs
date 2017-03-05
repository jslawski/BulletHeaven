using UnityEngine;
using System.Collections;

public class RotatingCircleShot : MonoBehaviour, BombAttack {
	PlayerEnum _owningPlayer = PlayerEnum.none;

	public Player thisPlayer;
	public PlayerEnum owningPlayer {
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
		StartCoroutine(FireWaves());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator FireWaves() {
		for (int i = 0; i < numWaves; i++) {
			RotatingCircleWave rotatingCircleWave = Instantiate(rotatingCircleWavePrefab, transform.position, new Quaternion()) as RotatingCircleWave;
			rotatingCircleWave.owningPlayer = owningPlayer;
			if (!GameManager.S.inGame) {
				rotatingCircleWave.thisPlayer = thisPlayer;
			}
			rotatingCircleWave.direction = (i % 2 == 0) ? 1 : -1;

			float waitTime = timeBetweenWaves + rotatingCircleWave.timeBetweenBursts*rotatingCircleWave.numBurstsPerWave;
            yield return new WaitForSeconds(waitTime);
		}
	}
}
