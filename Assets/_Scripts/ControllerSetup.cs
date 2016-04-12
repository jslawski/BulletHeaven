using UnityEngine;
using InControl;
using System.Collections;
using System.Collections.Generic;

public class ControllerSetup : MonoBehaviour {
	int maxNumberOfPlayers = 2;
	List<InputDevice> controllersInUse = new List<InputDevice>();

	// Use this for initialization
	void Start () {
		print("# controllers: " + InputManager.Devices.Count);
	}
	
	// Update is called once per frame
	void Update () {
		if (controllersInUse.Count == maxNumberOfPlayers) {
			return;
		}

		//Look for input from any device
		for (int i = 0; i < InputManager.Devices.Count; i++) {
			InputDevice curDevice = InputManager.Devices[i];
			//If start was pressed by a device not already in use, add it
			if (curDevice.MenuWasPressed && !controllersInUse.Contains(curDevice)) {
				GameManager.S.players[controllersInUse.Count].device = curDevice;
                controllersInUse.Add(curDevice);
                print("Player " + controllersInUse.Count + " added.");
			}
		}
	}
}
