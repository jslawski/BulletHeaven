using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	float recentlyLostHealthLerpSpeed = 0.04f;
	Transform recentlyLostHealth;
	Transform healthBar;


	public void SetHealth(float percent) {
		Vector3 curScale = healthBar.localScale;
		curScale.x = percent;
		healthBar.localScale = curScale;
	}

	// Use this for initialization
	void Start () {
		recentlyLostHealth = transform.FindChild("RecentlyLostHealth");
		healthBar = transform.FindChild("HealthBar");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curScale = recentlyLostHealth.localScale;
		curScale.x = Mathf.Lerp(curScale.x, healthBar.localScale.x, recentlyLostHealthLerpSpeed);
		recentlyLostHealth.localScale = curScale;
	}
}
