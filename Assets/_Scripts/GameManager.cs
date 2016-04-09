using UnityEngine;
using System.Collections;

public enum Player {
	player1,
	player2,
	none
}

public class GameManager : MonoBehaviour {
	public static GameManager S;

	public PlayerShip[] players;

	void Awake() {
		S = this;
		players = new PlayerShip[2];
		players[0] = GameObject.Find("Player1").GetComponent<PlayerShip>();
		players[1] = GameObject.Find("Player2").GetComponent<PlayerShip>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
