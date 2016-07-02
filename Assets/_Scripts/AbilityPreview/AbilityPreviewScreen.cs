using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityPreviewScreen : MonoBehaviour {
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
	void Start () {

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
	}
}
