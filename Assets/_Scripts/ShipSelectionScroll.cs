using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SelectionPosition {
	offscreenLeft,
	left,
	selected,
	right,
	offscreenRight,
	numPositions
}

[System.Serializable]
public struct PositionInfo {
	public PositionInfo(Vector3 _position, Color _alphaColor, int _sortingOrder, Vector3 _scale) {
		position = _position;
		alphaColor = _alphaColor;
		orderInLayer = _sortingOrder;
		scale = _scale;
	}

	public Vector3 position;
	public Vector3 scale;
	public Color alphaColor;
	public int orderInLayer;
}

public class ShipSelectionScroll : MonoBehaviour {
	ShipInfo[] ships;
	public ShipInfo selectedShip {
		get {
			return _selectedShip;
		}
		set {
			_selectedShip = value;
			SetStatsForShip(value);
		}
	}
	ShipInfo _selectedShip;

	public PositionInfo[] positionInfos;		//Information about each selection position (off-screen left, left, selected, etc.)
												//such as the world position, alpha value, and orderInLayer value

	public Text selectedShipNameField;
	[Header("Stat Bar References")]
	public StatBar offenseStat;
	public StatBar defenseStat;
	public StatBar speedStat;
	public StatBar maxHealthStat;
	public StatBar fireRateStat;
	public Text miscStatLabel;
	public StatBar miscStat;

	// Use this for initialization
	void Start () {
		ships = GetComponentsInChildren<ShipInfo>();
		//ships = new ShipInfo[(int)SelectionPosition.numPositions];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			Scroll(true);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			Scroll(false);
		}
	}

	public void Scroll(bool toTheRight) {
		foreach (var ship in ships) {
			if (ship == null) {
				continue;
			}
			//Scroll each ship in the correct direction
			ship.Scroll(toTheRight);
		}
		//print(selectedShip);
	}

	void SetStatsForShip(ShipInfo shipInfo) {
		//Gracefully handle the case of no ship selected
		if (shipInfo == null) {
			selectedShipNameField.text = "";

			offenseStat.SetStatValue(0, Color.white);
			defenseStat.SetStatValue(0, Color.white);
			speedStat.SetStatValue(0, Color.white);
			maxHealthStat.SetStatValue(0, Color.white);
			fireRateStat.SetStatValue(0, Color.white);
			miscStatLabel.text = "Miscellaneous Stat";
			miscStat.SetStatValue(0, Color.white);
			return;
		}

		selectedShipNameField.text = shipInfo.shipName;

		offenseStat.SetStatValue(shipInfo.offense, shipInfo.shipColor);
		defenseStat.SetStatValue(shipInfo.defense, shipInfo.shipColor);
		speedStat.SetStatValue(shipInfo.speed, shipInfo.shipColor);
		maxHealthStat.SetStatValue(shipInfo.maxHealth, shipInfo.shipColor);
		fireRateStat.SetStatValue(shipInfo.fireRate, shipInfo.shipColor);
		miscStatLabel.text = shipInfo.miscLabel;
		miscStat.SetStatValue(shipInfo.miscStat, shipInfo.shipColor);
	}
}
