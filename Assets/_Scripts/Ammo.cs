using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ammo : MonoBehaviour {

	Image liveAmmoImage;
	GameObject shockwavePrefab;
	public bool reloading = false;

	// Use this for initialization
	void Start () {
		liveAmmoImage = gameObject.GetComponent<Image>();
		shockwavePrefab = Resources.Load<GameObject>("Prefabs/Shockwave");
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

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
	}
}
