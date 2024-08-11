using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class PillbugAI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private FieldOfView fov;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [Space]
    [Header("Misc variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minDistanceBetweenEnemies;
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
    //Addera mer stat höjningar 

    private Material material;
   
    private Node topNode;
    private Health health;//Ta bort
    

    private bool isRunningAway = false;
    public bool IsRunningAway { get { return isRunningAway; } set { isRunningAway = value; } }
    public int Damage { get { return damage; } set { damage = value; } }

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
        health = GetComponent<Health>();//Ta bort

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
    }

    public void Start()
    {
        ConstructBehaviourTree();

        health.OnDeath += Death;//Ta bort
        health.OnDamaged += TakeDamage;//Ta bort
    }

    private void ConstructBehaviourTree()
    {
        PillBugHealthNode healthNode = new PillBugHealthNode(this, health);
        PillBugChaseNode chaseNode = new PillBugChaseNode(playerTransform, agent, this);
        RangeNode chasingRangeNode = new RangeNode(chasingRange, playerTransform, transform, animator);
        RangeNode meleeRangeNode = new RangeNode(attackRange, playerTransform, transform, animator);
        MeleeNode meleeNode = new MeleeNode(agent, this, meleeZonePrefab, fov, attackCooldown, attackTime, animator);
        PillBugThresholdNode thresholdNode = new PillBugThresholdNode(agent, lowHealthMovementSpeed);

        Sequence chaseSequence = new Sequence(new List<Node> { chasingRangeNode, chaseNode, });
        Sequence MeeleeSequence = new Sequence(new List<Node> { meleeRangeNode, meleeNode });
        Sequence ThresholdSequence = new Sequence(new List<Node> { healthNode, thresholdNode });

        topNode = new Selector(new List<Node> { ThresholdSequence, MeeleeSequence, chaseSequence });
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

    [BankRef]
    public string SFX;
    private void TakeDamage()
    {
        //TODO fiendeHealthBars ändringar     
        RuntimeManager.PlayOneShot("event:/Sounds/Ingame/Enemy/Enemy Hit", GetComponent<Transform>().position);
    }

    private void Death()
    {
        gameObject.SetActive(false);
        RuntimeManager.PlayOneShot("event:/Sounds/Ingame/Enemy/Enemy Death",GetComponent<Transform>().position);
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
