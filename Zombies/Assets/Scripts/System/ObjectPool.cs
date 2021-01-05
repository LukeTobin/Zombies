using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;

    public enum ObjectType{
        Zombie,
        BloodFX
    }
    
    [SerializeField] GameObject zombieObject = null;
    [SerializeField] int amountToPool_zombie = 0;
    [Space]
    [SerializeField] GameObject bloodFX = null;
    [SerializeField] int amountToPool_blood = 0;
    [Space]
    [SerializeField] List<GameObject> zombiePool = null;
    [SerializeField] List<GameObject> bloodFXPool = null;
    
    GameObject zombieContainer = null;
    GameObject bloodParticleContainer = null;

    void Awake(){
        SharedInstance = this;
    }

    void Start(){
        CreateNewPool(zombieObject, amountToPool_zombie, zombiePool, zombieContainer, "Zombie Pool");
        CreateNewPool(bloodFX, amountToPool_blood, bloodFXPool, bloodParticleContainer, "Blood FX Pool");
    }

    // should be changed to T (Generic)
    void CreateNewPool(GameObject objectToPool, int amountToPool, List<GameObject> poolList, GameObject container, string containerName){
        container = new GameObject(containerName);
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(objectToPool, container.transform);
            obj.SetActive(false);
            poolList.Add(obj);
        }
    }

    public GameObject GetPooledObject(ObjectType objectType){
        if(objectType == ObjectType.Zombie){
            for (int i = 0; i < zombiePool.Count; i++)
            {
                if(!zombiePool[i].activeInHierarchy){
                    return zombiePool[i];
                }
            }
            return null;
        }
        else if(objectType == ObjectType.BloodFX){
            for (int i = 0; i < bloodFXPool.Count; i++)
            {
                if(!bloodFXPool[i].activeInHierarchy){
                    return bloodFXPool[i];
                }
            }
            return null;
        }

        return null;
    }

    // moves object back into the pool
    // Accessable by using Enum
    public void ReturnPooledObject(GameObject obj, ObjectType objectType){
        switch(objectType){
            case ObjectType.Zombie:
                obj.SetActive(false);
                break;
            case ObjectType.BloodFX:
                obj.SetActive(false);
                break;
            default:
                Debug.Log("Could not return Object");
                break;
        }
    }
}
