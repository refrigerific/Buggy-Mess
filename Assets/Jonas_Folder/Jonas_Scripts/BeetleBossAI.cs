using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeetleBossAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FieldOfView fov;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float startingHealth;//Ta bort
    [SerializeField] private float lowHealthThreshold;//Ta bort
    [SerializeField] private float healthRestoreRate;//Ta bort
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minDistanceBetweenEnemies;
    [Space]
    [Header("Chase variables")]
    [SerializeField] private float chasingRange;
    [Space]
    [Header("Attack variables")]
    [SerializeField] private float shootingRange;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float projectileSpeed;
    [SerializeField] [Range(0, 100)] private float spreadAngle;
    [SerializeField] private float fireDelay;
    [Space]
    [Header("Buff variables")]
    [SerializeField] private float buffZone;

    private Material material;
    private NavMeshAgent agent;
    private Node topNode;

    private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, startingHealth); }
    }

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        material = GetComponent<MeshRenderer>().material;

        currentHealth = startingHealth;
        ConstructBehaviourTree();
    }

    private void Update()
    {
        topNode.Evaluate();
        if (topNode.NodeState == NodeState.FAILURE)
        {
            SetColor(Color.red);
        }

        AvoidOtherEnemies();

        if (playerTransform != null)
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
        topNode = new Selector(new List<Node> { });
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffZone);

        Vector3 spreadLine1 = Quaternion.AngleAxis(spreadAngle / 2, shootingPoint.transform.up) * shootingPoint.transform.forward * shootingRange;
        Vector3 spreadLine2 = Quaternion.AngleAxis(-spreadAngle / 2, shootingPoint.transform.up) * shootingPoint.transform.forward * shootingRange;

        Gizmos.color = Color.black;
        Gizmos.DrawLine(shootingPoint.transform.position, shootingPoint.transform.position + spreadLine1);
        Gizmos.DrawLine(shootingPoint.transform.position, shootingPoint.transform.position + spreadLine2);
        Gizmos.DrawLine(shootingPoint.transform.position + spreadLine1, shootingPoint.transform.position + spreadLine2);
    }
}
