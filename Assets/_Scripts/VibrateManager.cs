using UnityEngine;
using System.Collections;
using InControl;

public class VibrateManager : MonoBehaviour {

	public static VibrateManager S;
	float beamVibrate = 0;                //Current magnitude of vibration for a beam.  0 if no beam. beamVibrate_c if beam is present
	float beamVibrate_c = 0.15f;                        //Max magnitude of vibration during a beam spawn

	float hitVibrate1 = 0;                //Magnitude of vibration when taking damage player1
	float hitVibrate2 = 0;                //Magnitude of vibration when taking damage player2
	float hitVibrate_c = 1f;							//Max magnitude of vibration when a player is hit.

	public void Awake() {
		S = this;
	}

	void FixedUpdate() {
		if (GameManager.S.players[0].device != null) {
			float currentIntensity = beamVibrate + hitVibrate1;
			//Clamp intensity to 1
			if (currentIntensity > 1) {
				currentIntensity = 1;
			}
			GameManager.S.players[0].device.Vibrate(currentIntensity);
		}

		if (GameManager.S.players[1].device != null) {
			float currentIntensity = beamVibrate + hitVibrate2;
			//Clamp intensity to 1
			if (currentIntensity > 1) {
				currentIntensity = 1;
			}
			GameManager.S.players[1].device.Vibrate(currentIntensity);
		}
	}

	public void BeamVibrate() {
		StartCoroutine(BeamVibrateCoroutine(3f)); //JPS: FIX THIS TO NOT JUST USE 3...
	}

	public IEnumerator BeamVibrateCoroutine(float duration) {
		for (float i = 0; i < duration; i += 0.02f) {
			beamVibrate = beamVibrate_c;
			yield return new WaitForFixedUpdate();
		}
		
		beamVibrate = 0;
	}

	public void HitVibrate(Player hitPlayer) {
		StartCoroutine(HitVibrateCoroutine(hitPlayer, 0.2f)); //JPS: FIX THIS TO NOT JUST USE 0.5...
	}

	public IEnumerator HitVibrateCoroutine(Player hitPlayer, float duration) {
		for (float i = 0; i < duration; i += 0.02f) {
			if (hitPlayer == Player.player1) {
				hitVibrate1 = hitVibrate_c;
			}
			else if (hitPlayer == Player.player2) {
				hitVibrate2 = hitVibrate_c;
			}
			yield return new WaitForFixedUpdate();
		}

		if (hitPlayer == Player.player1) {
			hitVibrate1 = 0;
		}
		else if (hitPlayer == Player.player2) {
			hitVibrate2 = 0;
		}
	}
}
