﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityPreviewScreen : MonoBehaviour {
	public PreviewGameManager previewGameManager;
	[SerializeField]
	ShipSelectionManager shipSelection;
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

	public ShipInfo selectedShip;
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

	// Use this for initialization
	void Awake () {
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
			if (shipSelection.device.Action4.WasPressed) {
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

	public void	SetAbilityPreview(ShipInfo shipInfo) {
		gameObject.SetActive(true);
		shipSelection.hasFocus = false;
		previewCamera.gameObject.SetActive(true);
		//Tell the ability preview ship to initialize itself with this info
		previewGameManager.players[(int)shipSelection.player].InitializeShip(shipInfo);

		ShipInfo targetShipInfo = new ShipInfo();
		targetShipInfo.selectingPlayer = (previewGameManager.sceneOwner == Player.player1) ? Player.player2 : Player.player1;
		if (shipInfo.typeOfShip != ShipType.tank) {
			targetShipInfo.shipColor = new Color(0, 1, 15f / 255f);
			targetShipInfo.typeOfShip = ShipType.tank;
			targetShipInfo.hitBoxOffset = 0f;
			targetShipInfo.hitBoxRadius = 0.6f;
		}
		else {
			targetShipInfo.shipColor = new Color(57f/255f, 155f/255f, 234f / 255f);
			targetShipInfo.typeOfShip = ShipType.glassCannon;
			targetShipInfo.hitBoxOffset = -0.85f;
			targetShipInfo.hitBoxRadius = 0.34f;
		}
		previewGameManager.players[(int)previewGameManager.target.player].InitializeShip(targetShipInfo);

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
