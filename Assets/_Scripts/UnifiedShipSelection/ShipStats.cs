using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipStats : MonoBehaviour {
	[SerializeField]
	private Text selectedShipNameField;
	[SerializeField]
	private Text selectedShipDescriptionField;
	[SerializeField]
	private StatBar offenseStat;
	[SerializeField]
	private StatBar defenseStat;
	[SerializeField]
	private StatBar speedStat;
	[SerializeField]
	private StatBar maxHealthStat;
	[SerializeField]
	private StatBar difficultyStat;
	[SerializeField]
	private Text miscStatLabel;
	[SerializeField]
	private StatBar miscStat;

	public void SetStatsForShip(ShipInfo shipInfo) {
		//Gracefully handle the case of no ship selected
		if (shipInfo == null) {
			selectedShipNameField.text = string.Empty;
			selectedShipDescriptionField.text = string.Empty;

			offenseStat.SetStatValue(0, Color.white);
			defenseStat.SetStatValue(0, Color.white);
			speedStat.SetStatValue(0, Color.white);
			maxHealthStat.SetStatValue(0, Color.white);
			difficultyStat.SetStatValue(0, Color.white);
			miscStatLabel.text = "Miscellaneous Stat";
			miscStat.SetStatValue(0, Color.white);
			return;
		}
		//Animate the random ship selection
		else if (shipInfo.typeOfShip == CharactersEnum.random) {
			selectedShipNameField.text = shipInfo.shipName;
			selectedShipDescriptionField.text = shipInfo.description;
			miscStatLabel.text = shipInfo.miscLabel;

			offenseStat.AnimateRandomStats();
			defenseStat.AnimateRandomStats();
			speedStat.AnimateRandomStats();
			maxHealthStat.AnimateRandomStats();
			difficultyStat.AnimateRandomStats();
			miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
			return;
		}

		//Set the stat values for non-special case ships
		selectedShipNameField.text = shipInfo.shipName;
		selectedShipDescriptionField.text = shipInfo.description;

		offenseStat.SetStatValue(shipInfo.offense, shipInfo.shipColor);
		defenseStat.SetStatValue(shipInfo.defense, shipInfo.shipColor);
		speedStat.SetStatValue(shipInfo.speed, shipInfo.shipColor);
		maxHealthStat.SetStatValue(shipInfo.maxHealth, shipInfo.shipColor);
		difficultyStat.SetStatValue(shipInfo.difficulty, shipInfo.shipColor);
		miscStatLabel.text = shipInfo.miscLabel;
		miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
	}
}
