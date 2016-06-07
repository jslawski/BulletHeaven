using UnityEngine;
using System.Collections;

public class ShipInfo : MonoBehaviour {
	ShipSelectionScroll selectionMenu;

	public SelectionPosition position;
	public string shipName;
	public SpriteRenderer spriteRenderer;

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
	public int fireRate;
	public string miscLabel;
	[Range(0,10)]
	public int miscStat;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		selectionMenu = GetComponentInParent<ShipSelectionScroll>();

	}

	void Start() {
		if (position == SelectionPosition.selected) {
			selectionMenu.selectedShip = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		PositionInfo posInfo = selectionMenu.positionInfos[(int)position];
        transform.position = Vector3.Lerp(transform.position, posInfo.position, 0.1f);
		transform.localScale = Vector3.Lerp(transform.localScale, posInfo.scale, 0.1f);
		spriteRenderer.color = Color.Lerp(spriteRenderer.color, posInfo.alphaColor, 0.1f);
		spriteRenderer.sortingOrder = (int)Mathf.Lerp(spriteRenderer.sortingOrder, posInfo.orderInLayer, 0.1f);
	}

	public void Scroll(bool toTheRight) {
		//Deselect this ship if we're moving off of it
		if (selectionMenu.selectedShip == this) {
			//print(gameObject.name + " is no longer selected.");
			selectionMenu.selectedShip = null;
		}

		//Move the ship's position
		//Handle wrap-around from left to right side
		if (position == SelectionPosition.offscreenLeft && !toTheRight) {
			position = SelectionPosition.offscreenRight;
		}
		//Handle wrap-around from right to left side
		else if (position == SelectionPosition.offscreenRight && toTheRight) {
			position = SelectionPosition.offscreenLeft;
		}
		else {
			position += (toTheRight ? 1 : -1);
		}

		//Select this ship if we're moving into the selection slot
		if (position == SelectionPosition.selected) {
			//print(gameObject.name + " is now selected.");
			selectionMenu.selectedShip = this;
		}
	}
}
