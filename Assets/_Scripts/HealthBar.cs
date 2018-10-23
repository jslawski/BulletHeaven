using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	[SerializeField]
	private PlayerEnum owningPlayer;
	[SerializeField]
	private Character owningCharacter;

	private float maxHealth = 0;
	private float recentlyLostHealthLerpSpeed = 1.5f;
	public Text healthText;
	public Transform recentlyLostHealth;
	public Transform healthBar;
	public Transform healthBackground;

	// Use this for initialization
	void Start () {
		StartCoroutine(SetupOwningPlayerValues());

		this.healthBackground = transform.Find("HealthBarBackground");
		this.recentlyLostHealth = transform.Find("RecentlyLostHealth");
		this.healthBar = transform.Find("HealthBar");
		this.healthText = transform.Find("HealthCounter").GetComponent<Text>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 curScale = this.recentlyLostHealth.localScale;
		curScale.x = Mathf.Lerp(curScale.x, this.healthBar.localScale.x, Time.fixedDeltaTime*this.recentlyLostHealthLerpSpeed);
		this.recentlyLostHealth.localScale = curScale;
	}

	void OnDestroy() {
		this.UnsubscribeFromEvents();
	}

	public void SetHealth(float remainingHealth) {
		float percent = remainingHealth / maxHealth;
		Vector3 curScale = healthBar.localScale;
		curScale.x = percent;
		this.healthBar.localScale = curScale;

		//JPS: Why are we calculating this value if it is stored in the PlayerShip data?
		float curHealth = percent*maxHealth;
		int curHealthDisplay = Mathf.RoundToInt(curHealth * 10f);

		//Don't round down to zero for the display unless the player is dead
		if (curHealth*10f < 1 && curHealth*10 > 0) {
			curHealthDisplay = 1;
		}
		this.healthText.text = curHealthDisplay.ToString() + "/" + (this.maxHealth * 10f);
	}

	private void SetColor(Color playerColor) {
		this.healthBar.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.2f);
		this.healthBackground.GetComponent<Image>().color = Color.Lerp(playerColor, Color.black, 0.7f);
	}

	private IEnumerator SetupOwningPlayerValues() {
		//Wait until the player ships are fully instantiated and set in the GameManager
		//JPS: I don't like that we have to check for the ShipType != none here.  I'd like to refactor the way we load the game so
		//     we don't populate the players array in the GameManager twice...
		while (GameManager.S.players[(int)this.owningPlayer] == null || GameManager.S.players[(int)this.owningPlayer].character == null) {
			yield return null;
		}

		this.owningCharacter = GameManager.S.players[(int)this.owningPlayer].character;
		//TODO 3/6/17: Fix this to work for characters with multiple ships
		this.maxHealth = owningCharacter.ship.maxHealth;
		this.SetColor(owningCharacter.player.playerColor);
		this.SetHealth(maxHealth);

		//JDS 3/6/17: Why is this necessary? Seems like it does nothing...
		this.UnsubscribeFromEvents();
		this.SubscribeToEvents();
	}

	//TODO 3/6/17: Fix these to work for characters with multiple ships
	private void SubscribeToEvents() {
		this.owningCharacter.ship.onDamaged += SetHealth;
	}

	private void UnsubscribeFromEvents() {
		this.owningCharacter.ship.onDamaged -= SetHealth;
	}
}
