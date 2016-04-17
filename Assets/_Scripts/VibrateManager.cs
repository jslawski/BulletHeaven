using UnityEngine;
using System.Collections;
using InControl;

public class VibrateManager : MonoBehaviour {
	public static VibrateManager S;
	public bool DEBUG_MODE = false;
	float[] vibrations;

	public void Awake() {
		S = this;
	}

	void Start() {
		vibrations = new float[GameManager.S.players.Length];
	}

	void OnDestroy() {
		if (GameManager.S.players[0].device != null) {
			GameManager.S.players[0].device.Vibrate(0);
		}

		if (GameManager.S.players[1].device != null) {
			GameManager.S.players[1].device.Vibrate(0);
		}
	}
	
	public void RumbleVibrate(Player player, float duration, float intensity, bool stack=true) {
		PlayerShip curPlayer = GameManager.S.players[(int)player];
		//Ignore vibrations until the player has a controller plugged in
		if (curPlayer.device == null) {
			return;
		}
		StartCoroutine(RumbleVibrateCoroutine(curPlayer, duration, intensity, stack));
	}

	//Disable the vibrating after a specific duration
	IEnumerator RumbleVibrateCoroutine(PlayerShip player, float duration, float intensity, bool stack) {
		float timeElapsed = 0;
		int index = (int)player.player;
		bool hasTakenEffect = false;

		//If the vibration stacks, add it to the total vibration value
		if (stack) {
			//Increase the player's vibration by intensity value
			vibrations[index] += intensity;
			player.device.Vibrate(Mathf.Min(1, vibrations[index]));
			hasTakenEffect = true;
		}
		else {
			//While we are waiting for the non-stacking vibration to take place
			while (!hasTakenEffect && timeElapsed < duration) {
				//If it doesn't stack, only increase the player's vibration if there is no other vibration happening at the time
				//0.01 used to account for floating point inaccuracy
				if (vibrations[index] < 0.01f) {
					//Increase the player's vibration by intensity
					vibrations[index] += intensity;
					player.device.Vibrate(Mathf.Min(1, vibrations[index]));
					hasTakenEffect = true;
					break;
				}

				timeElapsed += Time.deltaTime;
				yield return null;
			}
		}
		//If we waited out for longer than the duration of the vibration, just give up and return
		if (!hasTakenEffect) {
			yield break;
		}

		if (DEBUG_MODE) {
			print("Time left on vibration: " + (duration - timeElapsed) + "\nCurrent intensity: " + vibrations[index]);
		}
		//If the vibration took effect, wait the remaining amount of time
		yield return new WaitForSeconds(duration - timeElapsed);

		//Decrease the player's vibration by intensity value
		vibrations[index] -= intensity;
		player.device.Vibrate(Mathf.Min(1, vibrations[index]));
	}
}
