using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Barricade attackPoint;

    [Header("Health")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth = 0;
    [Space]
    [SerializeField] int maxDamage = 15;
    [SerializeField] float maxSpeed = 1f;

    [Header("Prototype")]
    [SerializeField] PlayerController target = null;
    [SerializeField] Zone currentZone = null;

    NavMeshAgent zombieAgent;
    bool activeOnMap = false;

    void Awake(){
        zombieAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        zombieAgent.speed = maxSpeed;
    	currentHealth = maxHealth;

        if(attackPoint == null){
            // player hunt
            activeOnMap = true;
        }else{
            // barricade hunt
        }
    }

    void Update() {
        if(activeOnMap && target != null){
            zombieAgent.destination = target.transform.position;
        }    

        // check if zone is in zone list, if not check range from player
    }

    public bool TryKill(int damage){
        TakeDamage(damage);
        if(currentHealth == 0){
            KillZombie();
            return true;
        }else{
            return false;
        }
    }

    public void TakeDamage(int damage){
        if(currentHealth - damage <= 0){
            currentHealth = 0;
        }else{
            currentHealth -= damage;
        }   
    }

    public void KillZombie(){
        RoundManager.Instance.UpdateZombiesRemaining(this);
        ObjectPool.SharedInstance.ReturnPooledObject(gameObject);
    }

    public void CreateNewZombie(int health, int damage, float speed, PlayerController _target = null){
        maxHealth = health;
        maxDamage = damage;
        maxSpeed = speed;
        target = _target;

        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Zone>()){
            currentZone = other.GetComponent<Zone>();
        }
    }
}
