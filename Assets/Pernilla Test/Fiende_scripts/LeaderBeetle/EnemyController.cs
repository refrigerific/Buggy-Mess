using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Olika tillstånd för fienden
    private enum EnemyState { Idle, Walk, WalkAndDodge, RunAndFlee, Dodge, Death, Buff, Attack, AttackAndWalk }
    private EnemyState currentState;

    // Avståndströsklar för olika beteenden
    public float idleDistance = 10f;
    public float walkDistance = 8f;
    public float runAndFleeDistance = 5f;
    public float attackDistance = 3f;

    // Hälsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 50f; // Tröskel för att starta dödsanimation

    // Tidsbaserade variabler
    private float gameTime = 0f;
    public float buffTime = 40f; // Tid efter vilken buff-animationen ska spelas

    // Booleaner för animationstillstånd
    private bool IsIdle = false;
    private bool IsWalking = false;
    private bool IsWalkingAndDodging = false;
    private bool IsRunningAndFleeing = false;
    private bool IsDodging = false;
    private bool IsDead = false;
    private bool IsBuffed = false;
    private bool IsAttacking = false;
    private bool IsAttackingAndWalking = false;

    void Start()
    {
        // Hämta Animator-komponenten från GameObject
        animator = GetComponent<Animator>();

        // Sätt initialt tillstånd
        currentState = EnemyState.Idle;
        SetAnimationState();
    }

    void Update()
    {
        // Uppdatera spelets tid
        gameTime += Time.deltaTime;

        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            currentState = EnemyState.Death;
            SetAnimationState();
            return;
        }

        // Om spelet har varit igång i mer än buffTime sekunder, spela buff-animation
        if (gameTime >= buffTime)
        {
            currentState = EnemyState.Buff;
            SetAnimationState();
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer > idleDistance)
        {
            currentState = EnemyState.Idle;
        }
        else if (distanceToPlayer <= idleDistance && distanceToPlayer > walkDistance)
        {
            currentState = EnemyState.Walk;
        }
        else if (distanceToPlayer <= walkDistance && distanceToPlayer > runAndFleeDistance)
        {
            currentState = EnemyState.WalkAndDodge;
        }
        else if (distanceToPlayer <= runAndFleeDistance && distanceToPlayer > attackDistance)
        {
            currentState = EnemyState.RunAndFlee;
        }
        else if (distanceToPlayer <= attackDistance)
        {
            currentState = EnemyState.Attack;
        }

        SetAnimationState();
    }

    // Anropas när fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Spela dodge-animation om fienden inte redan är i dödsanimation
        if (currentState != EnemyState.Death)
        {
            currentState = EnemyState.Dodge;
            SetAnimationState();
        }
    }

    // Metod för att sätta animationstillstånd
    void SetAnimationState()
    {
        // Reset alla booleaner
        IsIdle = false;
        IsWalking = false;
        IsWalkingAndDodging = false;
        IsRunningAndFleeing = false;
        IsDodging = false;
        IsDead = false;
        IsBuffed = false;
        IsAttacking = false;
        IsAttackingAndWalking = false;

        // Sätt nya booleaner beroende på tillstånd
        switch (currentState)
        {
            case EnemyState.Idle:
                IsIdle = true;
                break;
            case EnemyState.Walk:
                IsWalking = true;
                break;
            case EnemyState.WalkAndDodge:
                IsWalkingAndDodging = true;
                break;
            case EnemyState.RunAndFlee:
                IsRunningAndFleeing = true;
                break;
            case EnemyState.Dodge:
                IsDodging = true;
                break;
            case EnemyState.Death:
                IsDead = true;
                break;
            case EnemyState.Buff:
                IsBuffed = true;
                break;
            case EnemyState.Attack:
                IsAttacking = true;
                break;
            case EnemyState.AttackAndWalk:
                IsAttackingAndWalking = true;
                break;
        }

        // Uppdatera animationstillstånd i Animator
        animator.SetBool("IsIdle", IsIdle);
        animator.SetBool("IsWalking", IsWalking);
        animator.SetBool("IsWalkingAndDodging", IsWalkingAndDodging);
        animator.SetBool("IsRunningAndFleeing", IsRunningAndFleeing);
        animator.SetBool("IsDodging", IsDodging);
        animator.SetBool("IsDead", IsDead);
        animator.SetBool("IsBuffed", IsBuffed);
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsAttackingAndWalking", IsAttackingAndWalking);
    }
}
