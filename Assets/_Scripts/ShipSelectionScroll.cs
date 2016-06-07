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

			offenseStat.SetStatValue(0);
			defenseStat.SetStatValue(0);
			speedStat.SetStatValue(0);
			maxHealthStat.SetStatValue(0);
			fireRateStat.SetStatValue(0);
			miscStatLabel.text = "Miscellaneous Stat";
			miscStat.SetStatValue(0);
			return;
		}

		selectedShipNameField.text = shipInfo.shipName;

		offenseStat.SetStatValue(shipInfo.offense);
		defenseStat.SetStatValue(shipInfo.defense);
		speedStat.SetStatValue(shipInfo.speed);
		maxHealthStat.SetStatValue(shipInfo.maxHealth);
		fireRateStat.SetStatValue(shipInfo.fireRate);
		miscStatLabel.text = shipInfo.miscLabel;
		miscStat.SetStatValue(shipInfo.miscStat);
	}
}
