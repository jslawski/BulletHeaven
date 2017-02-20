using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InControl;

//This behavior is responsible for the player-specific controls of selecting a ship
public class ShipSelectionControls : MonoBehaviour {

	//Unused right now
	[HideInInspector]
	public KeyCode left,right,A,B,Y,start;

	public bool hasFocus = true;

	public bool inChooseRandomShipCoroutine = false;

	public bool playerReady = false;

	public AbilityPreviewScreen abilityPreview;

	public OptionsMenu optionsMenu;  //TODO: JPS Not a big fan of both selection controls having a reference to the same object.  Maybe we could reduce it to one reference somewhere...

	public Player player;

	public InputDevice device;

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

	public PersistentShipInfo persistentInfoPrefab;

	[HideInInspector]
	public ShipInfo[] ships;

	[SerializeField]
	private ShipStats shipStats;

	void Awake()
	{
		this.persistentInfoPrefab = Resources.Load<PersistentShipInfo>("Prefabs/ShipInfo");

		this.ships = GetComponentsInChildren<ShipInfo>();
	}

	void Update () 
	{
		//Don't allow input while it doesn't have focus
		if (!this.hasFocus || OptionsMenu.hasFocus) 
		{
			return;
		}
		//Don't allow input while randoming ships
		if (this.inChooseRandomShipCoroutine || GameManager.S.gameState != GameStates.shipSelect) 
		{
			return;
		}

		//If the device hasn't been set yet (In the case of single-player, with ships being chosen asynchronously by the same controller), abort
		if (this.device == null) 
		{
			return;
		}

		#region Controller Support
		//Controller support
		if ((this.device.LeftStick.Right.WasPressed || this.device.DPadRight.WasPressed) && !this.playerReady) 
		{
			this.Scroll(ScrollDirection.right);
		} 
		else if ((this.device.LeftStick.Left.WasPressed || this.device.DPadLeft.WasPressed) && !this.playerReady) 
		{
			this.Scroll(ScrollDirection.left);
		}

		//Ready up
		if (this.device.Action1.WasPressed && !this.playerReady) 
		{
			if (this.selectedShip.typeOfShip == ShipType.random) 
			{
				StartCoroutine(this.RandomShip());
			} 
			else 
			{
				SoundManager.instance.Play("ShipConfirm", 1);
				this.playerReady = true;
			}
		}
		else if (this.device.Action2.WasPressed && this.playerReady) 
		{
			this.CancelPlayer();
		} 
		else if (this.device.Action4.WasPressed && this.selectedShip.typeOfShip != ShipType.random) 
		{
			abilityPreview.SetAbilityPreview(this.selectedShip);
			}

		if (GameManager.S.gameState == GameStates.shipSelect && this.device.MenuWasPressed) 
		{
			if (UnifiedShipSelectionManager.instance.AllPlayersReady()) 
			{
				SoundManager.instance.Play("StartGame");
				GameManager.S.gameState = GameStates.countdown;
				GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
			} 
			else 
			{
				optionsMenu.OpenOptionsMenu(this.device);
			}
		}
		#endregion
	}

	public void CancelPlayer()
	{
		SoundManager.instance.Play("ShipCancel");
		this.playerReady = false;
	}

	public void SetDevice(Player playerId, Player controllingPlayer)
	{
		this.player = controllingPlayer;
		this.device = GameManager.S.players[(int)controllingPlayer].device;
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
