using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public PlayerEnum playerEnum;
	public Color playerColor;
	public Character character;
	public InputDevice device;

	private Player _otherPlayer = null;
	public Player otherPlayer {
		get {
			if (_otherPlayer == null) {
				_otherPlayer = GameManager.S.OtherPlayer(this);
			}
			return _otherPlayer;
		}
	}

	//Player-related UI elements
	public DurationBar durationBar;
	public GameObject finishAttackPrompt;
	public Ammo[] ammoImages;

	//Player movement bounds
	public float viewportMinX;
	public float viewportMaxX;
	public float viewportMinY;
	public float viewportMaxY;
	//In world space
	public float worldSpaceMinX;
	public float worldSpaceMaxX;
	public float worldSpaceMinY;
	public float worldSpaceMaxY;

	public virtual void Awake() {
		Vector3 worldSpaceMax = Camera.main.ViewportToWorldPoint(new Vector3(viewportMaxX, viewportMaxY, 0));
		worldSpaceMaxX = worldSpaceMax.x;
		worldSpaceMaxY = worldSpaceMax.y;
		Vector3 worldSpaceMin = Camera.main.ViewportToWorldPoint(new Vector3(viewportMinX, viewportMinY, 0));
		worldSpaceMinX = worldSpaceMin.x;
		worldSpaceMinY = worldSpaceMin.y;
	}

	public virtual Character InstantiateCharacter(SelectedCharacterInfo characterInfo) {
		Debug.Assert(characterInfo.selectingPlayer == playerEnum, "Asked " + playerEnum.ToString() + " to instantiate a character for " + characterInfo.selectingPlayer.ToString());
		
		PlayerEnum otherPlayerEnum = GameManager.S.OtherPlayerEnum(playerEnum);
		Color newShipColor = characterInfo.shipColor;
		//If both players are running the same type of ship, use the secondary color for the ship instead
		if (characterInfo.selectingPlayer != PlayerEnum.player1 && characterInfo.typeOfShip == GameManager.S.players[(int)otherPlayerEnum].character.characterType) {
			newShipColor = characterInfo.shipSecondaryColor;
		}

		string prefabPath = "Prefabs/Ships/";
		switch (characterInfo.typeOfShip) {
			case CharactersEnum.generalist:
				prefabPath += "Generalist";
				break;
			case CharactersEnum.glassCannon:
				prefabPath += "GlassCannon";
				break;
			case CharactersEnum.masochist:
				prefabPath += "Masochist";
				break;
			case CharactersEnum.tank:
				prefabPath += "Tank";
				break;
			case CharactersEnum.vampire:
				prefabPath += "Vampire";
				break;
			//case CharactersEnum.twins:
			//	prefabPath += "Twins";
			//	break;
			//case CharactersEnum.swarm:
			//	prefab = swarmPrefab;
			//	break;
			default:
				Debug.LogError("Ship type " + characterInfo.typeOfShip.ToString() + " not found!");
				return null;
		}
		Character prefab = Resources.Load<Character>(prefabPath);

		Quaternion startRot = (playerEnum == PlayerEnum.player1) ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 90);
		this.character = Instantiate<Character>(prefab, this.transform.position, startRot, this.transform);
		this.character.playerEnum = this.playerEnum;
		
		//Set colors for the player
		this.playerColor = newShipColor;
		foreach (var ammo in this.ammoImages) {
			ammo.GetComponent<Image>().color = playerColor;
		}
		if (this.durationBar != null) {
			this.durationBar.SetColor(newShipColor);
		}

		return this.character;
	}

	// Use this for initialization
	IEnumerator Start () {
		while (!GameManager.S.shipsReady) {
			yield return null;
		}

		if (durationBar != null) {
			durationBar.SetPercent(0);
			durationBar.target = character.ship.transform;
		}
	}
}
