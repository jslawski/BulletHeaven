using UnityEngine;
using System.Collections;

public class EasterEgg : MonoBehaviour {
	string currentlyTyped;
	readonly string keyword = "neat";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.inputString != "") {
			currentlyTyped += Input.inputString;

			int compVal = StrComp(currentlyTyped, keyword);
			//If the keyword has been typed, toggle the easter egg
			if (compVal == -1) {
				currentlyTyped = "";
				SoundManager.instance.easterEgg = !SoundManager.instance.easterEgg;
				if (SoundManager.instance.easterEgg) {
					print("Easter egg activated. It's pretty neat.");
				}
				else {
					print("Easter egg deactivated. It's less neat now.");
				}
			}
			else if (compVal != -2) {
				currentlyTyped = "";
			}
		}
	}

	//Returns the index of the first difference, or -1 if the strings match, or -2 if str1 matches str2 up until it's end
	int StrComp(string str1, string str2) {
		for (int i = 0; i < str2.Length; i++) {
			if (i == str1.Length) {
				return -2;
			}
			else if (str1[i] != str2[i]) {
				return i;
			}

		}

		return -1;
	}
}
