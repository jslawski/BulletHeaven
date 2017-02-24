using UnityEngine;
using System.Collections;

public enum BulletState { none, parented, absorbedByMasochist, absorbedByVampire, affectedByBlackHole, absorbedByBlackHole, reflected}

public class Bullet : PooledObj {
	public GameObject explosionPrefab;
	public float damage;

	protected float transparencyCheckCooldown = 0.4f;
	Color unownedBulletColor = new Color(233f/255f, 154f/255f, 215f/255f);

	public SpriteRenderer sprite;
	ParticleSystem trail;
	public PhysicsObj physics;
	public SphereCollider hitbox;
	protected Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				if (GameManager.S.inGame) {
					thisPlayer = GameManager.S.players[(int)value];
					SetColor(thisPlayer.playerColor);
					otherPlayer = GameManager.S.OtherPlayerShip(thisPlayer).playerMovement;
				}
			}
			else {
				SetColor(unownedBulletColor);
			}
		}
	}
	public PlayerShip thisPlayer;
	public ShipMovement otherPlayer;
	protected bool _transparent = false;
	protected bool transparent {
		get {
			return _transparent;
		}
		set {
			SetTransparency(value);
		}
	}

	BulletState _curState = BulletState.none;

	public BulletState curState {
		get {
			return _curState;
		}
		set {
			//If this was part of a parented group and we're telling it not to be, unparent it
			if (_curState == BulletState.parented && value != BulletState.parented) {
				transform.parent = null;
			}
			_curState = value;
		}
	}

	protected float transparencyAlpha = 71f / 255f;
	protected float vampShieldHealAmount = 0.085f;

	public bool IsInteractable() {
		return (curState != BulletState.absorbedByBlackHole && curState != BulletState.absorbedByMasochist && curState != BulletState.absorbedByVampire);
	}

	protected void Awake() {
		trail = GetComponentInChildren<ParticleSystem>();
		physics = GetComponent<PhysicsObj>();
		sprite = GetComponent<SpriteRenderer>();
		hitbox = GetComponent<SphereCollider>();
		damage = 1;
	}

	protected void OnEnable() {
		physics.acceleration = Vector3.zero;
		physics.velocity = Vector3.zero;
		transparent = false;
		Invoke("StartTransparencyCheckCoroutine", 0.02f);
	}
	protected void StartTransparencyCheckCoroutine() {
		if (gameObject.activeSelf) {
			StartCoroutine(TransparencyCheck());
		}
	}

	protected IEnumerator TransparencyCheck() {
		while (true) {
			if (!sprite.isVisible) {
				curState = BulletState.none;
				ReturnToPool();
				yield break;
			}

			if (!transparent && InOwnPlayersTerritory() && curState != BulletState.absorbedByMasochist) {
				SetTransparency(true);
			}
			else if (transparent && !InOwnPlayersTerritory()) {
				SetTransparency(false);
			}

			yield return new WaitForSeconds(transparencyCheckCooldown);
		}
	}

	protected virtual void OnTriggerEnter(Collider other) {
		//Destroy bullets upon hitting a killzone
		if (other.tag == "KillZone") {
			curState = BulletState.none;
			ReturnToPool();
		}
		//Deal damage to any other player hit
		else if (other.tag == "Player") {
			PlayerShip playerHit = other.gameObject.GetComponentInParent<PlayerShip>();

			//print("Player hit: " + playerHit.name + "\nOwning Player: " + owningPlayer + "\nThisPlayer.TypeOfShip: " + thisPlayer.typeOfShip);
			if (playerHit.player != owningPlayer) {
				//Masochists with the shield up are immune to incoming bullets
				if (playerHit.typeOfShip == ShipType.masochist) {
					Masochist masochistHit = playerHit as Masochist;
					if (masochistHit != null && masochistHit.shieldUp) {
						return;
					}
				}

				//Do damage to the player hit
				if (GameManager.S.inGame && owningPlayer != Player.none && thisPlayer is Masochist) {  //kinky...
					Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
					playerHit.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
				}
				else {
					playerHit.TakeDamage(damage);
				}

				GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				curState = BulletState.none;
				ReturnToPool();
			}
			//If the bullet was absorbed by the vampire with it's shield up, heal slightly instead of doing damage
			else if (owningPlayer != Player.none && thisPlayer.typeOfShip == ShipType.vampire) {
				if (curState == BulletState.absorbedByVampire) {
					playerHit.TakeDamage(damage * -vampShieldHealAmount);
					curState = BulletState.none;
					damage = 1;
					ReturnToPool();
				}
			}
		}
		//Deal damage to any ProtagShip hit
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			if (GameManager.S.inGame && owningPlayer != Player.none && GameManager.S.players[(int)owningPlayer] is Masochist) {
				Masochist masochistOwningPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
				otherShip.TakeDamage(damage * masochistOwningPlayer.damageMultiplier);
			}
			else {
				otherShip.TakeDamage(damage);
			}

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
			curState = BulletState.none;
			ReturnToPool();
		}
	}

	protected void SetTransparency(bool isTransparent) {
		_transparent = isTransparent;

		if (transparent) {
			Color curColor = sprite.color;
			curColor.a = transparencyAlpha;
			sprite.color = curColor;
			sprite.sortingOrder = -1;

			if (trail != null) {
				trail.startColor = curColor;
			}
		}
		else {
			Color curColor = sprite.color;
			curColor.a = 1;
			sprite.color = curColor;
			sprite.sortingOrder = 0;

			if (trail != null) {
				trail.startColor = curColor;
			}
		}
	}

	protected bool InOwnPlayersTerritory() {

		//Don't do this check on the title screen
		if (GameManager.S.gameState != GameStates.playing) {
			return false;
		}

		//Player1 check
		if (owningPlayer == Player.player1) {
			if (transform.position.x < otherPlayer.worldSpaceMinX) {
				return true;
			}
		}
		//Player2 check
		else if (owningPlayer == Player.player2) {
			if (transform.position.x > otherPlayer.worldSpaceMaxX) {
				return true;
			}
		}
		return false;
	}

	public void SetColor(Color newColor) {
		sprite.color = newColor;
		if (trail != null) {
			trail.startColor = newColor;
		}
	}
}
