using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour {
	public static bool paused = false;

	float defaultFixedDeltaTime = 0;
	float realTimeToUnpause = 0.75f;
	bool inUnpauseCoroutine = false;

	void Awake() {
		defaultFixedDeltaTime = Time.fixedDeltaTime;
	}

	// Use this for initialization
	void Start () {
		//PauseGame();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")) {
			if (paused && !inUnpauseCoroutine) {
				UnpauseGame();
			}
			else if (!paused) {
				PauseGame();
			}
		}
	}

	public void PauseGame() {
		if (inUnpauseCoroutine) {
			StopAllCoroutines();
			inUnpauseCoroutine = false;
		}

		paused = true;

		Time.timeScale = 0f;
		Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;
	}

	public void UnpauseGame() {
		StartCoroutine(UnpauseGameCoroutine());
	}

	IEnumerator UnpauseGameCoroutine() {
		inUnpauseCoroutine = true;
		paused = false;

		float startTime = Time.realtimeSinceStartup;
		float curTime = startTime;
		float endTime = startTime + realTimeToUnpause;
		while (curTime < endTime) {
			curTime = Time.realtimeSinceStartup;
			float percent = (curTime-startTime)/(realTimeToUnpause);

			Time.timeScale = Mathf.Lerp(0f, 1, percent);
			Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

			yield return null;
		}

		inUnpauseCoroutine = false;
	}
}
