using UnityEngine;
using System.Collections;

public class SingletonInControl : MonoBehaviour {
	public static SingletonInControl S;

	// Use this for initialization
	void Awake () {
		if (S != null){
			Destroy(gameObject);
		}
		S = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
