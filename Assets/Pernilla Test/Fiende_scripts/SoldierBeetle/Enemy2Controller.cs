using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Olika tillstånd för fienden
    private enum EnemyState { Walk, AttackAndWalk, RunAndFlee, WalkAndDodge, Death, Dodge, Attack }
    private EnemyState currentState;

    // Avståndströsklar för olika beteenden
    public float walkDistance = 10f;
    public float runAndFleeDistance = 7f;
    public float attackDistance = 3f;

    // Hälsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 50f; // Tröskel för att starta dödsanimation

    void Start()
    {
        // Hämta Animator-komponenten från GameObject
        animator = GetComponent<Animator>();

        // Sätt initialt tillstånd
        currentState = EnemyState.Walk;
        SetAnimationState(currentState);
    }

    void Update()
    {
        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState(EnemyState.Death);
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer > walkDistance)
        {
            SetAnimationState(EnemyState.Walk);
        }
        else if (distanceToPlayer <= walkDistance && distanceToPlayer > runAndFleeDistance)
        {
            SetAnimationState(EnemyState.AttackAndWalk);
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
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("AttackAndWalk");
        animator.ResetTrigger("RunAndFlee");
        animator.ResetTrigger("WalkAndDodge");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Dodge");
        animator.ResetTrigger("Attack");

        // Sätt nya animationstrigger beroende på tillstånd
        switch (currentState)
        {
            case EnemyState.Walk:
                animator.SetTrigger("Walk");
                break;
            case EnemyState.AttackAndWalk:
                animator.SetTrigger("AttackAndWalk");
                break;
            case EnemyState.RunAndFlee:
                animator.SetTrigger("RunAndFlee");
                break;
            case EnemyState.WalkAndDodge:
                animator.SetTrigger("WalkAndDodge");
                break;
            case EnemyState.Death:
                animator.SetTrigger("Death");
                break;
            case EnemyState.Dodge:
                animator.SetTrigger("Dodge");
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Attack");
                break;
        }
    }
}
