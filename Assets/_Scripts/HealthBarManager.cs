using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour {
	public PlayerEnum owningPlayer;
	private Character owningCharacter;

	private HealthBar healthBarPrefab;
	public List<HealthBar> healthBars = new List<HealthBar>();
	private int _numHealthBars;
	public int numHealthBars {
		set {
			_numHealthBars = value;
			healthBars.Clear();
			for (int i = 0; i < value; i++) {
				HealthBar newHealthbar = Instantiate(healthBarPrefab, transform, false);
				SetHealthBarRect(newHealthbar, i);

				healthBars.Add(newHealthbar);
			}

			border.SetAsLastSibling();
		}
		get {
			return _numHealthBars;
		}
	}

	private Transform border;

	private void Awake() {
		healthBarPrefab = Resources.Load<HealthBar>("Prefabs/HealthBar");
		border = transform.FindChild("Border");
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(SetupOwningPlayerValues());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator SetupOwningPlayerValues() {
		//Wait until the player ships are fully instantiated and set in the GameManager
		//JPS: I don't like that we have to check for the ShipType != none here.  I'd like to refactor the way we load the game so
		//     we don't populate the players array in the GameManager twice...
		while (GameManager.S.players[(int)this.owningPlayer] == null || GameManager.S.players[(int)this.owningPlayer].character == null) {
			yield return null;
		}

		this.owningCharacter = GameManager.S.players[(int)this.owningPlayer].character;
		//Create as many health bars as there are ships
		this.numHealthBars = owningCharacter.ships.Count;

		for (int i = 0; i < numHealthBars; i++) {
			//Setting the owningShip property assigns all values associated with that ship
			healthBars[i].owningShip = owningCharacter.ships[i];
		}
	}

	private void SetHealthBarRect(HealthBar hb, int index) {
		RectTransform newRect = hb.GetComponent<RectTransform>();
		newRect.anchorMax = new Vector2((index+1) * 1f/numHealthBars, 1);
		newRect.anchorMin = new Vector2(index * 1f/numHealthBars, 0);
	}
}
