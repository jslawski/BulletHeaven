using UnityEngine;
using System.Collections;

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

	protected PhysicsObj physics;
	protected SpriteRenderer spriteRenderer;
	protected ParticleSystem[] particleSystems;
	protected Sprite[] playerBombSprites = new Sprite[2];


	protected float decelerationRate = 0.005f;

	protected void Awake() {
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

	public virtual void Detonate(Attack attackToPerform) { }

	void OnDestroy() {
		//print("Destroyed bomb");
		//Remove this bomb from the bombsInAir queue (only for the main scene, not the title scene)
		if (GameManager.S && owningPlayer != Player.none && Application.loadedLevelName != GameManager.S.titleSceneName) {
			GameManager.S.players[(int)owningPlayer].GetComponent<ShootBomb>().bombsInAir.Remove(this);
		}
	}
}
