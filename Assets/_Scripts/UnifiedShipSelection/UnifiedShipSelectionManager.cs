using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.SceneManagement;

public enum ScrollDirection 
{
	left, 
	right
}
	
//This class is responsible for handling the aspects of ship selection that are universal for both players
//It is also responsible for handling single-player selection logic
public class UnifiedShipSelectionManager : MonoBehaviour 
{
	public static UnifiedShipSelectionManager instance;

	private const float MinWaitTimeForInputInSeconds = 0.25f;

	[HideInInspector]
	public List<ShipSelectionControls> shipSelectionControls;

	void Awake() 
	{
		if (instance != null) {
			Destroy(this);
			return;
		}
		instance = this;
	
		ShipSelectionControls[] currentShipSelectionControls = GetComponentsInChildren<ShipSelectionControls>();
		this.shipSelectionControls = new List<ShipSelectionControls>();

		foreach (ShipSelectionControls controls in currentShipSelectionControls) 
		{
			this.shipSelectionControls.Add(controls);
		}

		DontDestroyOnLoad(this.gameObject);  //JPS: Do we know why this is here?  I destroy this game object when we transition to the main scene anyway...
	}

	//Called from ControllerSetup.cs the moment the first device is set in single-player
	#region Single-Player Selection Flow
	public IEnumerator SelectPlayerOneForSinglePlayer()
	{
		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		this.shipSelectionControls[(int)Player.player1].playerReady = false;

		//Continue looping until player 1's ship has been confirmed
		while (this.shipSelectionControls[(int)Player.player1].playerReady == false) 
		{
			yield return null;
		}

		//Move on to player 2 (the COM's) ship
		this.HandoffDevice(Player.player1, Player.player2);
		StartCoroutine(this.SelectComForSinglePlayer());
	}

	private IEnumerator SelectComForSinglePlayer()
	{
		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		this.shipSelectionControls[(int)Player.player2].playerReady = false;

		//Continue looping until COM's ship has been confirmed
		while (this.shipSelectionControls[(int)Player.player2].playerReady == false) 
		{
			//If the cancel button is pressed at any time while selecting the computer ship,
			//cancel out and return to selecting player 1's ship
			if (this.shipSelectionControls[(int)Player.player2].device.Action2.WasPressed) 
			{
				this.shipSelectionControls[(int)Player.player1].CancelPlayer();
				this.HandoffDevice(Player.player2, Player.player1);
				StartCoroutine(this.SelectPlayerOneForSinglePlayer());
				break;
			}

			yield return null;
		}
			
		//Move on to waiting state only if all players are ready
		if (this.AllPlayersReady() == true) 
		{
			StartCoroutine(this.WaitForStartGameForSinglePlayer());
		}
	}

	private IEnumerator WaitForStartGameForSinglePlayer()
	{
		//TODO: Hand controller off to Player 1 here, in case the player mashes start immediately after selecting COM's ship

		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		//Wait for the game to progress to the next state
		while (!this.shipSelectionControls[(int)Player.player2].device.MenuWasPressed) 
		{
			//If the cancel button is pressed at any time while waiting for the game to start, cancel out
			//And return to selecting the COM's ship
			if (this.shipSelectionControls[(int)Player.player2].device.Action2.WasPressed) 
			{
				this.shipSelectionControls[(int)Player.player2].CancelPlayer();
				StartCoroutine(this.SelectComForSinglePlayer());
				break;
			}

			yield return null;
		}

		//Handoff device back to player 1 before the game starts
		if (this.AllPlayersReady() == true) {
			this.HandoffDevice(Player.player2, Player.player1);
		}
	}

	//Pass the device between ShipSelectionControls references depending on where the player is in the process of selecting ships in single-player
	private void HandoffDevice(Player sourcePlayer, Player destinationPlayer)
	{
		if (this.shipSelectionControls[(int)sourcePlayer].device == null) 
		{
			Debug.LogError("UnifiedShipSelectionManager.HandoffDeviceToPlayerOne: No device found in source ShipSelectionControls. Handoff failed.");
		}

		this.shipSelectionControls[(int)destinationPlayer].device = this.shipSelectionControls[(int)sourcePlayer].device;
		this.shipSelectionControls[(int)sourcePlayer].device = null;
	}
	#endregion

	//TODO: JPS: OnLevelWasLoaded is deprecated.  Replace this implementation with SceneManager stuff at some point
	IEnumerator OnLevelWasLoaded(int levelIndex) {
		if (SceneManager.GetActiveScene().name != "_Scene_Main") {
			yield break;
		}

		//Wait for GameManager to initialize itself
		while (GameManager.S == null) {
			yield return null;
		}

		//Wait 2 frames, since GameManager takes 1 frame to re-assign player references
		//TODO: Can we have a more general implementation of this?
		yield return null;
		yield return null;

		for (int curControlsIndex = 0; curControlsIndex < this.shipSelectionControls.Count; curControlsIndex++) 
		{
			//Create the object that will pass the information to GameManager
			PersistentShipInfo persistentShipInfo = Instantiate(this.shipSelectionControls[curControlsIndex].persistentInfoPrefab);
			persistentShipInfo.gameObject.name = "P" + ((int)this.shipSelectionControls[curControlsIndex].player + 1) + "_ShipInfo";
			persistentShipInfo.Initialize(this.shipSelectionControls[curControlsIndex].selectedShip, this.shipSelectionControls[curControlsIndex].device);
		}

		Destroy(this.gameObject);
	}

	public bool AllPlayersReady() {
		foreach (ShipSelectionControls controls in shipSelectionControls) {
			if (!controls.playerReady) {
				return false;
			}
		}
		return true;
	}
}
