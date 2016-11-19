using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	[SerializeField]
	private Player owningPlayer;
	[SerializeField]
	private PlayerShip owningPlayerShip;

	private float maxHealth = 0;
	private float recentlyLostHealthLerpSpeed = 1.5f;
	public Text healthText;
	public Transform recentlyLostHealth;
	public Transform healthBar;
	public Transform healthBackground;

	public void SetHealth(float remainingHealth) {
		float percent = remainingHealth / maxHealth;
		Vector3 curScale = healthBar.localScale;
		curScale.x = percent;
		healthBar.localScale = curScale;

		//JPS: Why are we calculating this value if it is stored in the PlayerShip data?
		float curHealth = percent*maxHealth;
		int curHealthDisplay = Mathf.RoundToInt(curHealth * 10f);

		//Don't round down to zero for the display unless the player is dead
		if (curHealth*10f < 1 && curHealth*10 > 0) {
			curHealthDisplay = 1;
		}
		healthText.text = curHealthDisplay.ToString() + "/" + (maxHealth * 10f);
	}

	private void SetColor(Color playerColor) {
		healthBar.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.2f);
		healthBackground.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.7f);
	}

	private IEnumerator SetupOwningPlayerValues() {
		//Wait until the player ships are fully instantiated and set in the GameManager
		//JPS: I don't like that we have to check for the ShipType != none here.  I'd like to refactor the way we load the game so
		//     we don't populate the players array in the GameManager twice...
		while (GameManager.S.players[(int)owningPlayer] == null || GameManager.S.players[(int)owningPlayer].typeOfShip == ShipType.none) {
			yield return null;
		}

		owningPlayerShip = GameManager.S.players[(int)owningPlayer];
		maxHealth = owningPlayerShip.maxHealth;
		SetColor(owningPlayerShip.playerColor);
		SetHealth(maxHealth);
		SubscribeToEvents();
	}

	private void SubscribeToEvents() {
		owningPlayerShip.onDamaged += SetHealth;
	}

	private void UnsubscribeFromEvents() {
		owningPlayerShip.onDamaged -= SetHealth;
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(SetupOwningPlayerValues());

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

	void OnDestroy() {
		UnsubscribeFromEvents();
	}
}
