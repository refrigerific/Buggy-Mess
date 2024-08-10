using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Controller : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Olika tillstånd för fienden
    private enum EnemyState { Idle, Attack, RunAndRush, Run, Death, Jump }
    private EnemyState currentState;

    // Avståndströsklar för olika beteenden
    public float idleDistance = 15f;
    public float runDistance = 10f;
    public float runAndRushDistance = 5f;
    public float attackDistance = 2f;

    // Hälsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 0f; // Fienden dör vid 0 hälsa

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
        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState(EnemyState.Death);
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer > idleDistance)
        {
            SetAnimationState(EnemyState.Idle);
        }
        else if (distanceToPlayer <= idleDistance && distanceToPlayer > runDistance)
        {
            SetAnimationState(EnemyState.Run);
        }
        else if (distanceToPlayer <= runDistance && distanceToPlayer > runAndRushDistance)
        {
            SetAnimationState(EnemyState.RunAndRush);
        }
        else if (distanceToPlayer <= runAndRushDistance && distanceToPlayer > attackDistance)
        {
            // Här kan du lägga till logik för att göra att fienden hoppar ibland
            SetAnimationState(EnemyState.Jump);
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

        // Kontrollera om fienden ska dö
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState(EnemyState.Death);
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
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("RunAndRush");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Jump");

        // Sätt nya animationstrigger beroende på tillstånd
        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetTrigger("Idle");
                break;
            case EnemyState.Attack:
                animator.SetTrigger("Attack");
                break;
            case EnemyState.RunAndRush:
                animator.SetTrigger("RunAndRush");
                break;
            case EnemyState.Run:
                animator.SetTrigger("Run");
                break;
            case EnemyState.Death:
                animator.SetTrigger("Death");
                break;
            case EnemyState.Jump:
                animator.SetTrigger("Jump");
                break;
        }
    }
}
