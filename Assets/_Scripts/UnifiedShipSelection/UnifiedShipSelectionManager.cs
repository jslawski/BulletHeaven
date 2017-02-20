using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.SceneManagement;

/*public enum ScrollDirection 
{
	left, 
	right
}*/

//TODO: Setting input keycodes for controllers at runtime.
//TODO: Setting up ShipSelectionControls at runtime, so it's not assumed that 2 players are always selecting at the same time (could be one player selecting for both separately)

//This class is responsible for handling the aspects of ship selection that are universal for both players
//Ex: Listens for input from BOTH players to call scrolling functions when needed
public class UnifiedShipSelectionManager : MonoBehaviour 
{
	public static UnifiedShipSelectionManager instance;

	private const float MinWaitTimeForInputInSeconds = 0.25f;

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

		DontDestroyOnLoad(this.gameObject);
	}

	public IEnumerator SelectPlayerOneForSinglePlayer()
	{
		Debug.Log("===JPS=== Starting Player One Coroutine");

		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		this.shipSelectionControls[(int)Player.player1].playerReady = false;
		this.shipSelectionControls[(int)Player.player1].SetDevice(Player.player1, Player.player1);

		//Continue looping until player 1's ship has been confirmed
		while (this.shipSelectionControls[(int)Player.player1].playerReady == false) 
		{
			yield return null;
		}

		//Move on to player 2 (the COM's) ship
		StartCoroutine(this.SelectComForSinglePlayer());
	}

	private IEnumerator SelectComForSinglePlayer()
	{
		Debug.Log("===JPS=== Starting COM Coroutine");

		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		this.shipSelectionControls[(int)Player.player2].playerReady = false;
		this.shipSelectionControls[(int)Player.player2].SetDevice(Player.player2, Player.player1);  //Mainly used to set the Player field.  Redundantly sets the device before the handoff.  TODO: JPS Streamline this...
		this.HandoffDevice(Player.player1, Player.player2);

		//Continue looping until COM's ship has been confirmed
		while (this.shipSelectionControls[(int)Player.player2].playerReady == false) 
		{
			//If the cancel button is pressed at any time when selecting the computer ship, cancel
			//out and return to selecting player 1's ship
			if (this.shipSelectionControls[(int)Player.player2].device.Action2.WasPressed) 
			{
				this.shipSelectionControls[(int)Player.player1].CancelPlayer();
				this.HandoffDevice(Player.player2, Player.player1);
				StartCoroutine(this.SelectPlayerOneForSinglePlayer());
				break;
			}

			yield return null;
		}
			
		//Move on to player 2 (the COM's) ship only if all players are ready
		if (this.AllPlayersReady() == true) 
		{
			StartCoroutine(this.WaitForStartGameForSinglePlayer());
		}
	}

	private IEnumerator WaitForStartGameForSinglePlayer()
	{
		Debug.Log("===JPS=== Starting Waiting Coroutine");

		yield return new WaitForSeconds(UnifiedShipSelectionManager.MinWaitTimeForInputInSeconds);

		//Wait for the game to progress to the next state
		while (GameManager.S.gameState == GameStates.shipSelect && !this.shipSelectionControls[(int)Player.player2].device.MenuWasPressed) 
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
		this.HandoffDevice(Player.player2, Player.player1);
	}

	private void HandoffDevice(Player sourcePlayer, Player destinationPlayer)
	{
		if (this.shipSelectionControls[(int)sourcePlayer].device == null) 
		{
			Debug.LogError("UnifiedShipSelectionManager.HandoffDeviceToPlayerOne: No device found in source ShipSelectionControls. Handoff failed.");
		}

		this.shipSelectionControls[(int)destinationPlayer].device = this.shipSelectionControls[(int)sourcePlayer].device;
		this.shipSelectionControls[(int)sourcePlayer].device = null;
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
