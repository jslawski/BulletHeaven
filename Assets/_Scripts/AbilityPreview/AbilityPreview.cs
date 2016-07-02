using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityPreview : MonoBehaviour {
	AbilityPreviewScreen previewScreen;
	bool _selected = false;
	public bool selected {
		get {
			return _selected;
		}
		set {
			_selected = value;
			if (selected) {
				previewScreen.SetSelectedAbility(abilityInfo);
			}
		}
	}
	Image selectedBorder;
	Color selectedColor = new Color(103f/255f,176f/255f,1,255f/255f);
	Color unselectedColor;

	float selectionLerp = 8.75f;

	AbilityInfo abilityInfo;
	Text abilityNameField;

	// Use this for initialization
	void Awake () {
		selectedBorder = GetComponent<Image>();
		previewScreen = GetComponentInParent<AbilityPreviewScreen>();
		abilityNameField = GetComponentInChildren<Text>();
		unselectedColor = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (selected) {
			selectedBorder.color = Color.Lerp(selectedBorder.color, selectedColor, Time.fixedDeltaTime*selectionLerp);
		}
		else {
			selectedBorder.color = Color.Lerp(selectedBorder.color, unselectedColor, Time.fixedDeltaTime*selectionLerp);
		}
	}

	public void SetAbilityInfo(AbilityInfo _abilityInfo) {
		abilityInfo = _abilityInfo;
		abilityNameField.text = abilityInfo.abilityName;
		PlayAbilityPreview(abilityInfo.slot);
	}

	void PlayAbilityPreview(int slot) {
		switch (previewScreen.selectedShip.typeOfShip) {
			//Generalist ship
			case ShipType.generalist:
				switch (slot) {
					case 0:
						StartCoroutine(Generalist0Preview());
						break;
					case 1:
						break;
					case 2:
						break;
					case 3:
						break;
				}
				break;
		}
	}

	IEnumerator Generalist0Preview() {
		yield return null;
	}
}
