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

	[HideInInspector]
	public ShipMovement playerMovement;
	[HideInInspector]
	public ShootBomb playerShooting;

	float maxHealth = 100f;
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(float damageIn) {
		if (invincible) {
			return;
		}

		health -= damageIn;

		CameraEffects.S.CameraShake(0.1f, .5f);
		VibrateManager.S.RumbleVibrate(player, 0.2f, hitVibrateIntensity, true);

		if (health <= 0) {
			SoundManager.instance.Play("NearDeath");
			Die();
		}
		else if (health >= maxHealth) {
			health = maxHealth;
		}
	}

	void Die() {
		if (dead) {
			return;
		}
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
