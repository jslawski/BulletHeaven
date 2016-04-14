using UnityEngine;
using System.Collections;
using InControl;

public class VibrateManager : MonoBehaviour {

	public static VibrateManager S;

	public void Awake() {
		S = this;
	}

	void OnDestroy() {
		if (GameManager.S.players[0].device != null) {
			GameManager.S.players[0].device.Vibrate(0);
		}

		if (GameManager.S.players[1].device != null) {
			GameManager.S.players[1].device.Vibrate(0);
		}
	}
	
	public bool RumbleVibrate(Player player, float intensity, float duration, bool stack=false) {
		PlayerShip curPlayer = GameManager.S.players[(int)player];

		//Only increment the intensity if the vibration given is stackable, or the controller is currently not vibrating
		if (stack == true || curPlayer.vibrateIntensity == 0) {
			//Increment vibration intensity
			curPlayer.realVibrateIntensity += intensity;
			curPlayer.vibrateIntensity += intensity;

			StartCoroutine(RumbleVibrateCoroutine(curPlayer, intensity, duration));
			return true;
		}
		else {
			curPlayer.realVibrateIntensity += intensity;
			StartCoroutine(RumbleVibrateCoroutine(curPlayer, intensity, duration));
			return false;
		}
	}

	//Disable the vibrating after a specific duration
	IEnumerator RumbleVibrateCoroutine(PlayerShip player, float intensity, float duration) {
		yield return new WaitForSeconds(duration);
		//Continuously set the vibration
		//for (float i = 0; i < duration; i += Time.fixedDeltaTime) {
		//	player.vibrateIntensity = player.realVibrateIntensity;
		//	yield return new WaitForFixedUpdate();
		//}

		player.realVibrateIntensity -= intensity;
		player.vibrateIntensity = Mathf.Abs(player.realVibrateIntensity);
	}
}
