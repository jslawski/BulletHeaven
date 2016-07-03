using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	public PlayerShip thisPlayer;
	public Player _owningPlayer = Player.none;       //Set this when creating a new Bomb
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
				}

				//Move the button UI to always be behind the bomb's trajectory
				if (value == Player.player2 && buttonUI != null) {
					Vector3 buttonPos = buttonUI.transform.localPosition;
					buttonPos.x *= -1;
					buttonUI.transform.localPosition = buttonPos;
				}

				////Only turn on the prewarmed particle glow for this player
				//particleSystems[3].gameObject.SetActive((value == Player.player1));
				//particleSystems[4].gameObject.SetActive((value == Player.player2));
			}
		}
	}

	protected PhysicsObj physics;
	protected SpriteRenderer spriteRenderer;
	protected ParticleSystem[] particleSystems;
	protected ButtonHelpUI buttonUI;
	//protected Sprite[] playerBombSprites = new Sprite[2];


	protected float decelerationRate = 0.25f;

	protected void Awake() {
		physics = GetComponentInParent<PhysicsObj>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (transform.parent != null) {
			buttonUI = transform.parent.GetComponentInChildren<ButtonHelpUI>();
		}
		//playerBombSprites[0] = Resources.Load<Sprite>("Images/Bomb1");
		//playerBombSprites[1] = Resources.Load<Sprite>("Images/Bomb2");
		particleSystems = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		physics.velocity = Vector3.Lerp(physics.velocity, Vector3.zero, Time.fixedDeltaTime*decelerationRate);
	}

	void Update() {
		//Destroy the bomb if it goes off-screen (not during title screen though)
		if (!spriteRenderer.isVisible && (GameManager.S.gameState != GameStates.titleScreen)) {
			//print("Bomb " + gameObject.name + " went offscreen and died");
			Destroy(gameObject);
		}

		transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
	}

	public virtual void Detonate(AttackButtons attackToPerform) { }

	void OnDestroy() {
		//print("Destroyed bomb");
		//Remove this bomb from the bombsInAir queue (only for the main scene, not the title scene)
		if (GameManager.S && owningPlayer != Player.none && GameManager.S.gameState != GameStates.titleScreen) {
			thisPlayer.playerShooting.bombsInAir.Remove(this);
		}

		if (transform.parent != null) {
			Destroy(transform.parent.gameObject);
		}
	}

	public void SetColor(Color newColor) {
		//Set the color of each of the wing's particle systems
		for (int i = 0; i < particleSystems.Length; i++) {
			particleSystems[i].startColor = newColor;
			particleSystems[i].Play();
		}

	}
}
