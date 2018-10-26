using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlayer : Player {
	public PreviewGameManager previewGameManager;

	private PreviewCharacter previewCharacter {
		get { return character as PreviewCharacter; }
	}

	public override Character InstantiateCharacter(SelectedCharacterInfo characterInfo) {
		return previewCharacter.InitializeCharacter(characterInfo);
	}

	public override void Awake() {
		previewGameManager = GetComponentInParent<PreviewGameManager>();

		Quaternion spawnRotation = Quaternion.Euler((playerEnum == PlayerEnum.player1) ? Vector3.back * 90 : Vector3.forward * 90);
		Vector3 spawnPosition = (playerEnum == PlayerEnum.player1) ? Vector3.left * 18.75f : Vector3.right * 18.75f;
		character = Instantiate(Resources.Load<PreviewCharacter>("Prefabs/PreviewShip"), spawnPosition, spawnRotation, transform);

		Vector3 worldSpaceMax = previewGameManager.previewCamera.ViewportToWorldPoint(new Vector3(viewportMaxX, viewportMaxY, 0));
		worldSpaceMaxX = worldSpaceMax.x;
		worldSpaceMaxY = worldSpaceMax.y;
		Vector3 worldSpaceMin = previewGameManager.previewCamera.ViewportToWorldPoint(new Vector3(viewportMinX, viewportMinY, 0));
		worldSpaceMinX = worldSpaceMin.x;
		worldSpaceMinY = worldSpaceMin.y;
	}
}
