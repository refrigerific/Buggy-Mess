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

    void Start()
    {
        // Hämta Animator-komponenten från GameObject
        animator = GetComponent<Animator>();

        // Sätt initialt tillstånd
        currentState = EnemyState.Idle;
        SetAnimationState(currentState);
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
            SetAnimationState(EnemyState.Death);
            return;
        }

        // Om spelet har varit igång i mer än 40 sekunder, spela buff-animation
        if (gameTime >= buffTime)
        {
            SetAnimationState(EnemyState.Buff);
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer > idleDistance)
        {
            SetAnimationState(EnemyState.Idle);
        }
        else if (distanceToPlayer <= idleDistance && distanceToPlayer > walkDistance)
        {
            SetAnimationState(EnemyState.Walk);
        }
        else if (distanceToPlayer <= walkDistance && distanceToPlayer > runAndFleeDistance)
        {
            SetAnimationState(EnemyState.WalkAndDodge);
        }
        else if (distanceToPlayer <= runAndFleeDistance && distanceToPlayer > attackDistance)
        {
            SetAnimationState(EnemyState.RunAndFlee);
        }
        else if (distanceToPlayer <= attackDistance)
        {
            SetAnimationState(EnemyState.Attack);
        }
    }

    // Anropas när fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Spela dodge-animation om fienden inte redan är i dödsanimation
        if (currentState != EnemyState.Death)
        {
            SetAnimationState(EnemyState.Dodge);
        }
    }

    // Metod för att sätta animationstillstånd
    void SetAnimationState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        // Reset alla animationstriggers
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("WalkAndDodge");
        animator.ResetTrigger("RunAndFlee");
        animator.ResetTrigger("Dodge");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Buff");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("AttackAndWalk");

        // Sätt nya animationstrigger beroende på tillstånd
        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Walk:
                animator.SetTrigger("Walk");
                break;
            case EnemyState.WalkAndDodge:
                animator.SetTrigger("WalkAndDodge");
                break;
            case EnemyState.RunAndFlee:
                animator.SetTrigger("RunAndFlee");
                break;
            case EnemyState.Dodge:
                animator.SetTrigger("Dodge");
                break;
            case EnemyState.Death:
                animator.SetTrigger("Death");
                break;
            case EnemyState.Buff:
                animator.SetTrigger("Buff");
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Attack");
                break;
            case EnemyState.AttackAndWalk:
                animator.SetTrigger("AttackAndWalk");
                break;
        }
    }
}
