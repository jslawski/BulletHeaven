﻿using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public struct AbilityInfo {
	public int slot;
	public string abilityName;
	public string abilityDescription;
}

public enum SelectionPosition {
	offscreenLeft,
	left,
	selected,
	right,
	offscreenRight,
	invisibleCenter
}

public class ShipInfo : MonoBehaviour {
	private int numSelectionOptions {
		get {
			return selectionMenu.ships.Length;
		}
	}
	ShipSelectionControls selectionMenu;
	
	public PlayerEnum selectingPlayer = PlayerEnum.none;
	private int positionIndex;
	public SelectionPosition position {
		get {
			return (SelectionPosition)Mathf.Min((int)SelectionPosition.invisibleCenter, positionIndex);
		}
	}
	public CharactersEnum typeOfShip;
	public string shipName;
	public string description;
	public Color shipColor;
	public Color shipSecondaryColor;
	public SpriteRenderer spriteRenderer;

	float scrollSpeed = 0.1f;
	float timeSelected = 0f;
	float bounceAmplitude = 0.5f;
	float bounceFrequency = 2f;

	[Header("Ship Display Stats")]
	[Range(0,10)]
	public int offense;
	[Range(0,10)]
	public int defense;
	[Range(0,10)]
	public int speed;
	[Range(0,10)]
	public int maxHealth;
	[Range(0,10)]
	public int difficulty;
	public string miscLabel;
	[Range(0,10)]
	public int miscStat;

	[Header("Ship Abilities")]
	public AbilityInfo[] abilities;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		selectionMenu = GetComponentInParent<ShipSelectionControls>();

