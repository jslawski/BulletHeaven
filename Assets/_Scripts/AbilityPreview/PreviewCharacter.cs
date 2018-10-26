using UnityEngine;
using System.Collections;

public class PreviewCharacter : Character {
	public PreviewPlayer previewPlayer {
		get { return player as PreviewPlayer; }
	}

	public Character InitializeCharacter(SelectedCharacterInfo characterInfo) {
		print("Start initializing " + characterInfo.selectingPlayer);
		playerEnum = characterInfo.selectingPlayer;
		characterType = characterInfo.typeOfShip;
		previewPlayer.playerColor = characterInfo.shipColor;
		GetComponent<PreviewShip>().typeOfShip = characterInfo.typeOfShip;
		GetComponent<ShootBomb>().SetBombType(characterType);

		SetSprite();
		return this;
	}

	void SetSprite() {
		switch (characterType) {
			case CharactersEnum.generalist:
				SetSpriteTo("Images/GeneralistShip/GShip6", "Images/GeneralistShip/GShipAnimationController");
				break;
			case CharactersEnum.glassCannon:
				SetSpriteTo("Images/GlassCannonShip/GCShip6", "Images/GlassCannonShip/GCShipAnimationController");
				break;
			case CharactersEnum.masochist:
				SetSpriteTo("Images/MasochistShip/MShip6", "Images/MasochistShip/MShipAnimationController");
				break;
			case CharactersEnum.tank:
				SetSpriteTo("Images/TankyShip/TShip6", "Images/TankyShip/TShipAnimationController");
				break;
			case CharactersEnum.vampire:
				SetSpriteTo("Images/VampireShip/VampireShip6", "Images/VampireShip/VampireShipAnimationController");
				break;
			default:
				Debug.LogError("ShipType " + characterType + " not handled in SetSprite()");
				return;
		}
	}

	void SetSpriteTo(string pathToSprite, string pathToAnimationController) {
		ApplyToAllShips(ship => {
			GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>(pathToSprite);
			GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(pathToAnimationController);
		});
	}
}
