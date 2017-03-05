using UnityEngine;
using System.Collections;

public class ClusterBomb : MonoBehaviour, BombAttack {
	PlayerEnum _owningPlayer = PlayerEnum.none;

	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	ProximityMine minePrefab;

	float mainExplosionRadius = 3.9f;
	float mainExplosionBaseDamage = 30f;
	float minDamageFalloff = 0.5f;

	int minNumBombs = 6;
	int maxNumBombs = 9;
	float minDetonationTime = 0.25f;
	float maxDetonationTime = 0.5f;

	void Awake() {
		minePrefab = Resources.Load<ProximityMine>("Prefabs/ProximityMine");
	}

	public void FireBurst() {
		//This does nothing to appease the interface
	}

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
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("Explosion");
		}
		Collider[] allObjectsHit = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (var obj in allObjectsHit) {
			if (obj.gameObject.tag == "Player") {
				PlayerShip playerHit = obj.gameObject.GetComponentInParent<PlayerShip>();
				if (playerHit.playerEnum == owningPlayer) {
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

		int numMinesSpawned = Random.Range(minNumBombs, maxNumBombs);
		for (int i = 0; i < numMinesSpawned; i++) {
			ProximityMine newMine = Instantiate(minePrefab, transform.position, new Quaternion()) as ProximityMine;
			newMine.owningPlayer = owningPlayer;
		}
	}
}
