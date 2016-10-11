using UnityEngine;
using System.Collections;
using InControl;

public class OptionsMenu : MonoBehaviour {
	InputDevice controllingDevice;
	OptionMenuItem[] options;
	int _curOptionIndex = 0;
	int curOptionIndex {
		get {
			return _curOptionIndex;
		}
		set {
			selectedOption.selected = false;
			_curOptionIndex = value;
			selectedOption.selected = true;
			selectedOption.PlayHighlightedAnimation();
		}
	}

	//All values used in accelerating option selection while a direction is held
	float minChangeTime = 0.1f;
	float maxChangeTime = 0.2f;
	int numActionsTillMinTime = 5;
	int numActionsSinceLastChange = 0;
	float curChangeTime;
	float timeUntilNextChange = 0f;

	public GameObject shipSelectionCoverup;
	public static bool hasFocus = false;
	RectTransform rectTransform;
	Vector2 onscreenAnchorMin = new Vector2(0.2f, 0);
	Vector2 onscreenAnchorMax = new Vector2(0.8f, 1);
	Vector2 offscreenAnchorMin = new Vector2(0.2f, 1);
	Vector2 offscreenAnchorMax = new Vector2(0.8f, 2);
	float panelLerpSpeed = 0.2f;

	OptionMenuItem selectedOption {
		get {
			return options[curOptionIndex];
		}
	}

	ShipSelectionManager[] shipSelections;

	// Use this for initialization
	void Awake() {
		rectTransform = transform.parent.GetComponent<RectTransform>();
		options = GetComponentsInChildren<OptionMenuItem>();
		curChangeTime = maxChangeTime;
	}

	public void OpenOptionsMenu(InputDevice deviceThatOpenedOptions) {
		curOptionIndex = 0;
		controllingDevice = deviceThatOpenedOptions;
		foreach (var option in options) {
			option.SetOptionValue();
		}

		hasFocus = true;
		shipSelectionCoverup.SetActive(true);
	}
	public void CloseOptionsMenu() {
		controllingDevice = null;
		hasFocus = false;
		shipSelectionCoverup.SetActive(false);
	}

	// Update is called once per frame
	void Update() {
		//Move panel to proper position
		Vector2 targetAnchorMin = hasFocus ? onscreenAnchorMin : offscreenAnchorMin;
		Vector2 targetAnchorMax = hasFocus ? onscreenAnchorMax : offscreenAnchorMax;
		rectTransform.anchorMin = Vector2.Lerp(rectTransform.anchorMin, targetAnchorMin, panelLerpSpeed);
		rectTransform.anchorMax = Vector2.Lerp(rectTransform.anchorMax, targetAnchorMax, panelLerpSpeed);

		//Countdown till next option action
		if (timeUntilNextChange > 0) {
			timeUntilNextChange -= Time.deltaTime;
		}

		//Wait until the panel moves most of the way in before allowing input
		//(also prevents input from falling through)
		if (!hasFocus || rectTransform.anchorMin.y > 0.5f) {
			return;
		}

		//Scroll down
		if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && timeUntilNextChange <= 0) {
			timeUntilNextChange = curChangeTime;
			numActionsSinceLastChange++;
			curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
			ScrollDown();
		}
		//Scroll up
		else if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && timeUntilNextChange <= 0) {
			timeUntilNextChange = curChangeTime;
			numActionsSinceLastChange++;
			curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
			ScrollUp();
		}

		//Change option value up
		if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && timeUntilNextChange <= 0) {
			SoundManager.instance.Play("OptionsSelect", 1.1f);
			timeUntilNextChange = curChangeTime;
			numActionsSinceLastChange++;
			curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
			selectedOption.IncreaseOptionValue();
		}
		//Change option value down
		else if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && timeUntilNextChange <= 0) {
			SoundManager.instance.Play("OptionsSelect", 0.9f);
			timeUntilNextChange = curChangeTime;
			numActionsSinceLastChange++;
			curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
			selectedOption.DecreaseOptionValue();
		}

		//When input is released, reset action acceleration values
		if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S) ||
			Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W) ||
			Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D) ||
			Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) {
			timeUntilNextChange = 0;
			numActionsSinceLastChange = 0;
			curChangeTime = maxChangeTime;
		}

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			CloseOptionsMenu();
		}

		//Controller input
		if (controllingDevice != null) {
			//Scroll down
			if ((controllingDevice.LeftStick.Down.IsPressed || controllingDevice.DPadDown.IsPressed) && timeUntilNextChange <= 0) {
				timeUntilNextChange = curChangeTime;
				numActionsSinceLastChange++;
				curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
				ScrollDown();
			}
			//Scroll up
			else if ((controllingDevice.LeftStick.Up.IsPressed || controllingDevice.DPadUp.IsPressed) && timeUntilNextChange <= 0) {
				timeUntilNextChange = curChangeTime;
				numActionsSinceLastChange++;
				curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
				ScrollUp();
			}

			//Change option value up
			if ((controllingDevice.LeftStick.Right.IsPressed || controllingDevice.DPadRight.IsPressed) && timeUntilNextChange <= 0) {
				timeUntilNextChange = curChangeTime;
				numActionsSinceLastChange++;
				curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
				selectedOption.IncreaseOptionValue();
			}
			//Change option value down
			else if ((controllingDevice.LeftStick.Left.IsPressed || controllingDevice.DPadLeft.IsPressed) && timeUntilNextChange <= 0) {
				timeUntilNextChange = curChangeTime;
				numActionsSinceLastChange++;
				curChangeTime = Mathf.Lerp(maxChangeTime, minChangeTime, (float)numActionsSinceLastChange / numActionsTillMinTime);
				selectedOption.DecreaseOptionValue();
			}

			//When input is released, reset action acceleration values
			if (controllingDevice.LeftStick.Down.WasReleased || controllingDevice.DPadDown.WasReleased ||
				controllingDevice.LeftStick.Up.WasReleased || controllingDevice.DPadUp.WasReleased ||
				controllingDevice.LeftStick.Right.WasReleased || controllingDevice.DPadRight.WasReleased ||
				controllingDevice.LeftStick.Left.WasReleased || controllingDevice.DPadLeft.WasReleased) {
				timeUntilNextChange = 0;
				numActionsSinceLastChange = 0;
				curChangeTime = maxChangeTime;
			}
		}
	}

	void ScrollUp() {
		if (curOptionIndex == 0) {
			curOptionIndex = options.Length-1;
		}
		else {
			curOptionIndex--;
		}
	}
	void ScrollDown() {
		if (curOptionIndex == options.Length - 1) {
			curOptionIndex = 0;
		}
		else {
			curOptionIndex++;
		}
	}

}
