using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    GameObject objectPool;

    void Awake(){
        SharedInstance = this;
    }

    void Start(){
        objectPool = new GameObject("Object Pool");
        pooledObjects = new List<GameObject>();
        for(int i = 0;i < amountToPool;i++){
            GameObject obj = (GameObject)Instantiate(objectToPool, objectPool.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject(){
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy){
                return pooledObjects[i];
            }
        }
        return null;
    }

    // moves object back into the pool
    public void ReturnPooledObject(GameObject obj){
        obj.SetActive(false);
        pooledObjects.Add(obj);
        obj.transform.parent = objectPool.transform;
    }
}
