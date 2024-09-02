using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class PillbugAI : EnemyBase
{
    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FieldOfView fov;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Health health;
    [SerializeField] private EnemyAudioData pillbugAudio;
    [SerializeField] private StunManager stunManager;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minDistanceBetweenEnemies;
    [SerializeField] private float movementSpeed = 3.5f;
    [Space]
    [Header("Chase variables")]
    [SerializeField] private float chasingRange;
    [Space]
    [Header("Attack variables")]
    [SerializeField] private GameObject meleeZonePrefab;
    [SerializeField] private int damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackTime;
    [Space]
    [Header("Threshold variables")]
    [SerializeField] private float lowHealthMovementSpeed;
    //Addera mer stat h�jningar 

    private Material material;
    private Node topNode;
    private BehaviorTree behaviorTree;

    [SerializeField] private bool isStunned = false;

    private bool isRunningAway = false;
    public bool IsRunningAway { get { return isRunningAway; } set { isRunningAway = value; } }
    public int Damage { get { return damage; } set { damage = value; } }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
    }

    public void Start()
    {
        //StunManager stunManager = GetComponent<StunManager>();
        //if (stunManager != null)
        //{
        //    stunManager.Initialize(this); // Pass this instance to StunManager
        //}
        stunManager.Initialize(this);
        ConstructBehaviourTree();
        behaviorTree = new BehaviorTree(topNode);
        behaviorTree.Start();

        health.OnDeath += Death;
        health.OnDamaged += TakeDamage;
    }

    private void ConstructBehaviourTree()
    {
        PillBugHealthNode healthNode = new PillBugHealthNode(this, health);
        PillBugChaseNode chaseNode = new PillBugChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform, animator);
        RangeNode meleeRangeNode = new RangeNode(attackRange, playerTransform, transform, animator);
        MeleeNode meleeNode = new MeleeNode(agent, this, meleeZonePrefab, fov, attackCooldown, attackTime, animator);
        PillBugThresholdNode thresholdNode = new PillBugThresholdNode(agent, lowHealthMovementSpeed);

        // Create the stun check node
        StunManager stunManager = GetComponent<StunManager>();
        CheckStunnedNode checkStunnedNode = new CheckStunnedNode(stunManager);

        Sequence chaseSequence = new Sequence(new List<Node> { checkStunnedNode, chasingRangeNode, chaseNode, });
        Sequence MeeleeSequence = new Sequence(new List<Node> { checkStunnedNode, meleeRangeNode, meleeNode });
        Sequence ThresholdSequence = new Sequence(new List<Node> { healthNode, thresholdNode });

        topNode = new Selector(new List<Node> { ThresholdSequence, MeeleeSequence, chaseSequence });
    }

    private void Update()
    {
        if (!isStunned)
        {
            //topNode.Evaluate();
            behaviorTree.Evaluate();

            if (!isRunningAway && playerTransform != null)// && fov.SeesPlayer
            {
                Vector3 directionToPlayer = playerTransform.position - transform.position;

                directionToPlayer.y = 0f;

                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }

            AvoidOtherEnemies();
        }    
    }

    private void AvoidOtherEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, minDistanceBetweenEnemies);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
                // If another enemy is too close, move away from it
                Vector3 directionToOtherEnemy = (transform.position - collider.transform.position).normalized;
                transform.position += directionToOtherEnemy * Time.deltaTime;
            }
        }
    }

    public override void SetStunned(bool stunned)
    {
        isStunned = stunned;

        if(stunned)
        {
            //Stanna behaviourtreet här
            behaviorTree.Stop();
            //Stanna movement
            agent.speed = 0f;
        }
        else
        {
            //Starta behaviourtreet igen
            behaviorTree.Start();
            //Starta movement
            agent.speed = movementSpeed;
        }
    }

    public override bool IsStunned()
    {
        return isStunned;
    }

    public bool CheckStunned()
    {
        //Hjälp för behaviour treet för att att kolla om den är stunnad!
        return isStunned;
    }

    private void TakeDamage()
    {
        //TODO fiendeHealthBars �ndringar

        // Stun the enemy for a certain duration
        //StunManager stunManager = GetComponent<StunManager>();
        //if (stunManager != null)
        //{
        //    stunManager.ApplyStun(stunManager.StunDuration);
        //}
        stunManager.ApplyStun(stunManager.StunDuration);
        RuntimeManager.PlayOneShotAttached(pillbugAudio.hurt, gameObject);
        RuntimeManager.PlayOneShotAttached(pillbugAudio.hurtFeedback, gameObject);

    }

    private void Death()
    {
        gameObject.SetActive(false);
        RuntimeManager.PlayOneShotAttached(pillbugAudio.death, gameObject);
        RuntimeManager.PlayOneShotAttached(pillbugAudio.deathFeedback, gameObject);
    }

    //Hj�lpMetoder
    public void SetColor(Color color)
    {
        material.color = color;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chasingRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
