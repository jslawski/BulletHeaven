using UnityEngine;
using System.Collections;

public class HomingGroupShot : MonoBehaviour, BombAttack {
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			PlayerEnum otherPlayer = (value == PlayerEnum.player1) ? PlayerEnum.player2 : PlayerEnum.player1;
			if (GameManager.S.inGame) {
				target = GameManager.S.players[(int)otherPlayer].character.transform;
			}
		}
	}
	PlayerEnum _owningPlayer = PlayerEnum.none;
	HomingGroup homingGroupPrefab;
	public Player thisPlayer;
	public Transform target;

	int numShots = 3;
	float timeBetweenShots = 0.5f;

	public void FireBurst() {
		//This does nothing to appease the interface
	}

	// Use this for initialization
	IEnumerator Start () {
		homingGroupPrefab = Resources.Load<HomingGroup>("Prefabs/HomingGroup");

		for (int i = 0; i < numShots; i++) {
			HomingGroup newGroup = Instantiate(homingGroupPrefab, transform.position, new Quaternion()) as HomingGroup;
			newGroup.owningPlayer = owningPlayer;
			newGroup.target = target;
			if (!GameManager.S.inGame) {
				newGroup.thisPlayer = thisPlayer;
			}

			yield return new WaitForSeconds(timeBetweenShots);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
