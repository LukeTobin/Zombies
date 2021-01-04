using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Barricade attackPoint;

    [Header("Stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth = 0;
    [Space]
    [SerializeField] float maxDamage = 1.2f;
    [SerializeField] float maxSpeed = 1f;
    [Space]
    [SerializeField] float waitToAttack = 0.7f;

    [Header("Prototype")]
    [SerializeField] PlayerController target = null;
    [SerializeField] Zone currentZone = null;
    [SerializeField] float distanceFromTarget = 0;

    NavMeshAgent zombieAgent;
    bool activeOnMap = false;

    float currentWaitTime = 0;

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
            distanceFromTarget = Vector3.Distance(transform.position, target.transform.position);
            if(distanceFromTarget > 1.5f){
                zombieAgent.destination = target.transform.position;
                if(currentWaitTime != waitToAttack)
                    currentWaitTime = waitToAttack;
            }else if(currentWaitTime <= 0){
                TryAttack();
            }else{
                currentWaitTime -= Time.deltaTime;
            }
            
        }    

        // check if zone is in zone list, if not check range from player
    }

    // Attempt to kill zombie and register damage
    public bool TryKill(int damage){
        TakeDamage(damage);
        if(currentHealth == 0){
            KillZombie();
            return true;
        }else{
            return false;
        }
    }

    // Register damage to zombie
    public void TakeDamage(int damage){
        if(currentHealth - damage <= 0){
            currentHealth = 0;
        }else{
            currentHealth -= damage;
        }   
    }

    // Kill zombie & Update zombies remaining & add the zombie back into the object pool
    public void KillZombie(){
        RoundManager.Instance.UpdateZombiesRemaining(this);
        ObjectPool.SharedInstance.ReturnPooledObject(gameObject);
    }

    // Create a new zombie with specific stats
    public void CreateNewZombie(int health, float damage, float speed, PlayerController _target = null){
        maxHealth = health;
        maxDamage = damage;
        maxSpeed = speed;
        target = _target;

        currentHealth = maxHealth;
    }

    private void TryAttack(){
        if(distanceFromTarget < 1.5f){
            float dmg = maxDamage - Random.Range(0, 0.6f);
            //Debug.Log("Attacking for " + dmg);
            target.TakeDamage(dmg);

            currentWaitTime = waitToAttack / 1.2f;
        }
    }

    // Update active zones
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Zone>()){
            currentZone = other.GetComponent<Zone>();
        }
    }
}
