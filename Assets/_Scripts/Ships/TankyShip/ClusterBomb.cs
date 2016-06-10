using UnityEngine;
using System.Collections;

public class ClusterBomb : MonoBehaviour {
	public Player owningPlayer;

	float mainExplosionRadius = 3.9f;
	float mainExplosionBaseDamage = 30f;
	float minDamageFalloff = 0.5f;

	int minNumBombs = 3;
	int maxNumBombs = 5;
	float minDetonationTime = 0.25f;
	float maxDetonationTime = 0.5f;

	// Use this for initialization
	void Start () {
		Explode(mainExplosionBaseDamage, mainExplosionRadius);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	float CalculateDamageDealt(Transform victim, float baseDamage, float explosionRadius) {
		//Explosion deals more damage closer to the center
		return Mathf.Lerp(baseDamage*minDamageFalloff, baseDamage, ((transform.position - victim.position).magnitude) / explosionRadius);
	}

	void Explode(float explosionDamage, float explosionRadius) {
		Collider[] allObjectsHit = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (var obj in allObjectsHit) {
			if (obj.gameObject.tag == "Player") {
				PlayerShip playerHit = obj.gameObject.GetComponentInParent<PlayerShip>();
				if (playerHit.player == owningPlayer) {
					return;
				}
				else {
					playerHit.TakeDamage(CalculateDamageDealt(obj.transform, explosionDamage, explosionRadius));
				}
			}
			else if (obj.gameObject.tag == "ProtagShip") {
				ProtagShip shipHit = obj.gameObject.GetComponentInParent<ProtagShip>();
				shipHit.TakeDamage(CalculateDamageDealt(obj.transform, explosionDamage, explosionRadius));
			}
		}
	}
}
