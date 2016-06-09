using UnityEngine;
using System.Collections;

public class ShipInfo : MonoBehaviour {
	ShipSelectionManager selectionMenu;
	
	public Player selectingPlayer = Player.none;
	public SelectionPosition position;
	public ShipType typeOfShip;
	public string shipName;
	public Color shipColor;
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
	public int fireRate;
	public string miscLabel;
	[Range(0,10)]
	public int miscStat;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		selectionMenu = GetComponentInParent<ShipSelectionManager>();
	}

	void Start() {
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
		if (typeOfShip == ShipType.random) {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, new Color(1,1,1,0.75f*posInfo.alphaColor.a), scrollSpeed);
		}
		else {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, posInfo.alphaColor, scrollSpeed);
		}
		spriteRenderer.sortingOrder = (int)Mathf.Lerp(spriteRenderer.sortingOrder, posInfo.orderInLayer, scrollSpeed);
	}

	public void Scroll(bool toTheRight) {
		//Deselect this ship if we're moving off of it
		if (selectionMenu.selectedShip == this) {
			//print(gameObject.name + " is no longer selected.");
			selectionMenu.selectedShip = null;
		}

		//Move the ship's position
		//Handle wrap-around from left to right side
		if ((int)position == 0 && !toTheRight) {
			position = (SelectionPosition)(((int)SelectionPosition.numPositions)-1);
		}
		//Handle wrap-around from right to left side
		else if ((int)position == ((int)SelectionPosition.numPositions)-1 && toTheRight) {
			position = (SelectionPosition)0;
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
