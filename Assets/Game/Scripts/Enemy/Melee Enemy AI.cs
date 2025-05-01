using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject sword;
    public BoxCollider hitbox;
    public Animator anim1;
    public Animator anim2;
    public MeleeStunned MS;
    public Player pl;
    public Enemy enemy;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    public float walkPointRange;
    public float patrolCd;
    public bool canPatrol;
    private bool walkPointSet;
    private bool resetWalkPoint;

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
    }
    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Sets the AI's state
        if (!playerInSightRange && !playerInAttackRange && !MS.parried && !finisherActive)
            Patrolling();
        if (playerInSightRange && !playerInAttackRange && !MS.parried && !finisherActive)
            ChasePlayer();
        if (playerInSightRange && playerInAttackRange && !MS.parried && !finisherActive)
            AttackPlayer();

        // Allows Enemy to be finished off if parried
        if (MS.parried & !finisherActive)
            canBeFinished = true;
        else
            canBeFinished = false;

        anim1 = sword.GetComponent<Animator>();
        anim2 = GetComponent<Animator>();

        if (MS.parried)
        {
            agent.SetDestination(transform.position);
        }

        // Incase hitbox enables while AI is stunned
        if (!attacked)
            hitbox.enabled = false;
    }
    private void Patrolling()
    {
        if (!walkPointSet) searchWalkPoint();

        // Calculates distance to walk
        if(walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f && canPatrol)
        {
            walkPointSet = false;
            canPatrol = false;
            Invoke(nameof(PatrolCountdown), patrolCd);
        }

        // In case enemy cant reach walk point
        if (resetWalkPoint)
        {
            Invoke(nameof(ChangeWalkPoint), 10f);
            resetWalkPoint = false;
        }
    }
    private void searchWalkPoint()
    {
        // Calculates random walk point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

        resetWalkPoint = true;
    }
    private void PatrolCountdown()
    {
        canPatrol = true;
    }
    private void ChangeWalkPoint()
    {
        if (!canPatrol)
            searchWalkPoint();
    }
    private void ChasePlayer()
    {
        // Finds player location and moves towards them
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        // Stops enemy movement
        agent.SetDestination(transform.position);

        if (!attacked) // Attacks if ready to attack
        {
            Animator anim = sword.GetComponent<Animator>();
            anim.SetTrigger("In Range"); // Attack animation
            Invoke(nameof(DelayHitbox), 0.1f);

            attacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void DelayHitbox()
    {
        // In order for the animation to partially play before getting instantly parried
        hitbox.enabled = true;
    }
    public void ResetAttack()
    {
        attacked = false;
        hitbox.enabled = false;
    }
    public void FinisherKill()
    {
        Debug.Log("Finisher activated");
        if (!canBeFinished) return;
        canBeFinished = false;
        finisherActive = true;
        MS.DisabledAnimation1();
        Animator self = GetComponent<Animator>();
        Invoke(nameof(EndFinisher), .25f);
    }
    private void EndFinisher()
    {
        enemy.health = 0;
        pl.health += 50;
    }
}
