using UnityEngine;
using System.Collections;

public class ShipSelectStartPrompt : MonoBehaviour {
	float movementLerpSpeed = 8.75f;

	RectTransform rect;
	Vector2 startAnchorMin, endAnchorMin,
			startAnchorMax, endAnchorMax;

	// Use this for initialization
	IEnumerator Start () {
		rect = GetComponent<RectTransform>();
        startAnchorMin = rect.anchorMin;
		startAnchorMax = rect.anchorMax;

		endAnchorMin = new Vector2(0, startAnchorMin.y);
		endAnchorMax = new Vector2(1, startAnchorMax.y);

		yield return new WaitForSeconds(0.25f);
		while (true) {
			float t = Time.deltaTime * movementLerpSpeed;

			//Move prompt off-screen right when we move to the main scene
			if (GameManager.S.gameState == GameStates.transitioning) {
				rect.anchorMin = Vector2.Lerp(rect.anchorMin, new Vector2(0.52f, startAnchorMin.y), t);
				rect.anchorMax = Vector2.Lerp(rect.anchorMax, new Vector2(0.48f, startAnchorMax.y), t);
			}
			else {
				//Move prompt on-screen if all players are ready
				if (ShipSelectionManager.AllPlayersReady()) {
					rect.anchorMin = Vector2.Lerp(rect.anchorMin, endAnchorMin, t);
					rect.anchorMax = Vector2.Lerp(rect.anchorMax, endAnchorMax, t);
				}
				//Move prompt off-screen left if players aren't ready
				else {
					rect.anchorMin = Vector2.Lerp(rect.anchorMin, startAnchorMin, t);
					rect.anchorMax = Vector2.Lerp(rect.anchorMax, startAnchorMax, t);
				}
			}

			yield return null;
		}
	}
}
