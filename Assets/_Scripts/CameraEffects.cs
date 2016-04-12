using UnityEngine;
using System.Collections;

public class CameraEffects : MonoBehaviour {
	public static CameraEffects S;
	public Transform followObj;
	Camera thisCamera;
	
	Vector3 startOffset;                //Starting position of the camera relative to the object it is following
	Vector3 curOffset;                  //Used for camera shake, otherwise == startOffset

	float startSize;                    //Initial orthographic size of the camera
	float curSize;                      //Used for camera zoom, otherwise == startSize
	float zoomSpeed = 0.1f;             //Percent per frame the camera's orthographic size changes to its target size

	float followSpeed = 0.1f;           //Percent per frame the camera moves towards its target position

	bool inCameraShakeCoroutine = false;
	float cameraShakeFrequency = 40f;   //How many times per second the random offset for camera shake changes

	bool inCameraZoomCoroutine = false;

	// Use this for initialization
	void Start() {
		S = this;
		startOffset = transform.position;
		curOffset = startOffset;
		thisCamera = GetComponent<Camera>();
		startSize = thisCamera.orthographicSize;
		curSize = startSize;
	}

	//DEBUG INPUT FOR CAMERA SHAKE
	void Update() {
		if (Input.GetKey("x")) {
			CameraShake(10f, 2);
		}
		if (Input.GetKey("c")) {
			CameraShake(.2f, 20, true);
		}
		if (Input.GetKey("z")) {
			CameraZoom(0.4f, 0.5f);
		}
	}

	//Camera movement in FixedUpdate() for smoother following of the physics calculations
	void FixedUpdate() {
		if (!followObj) {
			transform.position = Vector3.Lerp(transform.position, startOffset + curOffset, followSpeed);
		}
		else {
			transform.position = Vector3.Lerp(transform.position, followObj.position + startOffset + curOffset, followSpeed);
		}
		thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, curSize, zoomSpeed);

		curOffset = Vector3.Lerp(curOffset, startOffset, 0.1f);
	}

	public void CameraShake(float duration, float intensity, bool overrideCurrentShake=false) {
		if (!inCameraShakeCoroutine) {
			StartCoroutine(CameraShakeCoroutine(duration, intensity));
		}
		else if (overrideCurrentShake) {
			StopAllCoroutines();
			StartCoroutine(CameraShakeCoroutine(duration, intensity));
		}
	}
	IEnumerator CameraShakeCoroutine(float duration, float intensity) {
		inCameraShakeCoroutine = true;

		float timeElapsed = 0;
		while (timeElapsed < duration) {
			Vector2 tempVector2 = Random.insideUnitCircle;
			curOffset = startOffset + intensity * (new Vector3(tempVector2.x, tempVector2.y, 0));

			timeElapsed += 1 / cameraShakeFrequency;
			yield return new WaitForSeconds(1 / cameraShakeFrequency);
		}

		curOffset = startOffset;
		inCameraShakeCoroutine = false;
	}

	public void CameraZoom(float duration, float percent) {
		if (!inCameraZoomCoroutine) {
			StartCoroutine(CameraZoomCoroutine(duration, percent));
		}
	}
	IEnumerator CameraZoomCoroutine(float duration, float percent) {
		inCameraZoomCoroutine = true;

		//Move the camera offset towards the ground
		curSize = startSize * percent;
		yield return new WaitForSeconds(duration);
		curSize = startSize;

		inCameraZoomCoroutine = false;
	}
}
