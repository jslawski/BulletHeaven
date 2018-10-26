using UnityEngine;
using System.Collections;

public class PreviewShipMovement : ShipMovement {
	[SerializeField]
	bool _autoMove = false;
	public bool autoMove {
		get {
			return _autoMove;
		}
		set {
			_autoMove = value;
			desiredPosition = startPos;
			desiredRotation = startRotation;
			if (value) {
				timeSinceDirectionChange = reverseDirectionTime / 2f;
			}
		}
	}
	int curDirection = 1;
	float reverseDirectionTime = 1;
	float timeSinceDirectionChange = .5f;
	public float speedMultiplier = 1f;

	Vector3 startPos;

	// Use this for initialization
	protected override void Awake() {
		PreviewCharacter thisPreviewCharacter = GetComponentInParent<PreviewCharacter>();
		thisCharacter = thisPreviewCharacter;
		renderCamera = thisPreviewCharacter.previewPlayer.previewGameManager.previewCamera;

		startPos = transform.position;

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
		//print("Bottom left: " + worldSpaceMin + "\nTop right: " + worldSpaceMax);

		desiredPosition = transform.position;
		startRotation = transform.rotation;
		desiredRotation = startRotation;
		dotVector = -transform.right;
	}

	protected void Start() {
		vertMovespeedDefault *= speedMultiplier;
		horizMovespeedDefault *= speedMultiplier;

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
	}

	protected override void Update() {
		if (movementDisabled || GameManager.S.gameState != GameStates.shipSelect) {
			desiredRotation = startRotation;
			return;
		}


		if (autoMove) {
			timeSinceDirectionChange += Time.deltaTime;
			if (timeSinceDirectionChange > reverseDirectionTime) {
				curDirection *= -1;
				timeSinceDirectionChange = 0;
			}

			Move(Vector3.up * verticalMovespeed * Time.deltaTime * curDirection);
		}
	}
}
