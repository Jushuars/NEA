using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public NavMeshAgent agent;
    public Transform player;
    private Transform playerPos;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject gun1;
    public GameObject gun2;
    public GameObject projectile;
    public GameObject gunTip1;
    public GameObject gunTip2;
    public RangedStunned RS;
    public Player pl;
    public Enemy enemy;
    private float xRotation;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    private bool attacked;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public bool canBeFinished, finisherActive;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        playerPos = GameObject.Find("PlayerPos").transform;
    }
    private void Update() 
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Sets the AI's state
        if(playerInSightRange && playerInAttackRange && !RS.stunned)
            AttackPlayer();

        // Allows Enemy to be finished off if parried
        if (RS.stunned & !finisherActive)
            canBeFinished = true;
        else
            canBeFinished = false;
    }

    private void AttackPlayer()
    {
        transform.LookAt(playerPos);

        if (!attacked) // Attacks if ready to attack
        {
            attacked = true;

            // Attack Animation
            Animator anim1 = gun1.GetComponent<Animator>();
            anim1.SetTrigger("In Range"); 
            Animator anim2 = gun2.GetComponent<Animator>();
            anim2.SetTrigger("In Range");

            // Projectie Attack
            Invoke(nameof(FireProjectile), 2.9f);
        }
    }
    public void ResetAttack()
    {
        attacked = false;
    }

    private void FireProjectile() // Creates the projectile & fires it
    {
        Rigidbody rb1 = Instantiate(projectile, gunTip1.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb1.AddForce(transform.forward * 16f, ForceMode.Impulse);
        Rigidbody rb2 = Instantiate(projectile, gunTip2.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb2.AddForce(transform.forward * 16f, ForceMode.Impulse);
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }
    public void FinisherKill()
    {
        Debug.Log("Ranged Finished");
        if (!canBeFinished) return;
        canBeFinished = false;
        finisherActive = true;
        RS.DisabledAnimation1();
        Animator self = GetComponent<Animator>();
        Invoke(nameof(EndFinisher), .25f);
    }
    private void EndFinisher()
    {
        enemy.health = 0;
        pl.health += 50;
    }
}
