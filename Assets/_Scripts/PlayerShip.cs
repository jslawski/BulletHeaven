using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour, DamageableObject {
	public Player player;
	public Color playerColor;
	public HealthBar healthBar;
	public ShipMovement playerMovement;

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

			healthBar.SetHealth(_health / maxHealth);
		}
	}

	// Use this for initialization
	void Start () {
		health = maxHealth;
		playerMovement = GetComponent<ShipMovement>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TakeDamage(float damageIn) {
		health -= damageIn;

		CameraEffects.S.CameraShake(0.1f, .5f);

		if (health <= 0) {
			Die();
		}
	}

	void Die() {
		print("I am dead");
	}
}
