using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {
	PooledObj prefab;   //Used for instantiating new instances when the pool is empty

	Stack<PooledObj> availableObjects = new Stack<PooledObj>();

	public PooledObj GetObject() {
		PooledObj obj;
		//If we have an object available, return that
		if (availableObjects.Count > 0) {
			obj = availableObjects.Pop();
			obj.gameObject.SetActive(true);
		}
		//Else create a new one to return
		else {
			obj = Instantiate(prefab);
			obj.transform.SetParent(transform, false);
			obj.Pool = this;
		}
		return obj;
	}
	public PooledObj GetObject(Vector3 moveToPosition) {
		PooledObj obj;
		//If we have an object available, return that
		if (availableObjects.Count > 0) {
			obj = availableObjects.Pop();
			obj.transform.position = moveToPosition;
			obj.gameObject.SetActive(true);
		}
		//Else create a new one to return
		else {
			obj = Instantiate(prefab);
			obj.transform.SetParent(transform, false);
			obj.transform.position = moveToPosition;
			obj.Pool = this;
		}
		return obj;
	}

	public void ReturnObject(PooledObj obj) {
		obj.gameObject.SetActive(false);
		availableObjects.Push(obj);
	}

	public static ObjectPool GetPool(PooledObj prefab) {
		GameObject obj;
		ObjectPool pool;
		if (Application.isEditor) {
			obj = GameObject.Find(prefab.name + " Pool");
			if (obj) {
				pool = obj.GetComponent<ObjectPool>();
				if (pool) {
					return pool;
				}
			}
		}
		obj = new GameObject(prefab.name + " Pool");
		pool = obj.AddComponent<ObjectPool>();
		pool.prefab = prefab;
		return pool;
	}
}
