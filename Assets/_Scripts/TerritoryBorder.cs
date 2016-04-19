using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerritoryBorder : MonoBehaviour {
	public Player owningPlayer;
	Image territoryImage;
	ShipMovement thisPlayer;

	float maxAlpha = 0.2f;
	float threshold;

	// Use this for initialization
	IEnumerator Start () {
		territoryImage = GetComponent<Image>();
		yield return null;
		thisPlayer = GameManager.S.players[(int)owningPlayer].playerMovement;

		if (owningPlayer == Player.player1) {
			threshold = thisPlayer.worldSpaceMaxX - GameManager.S.players[1].playerMovement.worldSpaceMinX;
		}
		else if (owningPlayer == Player.player2) {
			threshold = GameManager.S.players[0].playerMovement.worldSpaceMaxX - thisPlayer.worldSpaceMinX;
		}

		while (thisPlayer != null){
			if (owningPlayer == Player.player1) {
				float shiftedPos = thisPlayer.transform.position.x - (thisPlayer.worldSpaceMaxX - threshold);
				if (shiftedPos > 0) {
					Color curColor = territoryImage.color;
					curColor.a = maxAlpha * shiftedPos / threshold;
					territoryImage.color = curColor;
				}
				else if (territoryImage.color.a != 0) {
					Color curColor = territoryImage.color;
					curColor.a = 0;
					territoryImage.color = curColor;
				}
	        }
			else if (owningPlayer == Player.player2) {
				float shiftedPos = (thisPlayer.worldSpaceMinX + threshold) - thisPlayer.transform.position.x;
				if (shiftedPos > 0) {
					Color curColor = territoryImage.color;
					curColor.a = maxAlpha * shiftedPos / threshold;
					territoryImage.color = curColor;
				}
				else if (territoryImage.color.a != 0) {
					Color curColor = territoryImage.color;
					curColor.a = 0;
					territoryImage.color = curColor;
				}
			}
			yield return null;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
