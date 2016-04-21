using UnityEngine;
using System.Collections;
using InControl;

public class PlayerShip : MonoBehaviour, DamageableObject {
	public FinishAttack finalAttackPrefab;
	public Player player;
	public Color playerColor;
	public HealthBar healthBar;
	public InputDevice device;
	public PressStartPrompt controllerPrompt;
	float hitVibrateIntensity = 1f;
	public GameObject finishAttackPrompt;

	SpriteRenderer shipSprite;
	bool inDamageFlashCoroutine = false;
	float damageFlashDuration = 0.2f;
	float timeSinceTakenDamage = 0f;

	[HideInInspector]
	public ShipMovement playerMovement;
	[HideInInspector]
	public ShootBomb playerShooting;

	float maxHealth = 150f;
	float _health;
	float health {
		get {
			return _health;
		}
		set {
			_health = value;
			if (_health <= 0) {
				_health = 0;
			}

			if (healthBar != null) {
				healthBar.SetHealth(_health / maxHealth);
			}
		}
	}
	bool dead = false;
	public bool invincible = false;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		playerMovement = GetComponent<ShipMovement>();
		playerShooting = GetComponent<ShootBomb>();
		shipSprite = GetComponentInChildren<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		timeSinceTakenDamage += Time.deltaTime;
	}

	public void TakeDamage(float damageIn) {
		if (invincible || dead) {
			return;
		}

		if (damageIn > 0) {
			damageIn *= GameManager.S.curDamageAmplification;
			CameraEffects.S.CameraShake(0.1f, .5f);
			VibrateManager.S.RumbleVibrate(player, 0.2f, hitVibrateIntensity, true);
		}

		health -= damageIn;

		if (health <= 0) {
			Die();
		}
		else if (health >= maxHealth) {
			health = maxHealth;
		}
		timeSinceTakenDamage = 0;
		if (!inDamageFlashCoroutine) {
			StartCoroutine(FlashOnDamage(damageIn));
		}
	}

	IEnumerator FlashOnDamage(float damage) {
		inDamageFlashCoroutine = true;

		Color targetColor = (damage > 0) ? Color.red : Color.green;
		shipSprite.color = targetColor;

		//Taking damage
		if (damage > 0) {
			while (timeSinceTakenDamage < damageFlashDuration) {
				float percent = timeSinceTakenDamage / damageFlashDuration;
				shipSprite.color = Color.Lerp(targetColor, Color.white, percent);
	
				yield return null;
			}
		}
		//Healing damage
		else {
			float timeElapsed = 0;
			while (timeElapsed < 4*damageFlashDuration) {
				timeElapsed += Time.deltaTime;
				shipSprite.color = Color.Lerp(targetColor, Color.white, timeElapsed/(4*damageFlashDuration));

				yield return null;
			}
		}
		shipSprite.color = Color.white;

		inDamageFlashCoroutine = false;
	}

	void Die() {
		if (dead) {
			return;
		}
		SoundManager.instance.Play("NearDeath");
		dead = true;
		playerShooting.shootingDisabled = true;
		playerMovement.movementDisabled = true;
		print("I am dead");

		int otherPlayer = (player == Player.player1) ? (int)Player.player2 : (int)Player.player1;
		GameManager.S.players[otherPlayer].InitializeFinalAttack();
	}

	public void InitializeFinalAttack() {
		finishAttackPrompt.SetActive(true);

		Vector3 spawnPos = transform.position + transform.up * 4.5f;
        FinishAttack finalAttack = Instantiate(finalAttackPrefab, spawnPos, new Quaternion()) as FinishAttack;
		finalAttack.owningPlayer = player;
		finalAttack.fireKey = (player == Player.player1) ? KeyCode.E : KeyCode.KeypadEnter;

		//Disable shooting so you don't fire a bomb when you perform the final attack
		playerShooting.shootingDisabled = true;
		invincible = true;
	}
}
