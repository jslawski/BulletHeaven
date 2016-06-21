using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonHelpUI : MonoBehaviour {
	public bool onCanvas = false;
	Image[] UIbuttons;
	SpriteRenderer[] buttons;
	Color disabledColor = new Color(1,1,1,65f/255f);

	// Use this for initialization
	void Awake () {
		if (onCanvas) {
			UIbuttons = GetComponentsInChildren<Image>();
		}
		else {
			buttons = GetComponentsInChildren<SpriteRenderer>();
		}
	}

	public void SetButtons(bool aEnabled, bool bEnabled, bool xEnabled, bool yEnabled) {
		//print("SetButtons(" + aEnabled + ", " + bEnabled + ", " + xEnabled + ", " + yEnabled + ") called.");
		if (onCanvas) {
			if (!aEnabled && !bEnabled && !xEnabled && !yEnabled) {
				foreach (var button in UIbuttons) {
					button.enabled = false;
				}
				return;
			}
			else {
				foreach (var button in UIbuttons) {
					button.enabled = true;
				}
			}

			UIbuttons[0].color = aEnabled ? Color.white : disabledColor;
			UIbuttons[1].color = bEnabled ? Color.white : disabledColor;
			UIbuttons[2].color = xEnabled ? Color.white : disabledColor;
			UIbuttons[3].color = yEnabled ? Color.white : disabledColor;
		}
		else {
			if (!aEnabled && !bEnabled && !xEnabled && !yEnabled) {
				foreach (var button in buttons) {
					button.enabled = false;
				}
				return;
			}
			else {
				foreach (var button in buttons) {
					button.enabled = true;
				}
			}

			buttons[0].color = aEnabled ? Color.white : disabledColor;
			buttons[1].color = bEnabled ? Color.white : disabledColor;
			buttons[2].color = xEnabled ? Color.white : disabledColor;
			buttons[3].color = yEnabled ? Color.white : disabledColor;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
