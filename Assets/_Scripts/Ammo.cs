using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ammo : MonoBehaviour {
	Vector3 worldSpaceOfImage;
	Image liveAmmoImage;
	GameObject shockwavePrefab;
	public bool reloading = false;

	// Use this for initialization
	void Start () {
		liveAmmoImage = gameObject.GetComponent<Image>();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");

		//Determine where in the world this is
		RectTransform rect = GetComponent<RectTransform>();
		Vector3 viewportSpaceOfImage = rect.TransformPoint((rect.anchorMin + rect.anchorMax) / 2f);
		viewportSpaceOfImage.z = 0;
		viewportSpaceOfImage.x /= Camera.main.pixelWidth;
		viewportSpaceOfImage.y /= Camera.main.pixelHeight;
		worldSpaceOfImage = Camera.main.ViewportToWorldPoint(viewportSpaceOfImage);
	}
	
	public IEnumerator DisplayReloadCoroutine(float duration) {
		liveAmmoImage.fillAmount = 0;
		reloading = true;

		//Slowly refill the ammo image back
		for (float i = 0; i <= duration; i += Time.fixedDeltaTime) {
			liveAmmoImage.fillAmount = i / duration;
			yield return new WaitForFixedUpdate();
		}

		reloading = false;
		liveAmmoImage.fillAmount = 1; //Just in case...


		GameObject shockwave = Instantiate(shockwavePrefab, worldSpaceOfImage, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
	}
}
