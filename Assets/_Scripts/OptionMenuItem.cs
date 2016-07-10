using UnityEngine;
using System.Collections;

public class OptionMenuItem : MonoBehaviour {
	public bool selected = false;

	public virtual void SetOptionValue() {
		Debug.LogError("Pure virtual function OptionMenuItem.SetOptionValue() called");
	}
	public virtual void SetOptionValue(float value) {
		Debug.LogError("Pure virtual function OptionMenuItem.SetOptionValue() called");
	}
	public virtual void SetOptionValue(int value) {
		Debug.LogError("Pure virtual function OptionMenuItem.SetOptionValue() called");
	}
	public virtual void SetOptionValue(string value) {
		Debug.LogError("Pure virtual function OptionMenuItem.SetOptionValue() called");
	}

	public virtual void IncreaseOptionValue() {
		Debug.LogError("Pure virtual function OptionMenuItem.IncreaseOptionValue() called");
	}
	public virtual void DecreaseOptionValue() {
		Debug.LogError("Pure virtual function OptionMenuItem.DecreaseOptionValue() called");
	}
	public virtual void PlayHighlightedAnimation() {
		Debug.LogError("Pure virtual function OptionMenuItem.PlayHighlightedAnimation() called");
	}
}
