using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;

public enum ShipType {
	none,
	generalist,
	tank,
	masochist,
	glassCannon,
	vampire,
	random
}

public class PlayerShip : MonoBehaviour, DamageableObject {
	public ShipType typeOfShip;

	public FinishAttack finalAttackPrefab;
	public Player player;
	public Color playerColor;
	public DurationBar durationBar;
	public HealthBar healthBar;
	protected ParticleSystem healthPickupParticles;
	public InputDevice device;
	public PressStartPrompt controllerPrompt;
	protected float hitVibrateIntensity = 1f;
	public GameObject finishAttackPrompt;

	bool inHealthPickupCoroutine = false;
	float healthPickupParticleTime = 0.75f;

	protected bool inHeartbeatCoroutine = false;
	protected float lowOnHealthThreshold = 0.3f;
	protected float heartbeatPulseDuration = 0.1f;
	protected float timeBetweenHeartbeats = 1f;
	protected float heartbeatVibration = 0.5f;
	Color damageColor = new Color(1f, 56f/255f, 56f/255f);

	//Death particles
	public GameObject deathExplosionPrefab;
	public GameObject explosionPrefab;
	protected ParticleSystem smokeParticles;
	protected float pulsateRedPeriod = 2f;
	protected float maxExplosionRadius = 1f;
	protected float minTimeBetweenExplosions = 0.1f;
	protected float maxTimeBetweenExplosions = 0.5f;

	protected SpriteRenderer shipSprite;
	protected bool inDamageFlashCoroutine = false;
	protected float damageFlashDuration = 0.2f;
	protected float timeSinceTakenDamage = 0f;

	[HideInInspector]
	public ShipMovement playerMovement;
	[HideInInspector]
	public ShootBomb playerShooting;

	public PlayerShip otherPlayer;

	protected float maxHealth = 150f;
	protected float _health;
	protected float health {
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
	public bool dead = false;
	public bool invincible = false;

	// Use this for initialization
	virtual protected void Awake () {
		playerMovement = GetComponent<ShipMovement>();
		playerShooting = GetComponent<ShootBomb>();
		shipSprite = GetComponentInChildren<SpriteRenderer>();
		smokeParticles = transform.FindChild("SmokeParticleSystem").GetComponent<ParticleSystem>();
		healthPickupParticles = transform.FindChild("HealthPickupParticleSystem").GetComponent<ParticleSystem>();
	}

	protected void Start() {
		if (healthBar != null) {
			healthBar.maxHealth = maxHealth;
		}
		health = maxHealth;
		if (durationBar != null) {
			durationBar.SetPercent(0);
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		timeSinceTakenDamage += Time.deltaTime;
	}

	public virtual void TakeDamage(float damageIn) {
		if (invincible || dead) {
			return;
		}

		if (damageIn > 0) {
			damageIn *= GameManager.S.curDamageAmplification;
			CameraEffects.S.CameraShake(0.1f, .5f);
			VibrateManager.S.RumbleVibrate(player, 0.2f, hitVibrateIntensity, true);
			SoundManager.instance.Play("TakeDamage");
			GameManager.S.DisplayDamage(player, damageIn);

			if (health < lowOnHealthThreshold*maxHealth && !inHeartbeatCoroutine) {
				StartCoroutine(HeartbeatOnLowHealth());
			}
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

	IEnumerator HeartbeatOnLowHealth() {
		inHeartbeatCoroutine = true;

		yield return new WaitForSeconds(timeBetweenHeartbeats);

		while (GameManager.S.gameState == GameStates.playing && health < lowOnHealthThreshold*maxHealth) {
			VibrateManager.S.RumbleVibrate(player, heartbeatPulseDuration, heartbeatVibration, true);
			yield return new WaitForSeconds(timeBetweenHeartbeats);
		}

		inHeartbeatCoroutine = false;
	}

	protected IEnumerator FlashOnDamage(float damage) {
		inDamageFlashCoroutine = true;

		Color targetColor = (damage > 0) ? damageColor : Color.green;
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
			PlayHealthPickupParticles();
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

		VibrateManager.S.RumbleVibrate(player, 0.75f, 0.6f, true);

		SoundManager.instance.Play("NearDeath", 1);
		dead = true;
		playerShooting.shootingDisabled = true;
		playerMovement.movementDisabled = true;
		//print("I am dead");

		int otherPlayer = (player == Player.player1) ? (int)Player.player2 : (int)Player.player1;
		GameManager.S.players[otherPlayer].InitializeFinalAttack();
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
		if (durationBar != null) {
			durationBar.SetPercent(0);
		}

		StartCoroutine(DeathParticles());
		StartCoroutine(PulsateRed());
	}

	public void InitializeFinalAttack() {
		finishAttackPrompt.SetActive(true);
		finishAttackPrompt.GetComponentInChildren<Text>().color = playerColor;
		finishAttackPrompt.transform.FindChild("Plus").GetComponent<Image>().color = playerColor;
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
		if (durationBar != null) {
			durationBar.SetPercent(0);
		}

		Vector3 spawnPos = transform.position + transform.up * 4.5f;
        FinishAttack finalAttack = Instantiate(finalAttackPrefab, spawnPos, new Quaternion()) as FinishAttack;
		finalAttack.owningPlayer = player;
		finalAttack.fireKey = (player == Player.player1) ? KeyCode.E : KeyCode.KeypadEnter;

		//Disable shooting so you don't fire a bomb when you perform the final attack
		playerShooting.shootingDisabled = true;
		invincible = true;
	}

	IEnumerator DeathParticles() {
		GameObject playerExplosion = Instantiate(deathExplosionPrefab, transform.position, new Quaternion()) as GameObject;
		SoundManager.instance.Play("DestroyProtagShip");

		CameraEffects.S.CameraShake(1f, 1.5f, true);
		Destroy(playerExplosion, 5f);

		yield return new WaitForSeconds(Random.Range(minTimeBetweenExplosions, maxTimeBetweenExplosions));

		smokeParticles.Play();
		while (GameManager.S.gameState != GameStates.finalAttack) {
			Vector3 offset = maxExplosionRadius * Random.insideUnitCircle;

			GameObject explosion = Instantiate(explosionPrefab, transform.position + offset, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);

			yield return new WaitForSeconds(Random.Range(minTimeBetweenExplosions, maxTimeBetweenExplosions));
		}
	}
	IEnumerator PulsateRed() {
		float t = 0;
		while (GameManager.S.gameState != GameStates.finalAttack) {
			for (t = 0; t < pulsateRedPeriod; t += Time.deltaTime) {
				t += Time.deltaTime;

				shipSprite.color = Color.Lerp(Color.white, damageColor, 0.5f * (Mathf.Sin(2 * Mathf.PI * t / pulsateRedPeriod) + 1));

				yield return 0;
			}
		}
		shipSprite.color = Color.white;
	}

	void PlayHealthPickupParticles() {
		if (!inHealthPickupCoroutine) {
			StartCoroutine(HealthPickupCoroutine());
		}
	}

	IEnumerator HealthPickupCoroutine() {
		inHealthPickupCoroutine = true;

		healthPickupParticles.Play();
		yield return new WaitForSeconds(healthPickupParticleTime);
		healthPickupParticles.Stop();

		inHealthPickupCoroutine = false;
	}
}
