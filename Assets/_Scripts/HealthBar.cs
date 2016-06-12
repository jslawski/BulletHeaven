using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	float recentlyLostHealthLerpSpeed = 0.03f;
	Transform recentlyLostHealth;
	Transform healthBar;
	Transform healthBackground;

	public void SetHealth(float percent) {
		Vector3 curScale = healthBar.localScale;
		curScale.x = percent;
		healthBar.localScale = curScale;
	}

	public void SetColor(Color playerColor) {
		healthBar.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.2f);
		healthBackground.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.7f);
	}

	// Use this for initialization
	void Awake () {
		healthBackground = transform.FindChild("HealthBarBackground");
		recentlyLostHealth = transform.FindChild("RecentlyLostHealth");
		healthBar = transform.FindChild("HealthBar");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 curScale = recentlyLostHealth.localScale;
		curScale.x = Mathf.Lerp(curScale.x, healthBar.localScale.x, recentlyLostHealthLerpSpeed);
		recentlyLostHealth.localScale = curScale;
	}
}
