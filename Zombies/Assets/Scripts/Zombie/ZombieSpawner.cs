using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] Barricade connectedBarricade = null;

    public void SpawnZombie(){
        RoundManager roundM = RoundManager.Instance;
        GameObject zombie = ObjectPool.SharedInstance.GetPooledObject(ObjectPool.ObjectType.Zombie);
        if(zombie != null){
            zombie.transform.position = transform.position;
            int z_health = 100 * (roundM.GetCurrentRound() / 2);
            bool speedState = RoundManager.Instance.GetZombieSpeedState();
            zombie.GetComponent<Zombie>().CreateNewZombie(z_health, 1.2f, speedState, GameManager.Instance.GetPlayer());
            roundM.AddZombieToPool(zombie.GetComponent<Zombie>());
            zombie.SetActive(true);
        }
    }
}
