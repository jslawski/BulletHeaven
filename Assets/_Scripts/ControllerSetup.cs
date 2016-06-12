using UnityEngine;
using InControl;
using System.Collections;
using System.Collections.Generic;

public class ControllerSetup : MonoBehaviour {
	public static ControllerSetup S;
	public ShipSelectionManager[] shipSelectionMenus;

	public int maxNumberOfPlayers = 2;
	List<InputDevice> controllersInUse = new List<InputDevice>();
	public Player curPlayer = Player.player1;

	float minTimeInSceneForInput = 0.25f;
	float timeInScene = 0;

	void Awake() {
		S = this;
	}

	// Use this for initialization
	void Start () {
		print("# controllers: " + InputManager.Devices.Count);
	}
	
	// Update is called once per frame
	void Update () {
		if (controllersInUse.Count == maxNumberOfPlayers) {
			return;
		}

		if (timeInScene < minTimeInSceneForInput) {
			timeInScene += Time.deltaTime;
		}
		else {
			//Look for input from any device
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				InputDevice curDevice = InputManager.Devices[i];
				//If start was pressed by a device not already in use, add it
				if (curDevice.MenuWasPressed && !controllersInUse.Contains(curDevice)) {
					//In-game controller addition
					if (GameManager.S != null) {
						PlayerShip player = GameManager.S.players[(int)curPlayer];
						player.device = curDevice;
						if (player.controllerPrompt != null) {
							player.controllerPrompt.HidePressStartPrompt();
						}
					}
					//Ship selection menu
					if (shipSelectionMenus.Length > 0 && shipSelectionMenus[(int)curPlayer] != null) {
						shipSelectionMenus[(int)curPlayer].device = curDevice;
					}
					controllersInUse.Add(curDevice);
					print("Player " + controllersInUse.Count + " added.");

					curPlayer = (Player)(((int)curPlayer + 1) % ((int)Player.none));
				}
			}
		}
	}
}
