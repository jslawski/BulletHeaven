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

//TODO: Isolate Input device to ShipSelectionControls, rather than here
//TODO: If the for Loop in Update is problematic, set it up to broadcast events for each ShipSelectionControls instance to listen to
//TODO: Setting input keycodes for controllers at runtime.
//TODO: Setting up ShipSelectionControls at runtime, so it's not assumed that 2 players are always selecting at the same time (could be one player selecting for both separately)

//This class is responsible for handling the aspects of ship selection that are universal for both players
//Ex: Listens for input from BOTH players to call scrolling functions when needed
public class UnifiedShipSelectionManager : MonoBehaviour 
{
	public static UnifiedShipSelectionManager instance;

	public PersistentShipInfo persistentInfoPrefab;

	public Player[] players;
	public InputDevice device;

	ShipInfo[] ships;

	public AbilityPreviewScreen abilityPreview;

	public OptionsMenu optionsMenu;
	public Text selectedShipNameField;
	public Text selectedShipDescriptionField;
	[Header("Stat Bar References")]
	public StatBar offenseStat;
	public StatBar defenseStat;
	public StatBar speedStat;
	public StatBar maxHealthStat;
	public StatBar difficultyStat;
	public Text miscStatLabel;
	public StatBar miscStat;

	float minWaitTime = 0.075f;
	float maxWaitTime = 0.075f;

	private List<ShipSelectionControls> shipSelectionControls;

	void Awake() 
	{
		if (instance != null) {
			Destroy(this);
			return;
		}
		instance = this;

		this.persistentInfoPrefab = Resources.Load<PersistentShipInfo>("Prefabs/ShipInfo");
	
		ShipSelectionControls[] currentShipSelectionControls = GetComponentsInChildren<ShipSelectionControls>();
		this.shipSelectionControls = new List<ShipSelectionControls>();

		foreach (ShipSelectionControls controls in currentShipSelectionControls) 
		{
			this.shipSelectionControls.Add(controls);
		}

		DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	void Update () 
	{
		foreach (ShipSelectionControls controls in shipSelectionControls) 
		{
			//Don't allow input while it doesn't have focus
			if (!controls.hasFocus || OptionsMenu.hasFocus) 
			{
				return;
			}
			//Don't allow input while randoming ships
			if (controls.inChooseRandomShipCoroutine || GameManager.S.gameState != GameStates.shipSelect) 
			{
				return;
			}

			#region Keyboard Support
			//Keyboard support
			if (Input.GetKeyDown(controls.right) && !controls.playerReady) 
			{
				controls.Scroll(ScrollDirection.right);
			} 
			else if (Input.GetKeyDown(controls.left) && !controls.playerReady) 
			{
				controls.Scroll(ScrollDirection.left);
			}

			//Ready up
			if (Input.GetKeyDown(controls.A) && !controls.playerReady) 
			{
				if (controls.selectedShip.typeOfShip == ShipType.random) 
				{
					StartCoroutine(controls.RandomShip());
				} 
				else 
				{
					SoundManager.instance.Play("ShipConfirm", 1);
					controls.playerReady = true;
				}
			} 
			else if (Input.GetKeyDown(controls.B) && controls.playerReady) 
			{
				SoundManager.instance.Play("ShipCancel");
				controls.playerReady = false;
			} 
			else if (Input.GetKeyDown(controls.Y) && controls.selectedShip.typeOfShip != ShipType.random) 
			{
				abilityPreview.SetAbilityPreview(controls.selectedShip);
			}

			if (GameManager.S.gameState == GameStates.shipSelect && Input.GetKeyDown(controls.start)) 
			{
				if (AllPlayersReady()) 
				{
					SoundManager.instance.Play("StartGame");
					GameManager.S.gameState = GameStates.countdown;
					GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
				} 
				else 
				{
					optionsMenu.OpenOptionsMenu(device);
				}
			}
			#endregion

			#region Controller Support
			//Controller support
			if (device != null) {
				if ((device.LeftStick.Right.WasPressed || device.DPadRight.WasPressed) && !controls.playerReady) 
				{
					controls.Scroll(ScrollDirection.right);
				} 
				else if ((device.LeftStick.Left.WasPressed || device.DPadLeft.WasPressed) && !controls.playerReady) 
				{
					controls.Scroll(ScrollDirection.left);
				}

				//Ready up
				if (device.Action1.WasPressed && !controls.playerReady) 
				{
					if (controls.selectedShip.typeOfShip == ShipType.random) 
					{
						StartCoroutine(controls.RandomShip());
					} 
					else 
					{
						SoundManager.instance.Play("ShipConfirm", 1);
						controls.playerReady = true;
					}
				}
				else if (device.Action2.WasPressed && controls.playerReady) 
				{
					SoundManager.instance.Play("ShipCancel");
					controls.playerReady = false;
				} 
				else if (device.Action4.WasPressed && controls.selectedShip.typeOfShip != ShipType.random) 
				{
					abilityPreview.SetAbilityPreview(controls.selectedShip);
				}

				if (GameManager.S.gameState == GameStates.shipSelect && device.MenuWasPressed) 
				{
					if (AllPlayersReady()) 
					{
						SoundManager.instance.Play("StartGame");
						GameManager.S.gameState = GameStates.countdown;
						GameManager.S.TransitionScene(GameManager.S.fadeFromShipSelectDuration, "_Scene_Main");
					} 
					else 
					{
						optionsMenu.OpenOptionsMenu(device);
					}
				}
			}
			#endregion
		}
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
