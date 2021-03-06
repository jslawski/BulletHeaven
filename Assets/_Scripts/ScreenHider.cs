﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenHider : MonoBehaviour {
	Image fadePanel;


	// Use this for initialization
	void Start () {
		fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
		fadePanel.enabled = true;
		Invoke("RevealScreen", 1f);
	}
	
	void RevealScreen() {
		Color curColor = fadePanel.color;
		curColor.a = 0;
		fadePanel.color = curColor;
		Countdown.S.BeginCountdown();
	}
}
