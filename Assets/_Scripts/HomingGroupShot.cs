using UnityEngine;
using System.Collections;

public class HomingGroupShot : MonoBehaviour, BombAttack {
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			Player otherPlayer = (value == Player.player1) ? Player.player2 : Player.player1;
			if (GameManager.S.inGame) {
				target = GameManager.S.players[(int)otherPlayer].transform;
			}
		}
	}
	Player _owningPlayer = Player.none;
	HomingGroup homingGroupPrefab;
	public PlayerShip thisPlayer;
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
