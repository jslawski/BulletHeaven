using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TerritoryBorder : MonoBehaviour {
	public PlayerEnum owningPlayer;
	Character owningCharacter;
	Image territoryImage;
	Ship closestShip;

	float borderWorldSpacePosition;
	float maxAlpha = 0.2f;
	float threshold;

	// Use this for initialization
	IEnumerator Start () {
		territoryImage = GetComponent<Image>();
		while (!GameManager.S.shipsReady) {
			yield return null;
		}
		owningCharacter = GameManager.S.players[(int)owningPlayer].character;

		yield return new WaitForSeconds(0.1f);
		Color startColor = owningCharacter.player.playerColor;
		startColor.a = 0;
		territoryImage.color = startColor;

		if (owningPlayer == PlayerEnum.player1) {
			borderWorldSpacePosition = owningCharacter.player.worldSpaceMaxX;
			threshold = borderWorldSpacePosition - GameManager.S.players[1].character.player.worldSpaceMinX;
		}
		else if (owningPlayer == PlayerEnum.player2) {
			borderWorldSpacePosition = owningCharacter.player.worldSpaceMinX;
			threshold = GameManager.S.players[0].character.player.worldSpaceMaxX - borderWorldSpacePosition;
		}

		closestShip = FindClosestShipToBorder();

		while (closestShip != null && GameManager.S.gameState != GameStates.finalAttack){
			if (owningPlayer == PlayerEnum.player1) {
				float shiftedPos = closestShip.transform.position.x - (borderWorldSpacePosition - threshold);
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
				float shiftedPos = (borderWorldSpacePosition + threshold) - closestShip.transform.position.x;
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

	Ship FindClosestShipToBorder() {
		float minDistance = float.MaxValue;
		Ship closestShip = null;
		foreach (Ship ship in owningCharacter.ships) {
			float distanceToBorder = Mathf.Abs(ship.transform.position.x - borderWorldSpacePosition);
			if (distanceToBorder < minDistance) {
				minDistance = distanceToBorder;
				closestShip = ship;
			}
		}

		return closestShip;
	}
}
