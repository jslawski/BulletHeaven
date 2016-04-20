using UnityEngine;
using System.Collections;

public enum Attack {
	leadingShot,
	spiral,
	beam,
	reflector
}

public class Bomb : MonoBehaviour {
	Player _owningPlayer = Player.none;       //Set this when creating a new Bomb
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				spriteRenderer.sprite = playerBombSprites[(int)value];
				//Set the color of each of the wing's particle systems
				for (int i = 0; i < 3; i++) {
					particleSystems[i].startColor = GameManager.S.players[(int)value].playerColor;
				}
				//Only turn on the prewarmed particle glow for this player
				particleSystems[3].gameObject.SetActive((value == Player.player1));
				particleSystems[4].gameObject.SetActive((value == Player.player2));
            }
		}
	}
	public GameObject shockwavePrefab;
	public LeadingShot leadingShotPrefab;
	public SpiralShot spiralShotPrefab;
	public Beam beamShotPrefab;
	public Reflector reflectorPrefab;

	PhysicsObj physics;
	SpriteRenderer spriteRenderer;
	ParticleSystem[] particleSystems;
	Sprite[] playerBombSprites = new Sprite[2];


	float decelerationRate = 0.005f;

	void Awake() {
		physics = GetComponent<PhysicsObj>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		playerBombSprites[0] = Resources.Load<Sprite>("Images/Bomb1");
		playerBombSprites[1] = Resources.Load<Sprite>("Images/Bomb2");
		particleSystems = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		physics.velocity = Vector3.Lerp(physics.velocity, Vector3.zero, decelerationRate);
	}

	void Update() {
		//Destroy the bomb if it goes off-screen (not during title screen though)
		if (!spriteRenderer.isVisible && (Application.loadedLevelName != GameManager.S.titleSceneName)) {
			//print("Bomb " + gameObject.name + " went offscreen and died");
			Destroy(gameObject);
		}

		transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
	}

	public void Detonate(Attack attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		switch (attackToPerform) {
			case Attack.leadingShot:
				LeadingShot newShot = Instantiate(leadingShotPrefab, transform.position, new Quaternion()) as LeadingShot;
				newShot.owningPlayer = owningPlayer;
				newShot.FireBurst();
				break;
			case Attack.spiral:
				SpiralShot spiralShot = Instantiate(spiralShotPrefab, transform.position, new Quaternion()) as SpiralShot;
				spiralShot.owningPlayer = owningPlayer;
				spiralShot.FireBurst();
				break;
			case Attack.beam:
				Beam beamShot = Instantiate(beamShotPrefab, transform.position, new Quaternion()) as Beam;
				beamShot.owningPlayer = owningPlayer;
				break;
			case Attack.reflector:
				Reflector reflectorShot = Instantiate(reflectorPrefab, transform.position, new Quaternion()) as Reflector;
				reflectorShot.owningPlayer = owningPlayer;
				break;
			default:
				Debug.LogError("Attack type " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}

	void OnDestroy() {
		//print("Destroyed bomb");
		//Remove this bomb from the bombsInAir queue (only for the main scene, not the title scene)
		if (GameManager.S && owningPlayer != Player.none && Application.loadedLevelName != GameManager.S.titleSceneName) {
			GameManager.S.players[(int)owningPlayer].GetComponent<ShootBomb>().bombsInAir.Remove(this);
		}
	}
}
