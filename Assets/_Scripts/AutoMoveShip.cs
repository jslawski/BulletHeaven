using UnityEngine;
using System.Collections;

public class AutoMoveShip : MonoBehaviour {

	PlayerShip thisPlayer;
	Transform thisTransform;

	Vector3 curDirection;
	float speed = 0.2f;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<PlayerShip>();
		thisTransform = GetComponent<Transform>();

		curDirection = (thisPlayer.player == Player.player1) ? Vector3.up : Vector3.down;

		StartCoroutine(ChangeDirection());
	}
	
	IEnumerator ChangeDirection() {
		yield return new WaitForSeconds(1.7f);
		curDirection *= -1;
		StartCoroutine(ChangeDirection());
	}

	void FixedUpdate() {
		thisTransform.Translate(curDirection * speed, Space.World);
	}
}
