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
	protected PlayerEnum _owningPlayer = PlayerEnum.none;
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != PlayerEnum.none) {
				if (GameManager.S.inGame) {
					thisPlayer = GameManager.S.players[(int)value];
					SetColor(thisPlayer.playerColor);
					targetMovement = GameManager.S.OtherPlayer(thisPlayer).character.GetClosestShip(transform.position).movement;
				}
			}
			else {
				SetColor(unownedBulletColor);
			}
		}
	}
	public Player thisPlayer;
	public ShipMovement targetMovement;
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
		damage = 1;
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
				DestroyThisBullet();
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

	//TODO: Move the ship-specific logic to Ship subclasses and common logic to Ship base class
	protected virtual void OnTriggerEnter(Collider other) {
		//Destroy bullets upon hitting a killzone
		if (other.tag == "KillZone") {
			curState = BulletState.none;
			DestroyThisBullet();
		}
		//Deal damage to any other player hit
		else if (other.tag == "Player") {
			Ship shipHit = other.gameObject.GetComponentInParent<Ship>();
			Character characterHit = shipHit.character;
			
			if (characterHit.playerEnum != owningPlayer) {
				//Masochists with the shield up are immune to incoming bullets
				if (characterHit.characterType == CharactersEnum.masochist) {
					Masochist masochistHit = characterHit as Masochist;
					if (masochistHit != null && masochistHit.masochistShip.shieldUp) {
						return;
					}
				}

				//TODO: Re-add masochist damage multiplier to bullet creation
				//Do damage to the player hit
				shipHit.TakeDamage(damage);

				GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
				curState = BulletState.none;
				DestroyThisBullet();
			}
			//TODO: What the heck? Why is this thisPlayer == vampire? We should re-write this
			//If the bullet was absorbed by the vampire with it's shield up, heal slightly instead of doing damage
			else if (owningPlayer != PlayerEnum.none && thisPlayer.character.characterType == CharactersEnum.vampire) {
				if (curState == BulletState.absorbedByVampire) {
					shipHit.TakeDamage(damage * -vampShieldHealAmount);
					curState = BulletState.none;
					damage = 1;
					DestroyThisBullet();
				}
			}
		}
		//Deal damage to any ProtagShip hit
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			otherShip.TakeDamage(damage);

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
			curState = BulletState.none;
			DestroyThisBullet();
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
		if (owningPlayer == PlayerEnum.player1) {
			if (transform.position.x < targetMovement.worldSpaceMinX) {
				return true;
			}
		}
		//Player2 check
		else if (owningPlayer == PlayerEnum.player2) {
			if (transform.position.x > targetMovement.worldSpaceMaxX) {
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

	protected virtual void DestroyThisBullet() {
		ReturnToPool();
	}
}
