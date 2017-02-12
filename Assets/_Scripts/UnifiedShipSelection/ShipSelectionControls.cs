using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This behavior is responsible for the player-specific controls of selecting a ship
public class ShipSelectionControls : MonoBehaviour {
	[HideInInspector]
	public KeyCode left,right,A,B,Y,start;

	public bool hasFocus = true;

	public bool inChooseRandomShipCoroutine = false;

	public bool playerReady = false;

	private ShipInfo[] ships;

	private ShipInfo _selectedShip;
	public ShipInfo selectedShip 
	{
		get {
			return _selectedShip;
		}
		set {
			_selectedShip = value;
			shipStats.SetStatsForShip(value);
		}
	}

	public PositionInfo[] positionInfos;        //Information about each selection position (off-screen left, left, selected, etc.)
												//such as the world position, alpha value, and orderInLayer value
												//Set in inspector JPS: TODO: Have this set in code, in order to allow for easier addition of ships in the future

	[SerializeField]
	private ShipStats shipStats;

	void Start() {
		this.ships = GetComponentsInChildren<ShipInfo>();
	}

	public void Scroll(ScrollDirection scrollDirection) {
		foreach (ShipInfo ship in this.ships) {
			if (ship == null) {
				continue;
			}
			//Scroll each ship in the correct direction
			ship.Scroll(scrollDirection);
			if (scrollDirection == ScrollDirection.right) {
				SoundManager.instance.Play("ShipScroll", 1.1f);
			}
			else {
				SoundManager.instance.Play("ShipScroll", 1f);
			}
		}
	}

	public IEnumerator RandomShip() {
		int numPositions = positionInfos.Length;
		int randNumScrolls;

		float minWaitTime = 0.075f;
		float maxWaitTime = 0.075f;

		this.inChooseRandomShipCoroutine = true;

		//Choose a random number of times to scroll until we have something that won't end back on random
		do {
			randNumScrolls = Random.Range(numPositions, 2 * numPositions);
		}
		while (randNumScrolls % numPositions == 0);

		//Randomly choose to scroll left or right
		ScrollDirection direction = (ScrollDirection)Random.Range(0, 2);

		//Scroll to the randomly selected ship
		float waitTime = minWaitTime;
		for (int i = 0; i < randNumScrolls; i++) {
			Scroll(direction);
			yield return new WaitForSeconds(waitTime);
			waitTime = Mathf.Lerp(minWaitTime, maxWaitTime, (float)i / randNumScrolls);
		}
		inChooseRandomShipCoroutine = false;
	}
}
