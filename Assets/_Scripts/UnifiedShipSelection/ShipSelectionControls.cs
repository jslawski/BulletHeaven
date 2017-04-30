using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InControl;

//This behavior is responsible for the player-specific controls of selecting a ship
public class ShipSelectionControls : MonoBehaviour {
	public KeyCode left,right,A,B,Y,start;
	public bool hasFocus = true;
	public bool playerReady = false;
	public AbilityPreviewScreen abilityPreview;
	public OptionsMenu optionsMenu;  			//TODO: JPS Not a big fan of both selection controls having a reference to the same object.  Maybe we could reduce it to one reference somewhere in the UnifiedShipSelectionManager
	public PlayerEnum player; 						//TODO: Have this not set in inspector?
	public InputDevice device;

	public PositionInfo[] positionInfos;        //Information about each selection position (off-screen left, left, selected, etc.)
												//such as the world position, alpha value, and orderInLayer value
												//Set in inspector JPS: TODO: Have this set in code, in order to allow for easier addition of ships in the future

	public PersistentShipInfo persistentInfoPrefab;

	[HideInInspector]
	public ShipInfo[] ships;

	[SerializeField]
	private ShipStats shipStats;

	private bool inChooseRandomShipCoroutine = false;

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
	[SerializeField]
	private SpriteRenderer shipSelectionBackground;

	void Awake()
	{
		this.persistentInfoPrefab = Resources.Load<PersistentShipInfo>("Prefabs/ShipInfo");

		this.ships = GetComponentsInChildren<ShipInfo>();
		foreach (ShipInfo ship in ships) {
			ship.selectingPlayer = this.player;
		}

		#region Keyboard Support
		if (this.player == PlayerEnum.player1) {
			this.left = KeyCode.A;
			this.right = KeyCode.D;
			this.A = KeyCode.Alpha1;
			this.B = KeyCode.Alpha2;
			this.Y = KeyCode.Alpha4;
			this.start = KeyCode.Space;
		}
		else if	(this.player == PlayerEnum.player2) {
			this.left = KeyCode.LeftArrow;
			this.right = KeyCode.RightArrow;
			this.A = KeyCode.Keypad1;
			this.B = KeyCode.Keypad2;
			this.Y = KeyCode.Keypad4;
			this.start = KeyCode.KeypadEnter;
		}
		#endregion
	}

	void Update () 
	{
		shipSelectionBackground.color = Color.Lerp(shipSelectionBackground.color, selectedShip.shipColor, 5*Time.deltaTime);

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

		//For single player, a null device means that no input should be accepted.  Only a player with a device will be selecting ships (for both player 1 and the COM)
		//While a null device is checked for below, this is mostly here to maintain support for keyboard in multiplayer only.
		//Removing it will allow keyboard to select ships for the COM while Player 1 is using a controller to select their ship, which messes up the flow of the coroutines in UnifiedShipSelectionManager.
		if (this.device == null && GameManager.S.singlePlayer == true) 
		{
			return;
		}

		#region Controller Support
		if (device != null){
			if ((this.device.LeftStick.Right.WasPressed || this.device.DPadRight.WasPressed) && this.playerReady == false) 
			{
				this.Scroll(ScrollDirection.left);
			} 
			else if ((this.device.LeftStick.Left.WasPressed || this.device.DPadLeft.WasPressed) && this.playerReady == false) 
			{
				this.Scroll(ScrollDirection.right);
			}

			//Ready up
			if (this.device.Action1.WasPressed && this.playerReady == false) 
			{
				if (this.selectedShip.typeOfShip == CharactersEnum.random) 
				{
					StartCoroutine(this.RandomShip());
				} 
				else 
				{
					SoundManager.instance.Play("ShipConfirm", 1);
					this.playerReady = true;
				}
			}
			else if (this.device.Action2.WasPressed && this.playerReady == true) 
			{
				this.CancelPlayer();
			} 
			else if (this.device.Action4.WasPressed && this.selectedShip.typeOfShip != CharactersEnum.random) 
			{
				this.abilityPreview.SetAbilityPreview(this.selectedShip);
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
					this.optionsMenu.OpenOptionsMenu(this.device);
				}
			}
		}
		#endregion

		#region Keyboard Support
		//Keyboard support
		if (Input.GetKeyDown(this.right) && this.playerReady == false) {
			this.Scroll(ScrollDirection.left);
		}
		else if (Input.GetKeyDown(this.left) && this.playerReady == false) {
			this.Scroll(ScrollDirection.right);
		}

		//Ready up
		if (Input.GetKeyDown(this.A) && this.playerReady == false) {
			if (this.selectedShip.typeOfShip == CharactersEnum.random) {
				StartCoroutine(this.RandomShip());
			}
			else {
				SoundManager.instance.Play("ShipConfirm", 1);
				print(this.player + " is ready.");
				this.playerReady = true;
			}
		}
		else if (Input.GetKeyDown(this.B) && this.playerReady == true) {
			SoundManager.instance.Play("ShipCancel");
			print(this.player + " is no longer ready.");
			this.playerReady = false;
		}
		else if (Input.GetKeyDown(this.Y) && this.selectedShip.typeOfShip != CharactersEnum.random) {
			this.abilityPreview.SetAbilityPreview(this.selectedShip);
		}

		if (GameManager.S.gameState == GameStates.shipSelect && Input.GetKeyDown(this.start)) {
			if (UnifiedShipSelectionManager.instance.AllPlayersReady()) {
				SoundManager.instance.Play("StartGame");
				GameManager.S.gameState = GameStates.countdown;
				GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
			}
			else {
				this.optionsMenu.OpenOptionsMenu(this.device);
			}
		}
		#endregion
	}

	public void CancelPlayer()
	{
		SoundManager.instance.Play("ShipCancel");
		this.playerReady = false;
	}

	public void SetDevice(PlayerEnum controllingPlayer)
	{
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
		int numPositions = ships.Length;
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
