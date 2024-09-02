using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodingPillBugAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FieldOfView fov;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private BezierProjectileMovement bezierProjectile;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minDistanceBetweenEnemies;
    [Space]
    [Header("Chase variables")]
    [SerializeField] private float chasingRange;
    [Space]
    [Header("Attack variables")]
    [SerializeField] private GameObject explodeZonePrefab;
    [SerializeField] private float attackRange;
    [SerializeField] private float explodeChargeUpTime;
    [SerializeField] private Vector3 explosionSize;
    [Space]
    [Header("Threshold variables")]    //Addera mer stat höjningar senare
    [SerializeField] private float lowHealthMovementSpeed;

    //[SerializeField] private Rigidbody rigidbody;
    private Material material;
    private Node topNode;
    private Health health;
    
    private bool hasMovedOnce = false;
    private bool isRunningAway = false;
    private bool isSpawned = false;

    public bool IsSpawned { get { return isSpawned; } set { isSpawned = value; } }
    public bool IsRunningAway { get { return isRunningAway; } set { isRunningAway = value; } }
    public bool HasMovedOnes { get { return hasMovedOnce; } set { hasMovedOnce = value; } }
    public float AttackRange { get { return attackRange; } set { attackRange = value; } }

    private void Awake()
    {
        //agent = GetComponentInChildren<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;
        //animator = GetComponentInChildren<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
        health = GetComponent<Health>();
    }

    public void Start()
    {
        ConstructBehaviourTree();

        health.OnDeath += Death;
        health.OnDamaged += TakeDamage;
    }

    private void ConstructBehaviourTree()
    {
        ExplodePillbugHealthNode healthNode = new ExplodePillbugHealthNode(this, health);
        ExplodePillbugChaseNode chaseNode = new ExplodePillbugChaseNode(playerTransform, agent, this);
        ExplodingPillBugRangeNode chasingRangeNode = new ExplodingPillBugRangeNode(chasingRange, playerTransform, transform, this);
        ExplodingPillBugRangeNode meleeRangeNode = new ExplodingPillBugRangeNode(attackRange, playerTransform, transform, this);
        ExplodeNode explodeNode = new ExplodeNode(agent, this, explodeZonePrefab, fov, explodeChargeUpTime, playerTransform, attackRange, explosionSize, bezierProjectile);
        PillBugThresholdNode thresholdNode = new PillBugThresholdNode(agent, lowHealthMovementSpeed);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode });
        Sequence explodeSequence = new Sequence(new List<Node> { meleeRangeNode, explodeNode });
        Sequence thresholdSequence = new Sequence(new List<Node> { healthNode, thresholdNode });

        topNode = new Selector(new List<Node> { thresholdSequence, explodeSequence, chaseSequence });
    }

    private void Update()
    {
        topNode.Evaluate();

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

    private void TakeDamage()
    {
        //TODO fiendeHealthBars ändringar
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

    //HjälpMetoder
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
