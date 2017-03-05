using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerritoryBorder : MonoBehaviour {
	public PlayerEnum owningPlayer;
	Image territoryImage;
	ShipMovement shipMovement;

	float maxAlpha = 0.2f;
	float threshold;

	// Use this for initialization
	IEnumerator Start () {
		territoryImage = GetComponent<Image>();
		while (!GameManager.S.shipsReady) {
			yield return null;
		}
		shipMovement = GameManager.S.players[(int)owningPlayer].ship.movement;

		yield return new WaitForSeconds(0.1f);
		Color startColor = shipMovement.thisPlayerShip.player.playerColor;
		startColor.a = 0;
		territoryImage.color = startColor;

		if (owningPlayer == PlayerEnum.player1) {
			threshold = shipMovement.worldSpaceMaxX - GameManager.S.players[1].ship.movement.worldSpaceMinX;
		}
		else if (owningPlayer == PlayerEnum.player2) {
			threshold = GameManager.S.players[0].ship.movement.worldSpaceMaxX - shipMovement.worldSpaceMinX;
		}

		while (shipMovement != null && GameManager.S.gameState != GameStates.finalAttack){
			if (owningPlayer == PlayerEnum.player1) {
				float shiftedPos = shipMovement.transform.position.x - (shipMovement.worldSpaceMaxX - threshold);
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
			else if (owningPlayer == PlayerEnum.player2) {
				float shiftedPos = (shipMovement.worldSpaceMinX + threshold) - shipMovement.transform.position.x;
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

		territoryImage.enabled = false;
	}
}
