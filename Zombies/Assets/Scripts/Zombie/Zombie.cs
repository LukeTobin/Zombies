using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    // STATEMACHINE
    public enum States{
        Spawning,
        Walking,
        Running,
        Attacking
    }

    [Header("References")]
    [SerializeField] Barricade attackPoint;
    [SerializeField] PlayerController target = null;
    [SerializeField] Zone currentZone = null;

    [Header("Stats")]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth = 0;
    [Space]
    [SerializeField] float maxDamage = 1.2f;
    [SerializeField] float walkSpeed = 1.2f; // speeds are hard coded for now
    [SerializeField] float runSpeed = 3.8f;
    [Space]
    [SerializeField] float waitToAttack = 0.7f;
    [SerializeField] bool idleStance = false;

    [Header("Prototype")]
    [SerializeField] float distanceFromTarget = 0;

    NavMeshAgent zombieAgent;
    ZombieAnimationController zombieAnimationController;

    bool activeOnMap = false;
    bool isRunner = false;

    float currentWaitTime = 0;
    float animWaitTime = 0;
    bool playAttackAnim = false;

    void Awake(){
        zombieAgent = GetComponent<NavMeshAgent>();
        zombieAnimationController = GetComponent<ZombieAnimationController>();
    }

    void Start()
    {
        zombieAgent.speed = walkSpeed;
    	currentHealth = maxHealth;

        if(idleStance)
            zombieAnimationController.SetIdleAnimation();

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
            if(distanceFromTarget > 1.55f && animWaitTime <= 0){
                zombieAgent.destination = target.transform.position;
                if(currentWaitTime != waitToAttack)
                    currentWaitTime = waitToAttack;
            }else if(currentWaitTime <= 0){
                TryAttack();
                playAttackAnim = false;
            }else if(!playAttackAnim && currentWaitTime > 0 && animWaitTime <= 0){
                playAttackAnim = true;
                zombieAnimationController.PlayAttackAnimation();
                animWaitTime = zombieAnimationController.GetCurrentClipLength();
            }else if(playAttackAnim){
                currentWaitTime -= Time.deltaTime;
            }
        }

        if(animWaitTime > 0){
            animWaitTime -= Time.deltaTime;
        }
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
        ObjectPool.SharedInstance.ReturnPooledObject(gameObject, ObjectPool.ObjectType.Zombie);
    }

    // Create a new zombie with specific stats
    public void CreateNewZombie(int health, float damage, bool _isRunner, PlayerController _target = null){
        maxHealth = health;
        maxDamage = damage;
        isRunner = _isRunner;
        if(!_isRunner)
            zombieAgent.speed = walkSpeed;
        else
            zombieAgent.speed = runSpeed;
        zombieAnimationController.UpdateMovementSpeed(_isRunner);
        target = _target;
        currentHealth = maxHealth;
    }

    private void TryAttack(){
        if(distanceFromTarget < 1.5f){
            float dmg = maxDamage - Random.Range(0, 0.6f);
            print("Attacked Player");
            target.TakeDamage(dmg);

            currentWaitTime = waitToAttack;
        }
    }

    // Update active zones
    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Zone>()){
            currentZone = other.GetComponent<Zone>();
        }
    }
}
