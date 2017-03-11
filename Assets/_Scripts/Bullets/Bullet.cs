using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BulletState { none, parented, absorbedByMasochist, absorbedByVampire, affectedByBlackHole, absorbedByBlackHole, reflected}

public enum BulletShapes {
	triangle,
	roundedSquare,
	diamond,
	hexagon,
	sun,
	crescent,
	numShapes
}

public class Bullet : PooledObj {
	public GameObject explosionPrefab;
	public float damage;

	protected float transparencyCheckCooldown = 0.4f;
	Color unownedBulletColor = new Color(233f/255f, 154f/255f, 215f/255f);

	public SpriteRenderer sprite;
	ParticleSystem particles;
	ParticleSystemRenderer particleRenderer;
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
					curShape = thisPlayer.character.bulletShape;
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

	Dictionary<BulletShapes, Sprite> sprites = new Dictionary<BulletShapes, Sprite>();
	Dictionary<BulletShapes, Material> shapes = new Dictionary<BulletShapes, Material>();
	BulletShapes _curShape = BulletShapes.numShapes;
	private BulletShapes curShape {
		set {
			if (_curShape == value) return;

			sprite.sprite = sprites[value];
			particleRenderer.material = shapes[value];
			_curShape = value;
		}
	}

	protected float transparencyAlpha = 71f / 255f;
	protected float vampShieldHealAmount = 0.085f;

	public bool IsInteractable() {
		return (curState != BulletState.absorbedByBlackHole && curState != BulletState.absorbedByMasochist && curState != BulletState.absorbedByVampire);
	}

	protected void Awake() {
		particles = GetComponentInChildren<ParticleSystem>();
		particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
		physics = GetComponent<PhysicsObj>();
		sprite = GetComponent<SpriteRenderer>();
		hitbox = GetComponent<SphereCollider>();
		damage = 1;

		shapes.Add(BulletShapes.triangle, Resources.Load<Material>("Materials/Triangle"));
		sprites.Add(BulletShapes.triangle, Resources.Load<Sprite>("Images/BulletTriangle"));
		shapes.Add(BulletShapes.roundedSquare, Resources.Load<Material>("Materials/RoundedSquare"));
		sprites.Add(BulletShapes.roundedSquare, Resources.Load<Sprite>("Images/BulletRoundSquare"));
		shapes.Add(BulletShapes.diamond, Resources.Load<Material>("Materials/Diamond"));
		sprites.Add(BulletShapes.diamond, Resources.Load<Sprite>("Images/BulletDiamond"));
		shapes.Add(BulletShapes.hexagon, Resources.Load<Material>("Materials/Hexagon"));
		sprites.Add(BulletShapes.hexagon, Resources.Load<Sprite>("Images/BulletHex"));
		shapes.Add(BulletShapes.sun, Resources.Load<Material>("Materials/Sun"));
		sprites.Add(BulletShapes.sun, Resources.Load<Sprite>("Images/BulletSun"));
		shapes.Add(BulletShapes.crescent, Resources.Load<Material>("Materials/Crescent"));
		sprites.Add(BulletShapes.crescent, Resources.Load<Sprite>("Images/BulletCrescent"));
	}

	protected void OnEnable() {
		physics.acceleration = Vector3.zero;
		physics.velocity = Vector3.zero;
		transparent = false;
		damage = 1;
		Invoke("StartCoroutines", 0.02f);
	}
	protected void StartCoroutines() {
		if (gameObject.activeSelf) {
			StartCoroutine(TransparencyCheck());
			StartCoroutine(SpriteOrientCoroutine());
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

	protected IEnumerator SpriteOrientCoroutine() {
		while (gameObject.activeSelf) {
			if (!sprite.isVisible) {
				yield break;
			}

			transform.rotation = Quaternion.LookRotation(Vector3.forward, physics.velocity);

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

			if (particles != null) {
				particles.startColor = curColor;
			}
		}
		else {
			Color curColor = sprite.color;
			curColor.a = 1;
			sprite.color = curColor;
			sprite.sortingOrder = 0;

			if (particles != null) {
				particles.startColor = curColor;
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
		if (particles != null) {
			particles.startColor = newColor;
		}
	}

	protected virtual void DestroyThisBullet() {
		ReturnToPool();
	}
}
