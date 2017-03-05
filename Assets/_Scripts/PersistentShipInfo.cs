using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using InControl;

public class PersistentShipInfo : MonoBehaviour {
	ShipInfo shipInfo;
	InputDevice device;

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		if (shipInfo == null) {
			Debug.LogError("ShipInfo not intialized before sending to GameManager.InitializePlayerShip()");
			return;
		}
		GameManager.S.InitializePlayerShip(shipInfo, device);
	}

	public void Initialize(ShipInfo shipInfo, InputDevice device) {
		this.shipInfo = shipInfo;
		this.device = device;
	}

	IEnumerator OnLevelWasLoaded(int levelIndex) {
		if (SceneManager.GetActiveScene().name != "_Scene_Main") {
			Destroy(gameObject);
		}
		else {
			//Wait a couple of frames for GameManager to initialize
			yield return null;
			yield return null;

			//Wait an additional frame to intialize player2 to guarantee order
			if (shipInfo.selectingPlayer == PlayerEnum.player2) {
				yield return null;
			}
			GameManager.S.InitializePlayerShip(shipInfo, device);
		}
	}

}
