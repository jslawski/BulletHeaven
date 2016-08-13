using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	public float maxHealth = 0;
	float recentlyLostHealthLerpSpeed = 1.5f;
	Text healthText;
	Transform recentlyLostHealth;
	Transform healthBar;
	Transform healthBackground;

	public void SetHealth(float percent) {
		Vector3 curScale = healthBar.localScale;
		curScale.x = percent;
		healthBar.localScale = curScale;

		float curHealth = percent*maxHealth;
		int curHealthDisplay = Mathf.RoundToInt(curHealth * 10f);

		//Don't round down to zero for the display unless the player is dead
		if (curHealth*10f < 1 && curHealth*10 > 0) {
			curHealthDisplay = 1;
		}
		healthText.text = curHealthDisplay.ToString() + "/" + (maxHealth * 10f);
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
		healthText = transform.FindChild("HealthCounter").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 curScale = recentlyLostHealth.localScale;
		curScale.x = Mathf.Lerp(curScale.x, healthBar.localScale.x, Time.fixedDeltaTime*recentlyLostHealthLerpSpeed);
		recentlyLostHealth.localScale = curScale;
	}
}
