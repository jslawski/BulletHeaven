using UnityEngine;
using System.Collections;

public class ClusterBomb : MonoBehaviour {
	public Player owningPlayer;

	float mainExplosionRadius = 3.9f;
	float mainExplosionBaseDamage = 30f;

	int minNumBombs = 3;
	int maxNumBombs = 5;
	float minDetonationTime = 0.25f;
	float maxDetonationTime = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	float CalculateDamageDealt(Transform victim, float baseDamage, float explosionRadius) {
		//Normalize the distance to be a value between 0 (center of explosion) and 1 (edge of explosion)
		//Explosion deals more damage closer to the center, so a normalized value of 0 should yield the highest scalar of 1
		float damageScalar = Mathf.Abs(1 - ((transform.position - victim.position).magnitude) / explosionRadius);
		return baseDamage * damageScalar;
	}
}
