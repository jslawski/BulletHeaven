using UnityEngine;
using System.Collections;

public class AutoMoveShip : MonoBehaviour {

	Player thisPlayer;
	Transform thisTransform;

	Vector3 curDirection;
	float speed = 10f;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponentInParent<Player>();
		thisTransform = GetComponent<Transform>();

		curDirection = (thisPlayer.playerEnum == PlayerEnum.player1) ? Vector3.up : Vector3.down;

		StartCoroutine(ChangeDirection());
	}
	
	IEnumerator ChangeDirection() {
		yield return new WaitForSeconds(1.7f);
		curDirection *= -1;
		StartCoroutine(ChangeDirection());
	}

	void FixedUpdate() {
		thisTransform.Translate(curDirection * speed * Time.fixedDeltaTime, Space.World);
	}
}
