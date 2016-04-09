using UnityEngine;
using System.Collections;

public class PooledObj : MonoBehaviour {
	public ObjectPool Pool { get; set; }

	[System.NonSerialized]
	ObjectPool poolInstanceForPrefab;

	public void ReturnToPool() {
		if (Pool) {
			Pool.ReturnObject(this);
		}
		else {
			Destroy(gameObject);
		}
	}

	public T GetPooledInstance<T>() where T : PooledObj {
		if (!poolInstanceForPrefab) {
			poolInstanceForPrefab = ObjectPool.GetPool(this);
		}
		return (T)poolInstanceForPrefab.GetObject();
	}
	public T GetPooledInstance<T>(Vector3 position) where T : PooledObj {
		if (!poolInstanceForPrefab) {
			poolInstanceForPrefab = ObjectPool.GetPool(this);
		}
		return (T)poolInstanceForPrefab.GetObject(position);
	}
}
