using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrenadierAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minDistanceBetweenEnemies;
    [Space]
    [Header("Run away variables")]
    [SerializeField] private List<Transform> fleePlaces;
    [SerializeField] private float runAwayRange;
    [Space]
    [Header("Attack variables")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float shootingRange;
    [SerializeField] private float fireDelay;

    private Material material;
    private NavMeshAgent agent;
    private Node topNode;
    private Health health;
    private Animator animator;

    private bool isRunningAway = false;
    public bool IsRunningAway { get { return isRunningAway; } set { isRunningAway = value; } }

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;

        GameObject[] fleePlaceObjects = GameObject.FindGameObjectsWithTag("FleePlace");
        fleePlaces = new List<Transform>();
        foreach (GameObject fleePlaceObject in fleePlaceObjects)
        {
            fleePlaces.Add(fleePlaceObject.transform);
        }

        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        ConstructBehaviourTree();

        health.OnDeath += Death;
        health.OnDamaged += TakeDamage;
    }

    private void Update()
    {
        topNode.Evaluate();

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isRunningAway = false;
        }


        AvoidOtherEnemies();

        if (!isRunningAway && playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            directionToPlayer.y = 0f;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void ConstructBehaviourTree()
    {
        TailBoiShootNode tailBoiShootNode = new TailBoiShootNode(this, shootingPoint, projectilePrefab, fireDelay);
        RangeNode shootingRangeNode = new RangeNode(shootingRange, playerTransform, transform, animator);
        RangeNode runAwayRangeNode = new RangeNode(runAwayRange, playerTransform, transform, animator);
        FleeNode fleeNode = new FleeNode(this, agent, fleePlaces);

        Sequence shootSequence = new Sequence(new List<Node> { shootingRangeNode, tailBoiShootNode });
        Sequence runAwaySequence = new Sequence(new List<Node> { runAwayRangeNode, fleeNode });

        topNode = new Selector(new List<Node> {runAwaySequence, shootSequence });
    }

    private void AvoidOtherEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, minDistanceBetweenEnemies);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
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
        Gizmos.DrawWireSphere(transform.position, runAwayRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
