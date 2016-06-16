using UnityEngine;
using System.Collections;

public class HomingGroupShot : MonoBehaviour {
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			Player otherPlayer = (value == Player.player1) ? Player.player2 : Player.player1;
			target = GameManager.S.players[(int)otherPlayer].transform;
		}
	}
	Player _owningPlayer = Player.none;
	HomingGroup homingGroupPrefab;
	Transform target;

	int numShots = 3;
	float timeBetweenShots = 0.5f;

	// Use this for initialization
	IEnumerator Start () {
		homingGroupPrefab = Resources.Load<HomingGroup>("Prefabs/HomingGroup");

		for (int i = 0; i < numShots; i++) {
			HomingGroup newGroup = Instantiate(homingGroupPrefab, transform.position, new Quaternion()) as HomingGroup;
			newGroup.owningPlayer = owningPlayer;
			newGroup.target = target;
			yield return new WaitForSeconds(timeBetweenShots);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
