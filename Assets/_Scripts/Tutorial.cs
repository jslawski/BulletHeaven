using UnityEngine;
using System.Collections;
using InControl;

public class Tutorial : MonoBehaviour {
	int curPanel = 0;
	public GameObject[] panels;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (panels.Length == 0) {
			return;
		}

		if (InputManager.ActiveDevice.Action1.WasPressed) {
			AdvanceToNextPanel();
		}
		else if (InputManager.ActiveDevice.Action2.WasPressed) {
			GoToPreviousPanel();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
			AdvanceToNextPanel();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
			GoToPreviousPanel();
		}
	}

	void AdvanceToNextPanel() {
		if (curPanel == panels.Length - 1) {
			return;
		}

		panels[curPanel++].SetActive(false);
		panels[curPanel].SetActive(true);
	}

	void GoToPreviousPanel() {
		if (curPanel == 0) {
			return;
		}

		panels[curPanel--].SetActive(false);
		panels[curPanel].SetActive(true);
	}
}
