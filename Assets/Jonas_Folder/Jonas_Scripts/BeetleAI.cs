using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeetleAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FieldOfView fov;
    [SerializeField] private Cover[] avaliableCovers;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float minDistanceBetweenEnemies = 4f;
    [SerializeField] private float healthRestoreRate;
    [Space]
    [Header("Chase variables")]
    [SerializeField] private float chasingRange = 30f;
    [Space]
    [Header("Attack variables")]
    [SerializeField] private float shootingRange = 20f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] [Range(0, 100)] private float spreadAngle = 40f;
    [SerializeField] private float fireDelay = 4f;

    private int damage = 10;

    private Material material;
    [SerializeField] private Transform bestCoverSpot;
    private Node topNode;
    private Health health;

    private bool isRunningAway = true;
    public bool IsRunningAway { get { return isRunningAway; } set { isRunningAway = value; } }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        health = GetComponent<Health>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;

        GameObject[] coverObjects = GameObject.FindGameObjectsWithTag("Spot");
        avaliableCovers = new Cover[coverObjects.Length];
        for (int i = 0; i < coverObjects.Length; i++)
        {
            avaliableCovers[i] = coverObjects[i].GetComponent<Cover>();
        }
    }

    public void Start()
    {
        ConstructBehaviourTree();

        health.OnDeath += Death;
        //health.OnDamaged += TakeDamage;
      
    }

    private void ConstructBehaviourTree()
    {
        IsCoverAvailableNode coverAvaliableNode = new IsCoverAvailableNode(avaliableCovers, playerTransform, this);
        GoToCoverNode goToCoverNode = new GoToCoverNode(agent, this);
        HealthNode healthNode = new HealthNode(this, health, healthRestoreRate);
        IsCoveredNode isCoveredNode = new IsCoveredNode(playerTransform, transform);
        ChaseNode chaseNode = new ChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform, animator);
        RangeNode shootingRangeNode = new RangeNode(shootingRange, playerTransform, transform, animator);
        ShootNode shootNode = new ShootNode(agent, this, playerTransform, projectilePrefab, shootingPoint, projectileSpeed, fireDelay, spreadAngle, fov);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode, });
        Sequence shootSequence = new Sequence(new List<Node> { shootingRangeNode, shootNode});

        Sequence goToCoverSequence = new Sequence(new List<Node> { coverAvaliableNode, goToCoverNode});
        Selector findCoverSelector = new Selector(new List<Node> { goToCoverSequence, chaseSequence});
        Selector tryToTakeCoverSelector = new Selector(new List<Node> { isCoveredNode, findCoverSelector});
        Sequence mainCoverSequence = new Sequence(new List<Node> { healthNode, tryToTakeCoverSelector});

        topNode = new Selector(new List<Node> { mainCoverSequence, shootSequence, chaseSequence });
    }

    private void Update()
    {
        topNode.Evaluate();
        if(topNode.NodeState == NodeState.FAILURE)
        {
            SetColor(Color.red);
        }

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

        
        RestoreHealth();
        //AvoidOtherEnemies();
        TakeDamage();
    }

    private void FixedUpdate()
    {
        AvoidOtherEnemies();
    }

    public void SetBestCoverSpot(Transform bestCoverSpot)
    {
        this.bestCoverSpot = bestCoverSpot;
    }

    public Transform GetBestCoverSpot()
    {
        return bestCoverSpot;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            health.TakeDamage(damage);
        }
    }
    
    private void Death()
    {
        gameObject.SetActive(false);
    }

    private void RestoreHealth()
    {
        if (health.GetCurrentHealth() == health.GetMaxHealth())
        {
            return;
        }

        health.Heal(healthRestoreRate);
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
        Gizmos.DrawWireSphere(transform.position, shootingRange);

        Vector3 spreadLine1 = Quaternion.AngleAxis(spreadAngle / 2, shootingPoint.transform.up) * shootingPoint.transform.forward * shootingRange;
        Vector3 spreadLine2 = Quaternion.AngleAxis(-spreadAngle / 2, shootingPoint.transform.up) * shootingPoint.transform.forward * shootingRange;

        Gizmos.color = Color.black;
        Gizmos.DrawLine(shootingPoint.transform.position, shootingPoint.transform.position + spreadLine1);
        Gizmos.DrawLine(shootingPoint.transform.position, shootingPoint.transform.position + spreadLine2);
        Gizmos.DrawLine(shootingPoint.transform.position + spreadLine1, shootingPoint.transform.position + spreadLine2);
    }
}
