using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityPreviewScreen : MonoBehaviour {
	public PreviewGameManager previewGameManager;
	[SerializeField]
	ShipSelectionControls shipSelection;
	[SerializeField]
	Text shipNameField;
	[SerializeField]
	AbilityPreview[] abilityBoxes;
	[SerializeField]
	Text descriptionField;
	[SerializeField]
	Text abilityNamePreviewField;
	[SerializeField]
	Camera previewCamera;

	public SelectedCharacterInfo selectedShip;
	int _selectedAbility = 0;
	int selectedAbility {
		get {
			return _selectedAbility;
		}
		set {
			abilityBoxes[_selectedAbility].selected = false;
			_selectedAbility = value;
			abilityBoxes[_selectedAbility].selected = true;
		}
	}

	// Update is called once per frame
	void Update () {
		if (shipSelection.hasFocus) {
			return;
		}

		//Close the ability preview screen
		if (Input.GetKeyDown(shipSelection.Y)) {
			HideAbilityPreviewPanel();
		}

		//Scroll between abilities
		if (Input.GetKeyDown(shipSelection.left)) {
			if (selectedAbility == 0) {
				selectedAbility = abilityBoxes.Length - 1;
			}
			else {
				selectedAbility--;
			}
		}
		else if (Input.GetKeyDown(shipSelection.right)) {
			if (selectedAbility == abilityBoxes.Length - 1) {
				selectedAbility = 0;
			}
			else {
				selectedAbility++;
			}
		}

		//Controller input
		if (shipSelection.device != null) {
			//Close the ability preview screen
			if (shipSelection.device.Action4.WasPressed || shipSelection.device.Action2.WasPressed) {
				HideAbilityPreviewPanel();
			}

			//Scroll between abilities
			if (shipSelection.device.LeftStick.Left.WasPressed ||shipSelection.device.DPadLeft.WasPressed) {
				if (selectedAbility == 0) {
					selectedAbility = abilityBoxes.Length - 1;
				}
				else {
					selectedAbility--;
				}
			}
			else if (shipSelection.device.LeftStick.Right.WasPressed || shipSelection.device.DPadRight.WasPressed) {
				if (selectedAbility == abilityBoxes.Length - 1) {
					selectedAbility = 0;
				}
				else {
					selectedAbility++;
				}
			}
		}
	}

	public void	SetAbilityPreview(SelectedCharacterInfo shipInfo) {
		gameObject.SetActive(true);
		previewGameManager.gameObject.SetActive(true);
		shipSelection.hasFocus = false;
		previewCamera.gameObject.SetActive(true);
		//Tell the ability preview ship to initialize itself with this info
		previewGameManager.previewPlayers[(int)shipSelection.player].InstantiateCharacter(shipInfo);

		SelectedCharacterInfo targetShipInfo = new SelectedCharacterInfo();
		targetShipInfo.selectingPlayer = (previewGameManager.sceneOwner == PlayerEnum.player1) ? PlayerEnum.player2 : PlayerEnum.player1;
		if (shipInfo.typeOfShip != CharactersEnum.tank) {
			targetShipInfo.shipColor = new Color(0, 1, 15f / 255f);
			targetShipInfo.typeOfShip = CharactersEnum.tank;
		}
		else {
			targetShipInfo.shipColor = new Color(57f/255f, 155f/255f, 234f / 255f);
			targetShipInfo.typeOfShip = CharactersEnum.glassCannon;
		}

		PreviewPlayer targetPreviewPlayer = previewGameManager.previewPlayers[(int)GameManager.S.OtherPlayerEnum(shipSelection.player)];
		targetPreviewPlayer.InstantiateCharacter(targetShipInfo);
		previewGameManager.target = targetPreviewPlayer.character.ship;

		//Don't waste time setting information if it's not new
		if (shipInfo == selectedShip) {
			return;
		}
		selectedShip = shipInfo;

		for (int i = 0; i < shipInfo.abilities.Length; i++) {
			abilityBoxes[i].SetAbilityInfo(shipInfo.abilities[i]);
		}

		shipNameField.text = shipInfo.shipName;
		selectedAbility = 0;
	}

	void HideAbilityPreviewPanel() {
		gameObject.SetActive(false);
		shipSelection.hasFocus = true;
		previewCamera.gameObject.SetActive(false);
	}

	public void SetSelectedAbility(AbilityInfo abilityInfo) {
		abilityNamePreviewField.text = abilityInfo.abilityName + " Preview";
		descriptionField.text = abilityInfo.abilityDescription.Replace("\\n", "\n");
		previewGameManager.PlayAbilityPreview(abilityInfo.slot);
	}
}