		GetShipInfoStrings();
	}

	void Start() {
		positionIndex = Array.IndexOf(selectionMenu.ships, this);
		if (position == SelectionPosition.selected) {
			selectionMenu.selectedShip = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		PositionInfo posInfo;

		//Player has locked in, hide the non-selected ships off-screen
		if (position != SelectionPosition.selected && selectionMenu.playerReady) {
			//Determine whether to hide this ship to the left or right offscreen area
			if (position > SelectionPosition.selected) {
				posInfo = selectionMenu.positionInfos[(int)SelectionPosition.offscreenRight];
			}
			else {
				posInfo = selectionMenu.positionInfos[(int)SelectionPosition.offscreenLeft];
			}
		}
		//Player isn't locked in, show the non-selected ships onscreen off to the side
		else {
			posInfo = selectionMenu.positionInfos[(int)position];
		}

		//Bounce the selected ship
		if (position == SelectionPosition.selected && !selectionMenu.playerReady) {
			posInfo.position.y += bounceAmplitude*Mathf.Cos((2*Mathf.PI)*timeSelected/bounceFrequency);
			timeSelected += Time.deltaTime;
		}

		//Apply the new position information gradually to this ship selection
		transform.position = Vector3.Lerp(transform.position, posInfo.position, scrollSpeed);
		transform.localScale = Vector3.Lerp(transform.localScale, posInfo.scale, scrollSpeed);
		if (typeOfShip == CharactersEnum.random) {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, new Color(1,1,1,0.75f*posInfo.alphaColor.a), scrollSpeed);
		}
		else {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, posInfo.alphaColor, scrollSpeed);
		}
		spriteRenderer.sortingOrder = (int)Mathf.Lerp(spriteRenderer.sortingOrder, posInfo.orderInLayer, scrollSpeed);
	}

	public void Scroll(ScrollDirection scrollDirection) {
		//Deselect this ship if we're moving off of it
		if (selectionMenu.selectedShip == this) {
			selectionMenu.selectedShip = null;
		}

		//Move the ship's position
		//Handle wrap-around from left to right side and vice versa
		positionIndex += ((scrollDirection == ScrollDirection.right) ? 1 : -1);
		positionIndex = (positionIndex == -1) ? numSelectionOptions - 1 : positionIndex % numSelectionOptions;

		//Select this ship if we're moving into the selection slot
		if (position == SelectionPosition.selected) {
			selectionMenu.selectedShip = this;
		}
	}

	//Get the correct strings depeinding on the type of the ship
	void GetShipInfoStrings() {
		switch (typeOfShip) {
			case CharactersEnum.generalist:
				shipName = TextLiterals.SHIP_NAME_LANCELOT;
				description = TextLiterals.SHIP_DESC_LANCELOT;
				miscLabel = TextLiterals.MISC_STAT_LANCELOT;
				abilities[0].abilityName = TextLiterals.ABILITY_NAME_LANCELOT_0;
				abilities[0].abilityDescription = TextLiterals.ABILITY_DESC_LANCELOT_0;
				abilities[1].abilityName = TextLiterals.ABILITY_NAME_LANCELOT_1;
				abilities[1].abilityDescription = TextLiterals.ABILITY_DESC_LANCELOT_1;
				abilities[2].abilityName = TextLiterals.ABILITY_NAME_LANCELOT_2;
				abilities[2].abilityDescription = TextLiterals.ABILITY_DESC_LANCELOT_2;
				abilities[3].abilityName = TextLiterals.ABILITY_NAME_LANCELOT_3;
				abilities[3].abilityDescription = TextLiterals.ABILITY_DESC_LANCELOT_3;
				break;
			case CharactersEnum.vampire:
				shipName = TextLiterals.SHIP_NAME_NOSEFERATU;
				description = TextLiterals.SHIP_DESC_NOSEFERATU;
				miscLabel = TextLiterals.MISC_STAT_NOSEFERATU;
				abilities[0].abilityName = TextLiterals.ABILITY_NAME_NOSEFERATU_0;
				abilities[0].abilityDescription = TextLiterals.ABILITY_DESC_NOSEFERATU_0;
				abilities[1].abilityName = TextLiterals.ABILITY_NAME_NOSEFERATU_1;
				abilities[1].abilityDescription = TextLiterals.ABILITY_DESC_NOSEFERATU_1;
				abilities[2].abilityName = TextLiterals.ABILITY_NAME_NOSEFERATU_2;
				abilities[2].abilityDescription = TextLiterals.ABILITY_DESC_NOSEFERATU_2;
				abilities[3].abilityName = TextLiterals.ABILITY_NAME_NOSEFERATU_3;
				abilities[3].abilityDescription = TextLiterals.ABILITY_DESC_NOSEFERATU_3;
				break;
			case CharactersEnum.masochist:
				shipName = TextLiterals.SHIP_NAME_TEST_SUBJECT_P41N;
				description = TextLiterals.SHIP_DESC_TEST_SUBJECT_P41N;
				miscLabel = TextLiterals.MISC_STAT_TEST_SUBJECT_P41N;
				abilities[0].abilityName = TextLiterals.ABILITY_NAME_TEST_SUBJECT_P41N_0;
				abilities[0].abilityDescription = TextLiterals.ABILITY_DESC_TEST_SUBJECT_P41N_0;
				abilities[1].abilityName = TextLiterals.ABILITY_NAME_TEST_SUBJECT_P41N_1;
				abilities[1].abilityDescription = TextLiterals.ABILITY_DESC_TEST_SUBJECT_P41N_1;
				abilities[2].abilityName = TextLiterals.ABILITY_NAME_TEST_SUBJECT_P41N_2;
				abilities[2].abilityDescription = TextLiterals.ABILITY_DESC_TEST_SUBJECT_P41N_2;
				abilities[3].abilityName = TextLiterals.ABILITY_NAME_TEST_SUBJECT_P41N_3;
				abilities[3].abilityDescription = TextLiterals.ABILITY_DESC_TEST_SUBJECT_P41N_3;
				break;
			case CharactersEnum.tank:
				shipName = TextLiterals.SHIP_NAME_JUNK_DRIVER;
				description = TextLiterals.SHIP_DESC_JUNK_DRIVER;
				miscLabel = TextLiterals.MISC_STAT_JUNK_DRIVER;
				abilities[0].abilityName = TextLiterals.ABILITY_NAME_JUNK_DRIVER_0;
				abilities[0].abilityDescription = TextLiterals.ABILITY_DESC_JUNK_DRIVER_0;
				abilities[1].abilityName = TextLiterals.ABILITY_NAME_JUNK_DRIVER_1;
				abilities[1].abilityDescription = TextLiterals.ABILITY_DESC_JUNK_DRIVER_1;
				abilities[2].abilityName = TextLiterals.ABILITY_NAME_JUNK_DRIVER_2;
				abilities[2].abilityDescription = TextLiterals.ABILITY_DESC_JUNK_DRIVER_2;
				abilities[3].abilityName = TextLiterals.ABILITY_NAME_JUNK_DRIVER_3;
				abilities[3].abilityDescription = TextLiterals.ABILITY_DESC_JUNK_DRIVER_3;
				break;
			case CharactersEnum.glassCannon:
				shipName = TextLiterals.SHIP_NAME_REDEYE;
				description = TextLiterals.SHIP_DESC_REDEYE;
				miscLabel = TextLiterals.MISC_STAT_REDEYE;
				abilities[0].abilityName = TextLiterals.ABILITY_NAME_REDEYE_0;
				abilities[0].abilityDescription = TextLiterals.ABILITY_DESC_REDEYE_0;
				abilities[1].abilityName = TextLiterals.ABILITY_NAME_REDEYE_1;
				abilities[1].abilityDescription = TextLiterals.ABILITY_DESC_REDEYE_1;
				abilities[2].abilityName = TextLiterals.ABILITY_NAME_REDEYE_2;
				abilities[2].abilityDescription = TextLiterals.ABILITY_DESC_REDEYE_2;
				abilities[3].abilityName = TextLiterals.ABILITY_NAME_REDEYE_3;
				abilities[3].abilityDescription = TextLiterals.ABILITY_DESC_REDEYE_3;
				break;
			case CharactersEnum.random:
				break;
			default:
				Debug.LogWarning("<color='red'>ERROR: Ship type " + typeOfShip + " not found!</color>");
				break;
		}
	}
}
