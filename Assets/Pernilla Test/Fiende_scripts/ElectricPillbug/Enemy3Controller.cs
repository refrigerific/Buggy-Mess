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

    // Booleaner för animationstillstånd
    private bool isIdle = false;
    private bool isAttacking = false;
    private bool isRunningAndRushing = false;
    private bool isRunning = false;
    private bool isDead = false;
    private bool isJumping = false;

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
        SetAnimationState();
    }

    void Update()
    {
        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            currentState = EnemyState.Death;
            SetAnimationState();
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer > idleDistance)
        {
            currentState = EnemyState.Idle;
        }
        else if (distanceToPlayer <= idleDistance && distanceToPlayer > runDistance)
        {
            currentState = EnemyState.Run;
        }
        else if (distanceToPlayer <= runDistance && distanceToPlayer > runAndRushDistance)
        {
            currentState = EnemyState.RunAndRush;
        }
        else if (distanceToPlayer <= runAndRushDistance && distanceToPlayer > attackDistance)
        {
            currentState = EnemyState.Jump;
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

        // Kontrollera om fienden ska dö
        if (enemyHealth <= damageThreshold)
        {
            currentState = EnemyState.Death;
            SetAnimationState();
        }
    }

    // Metod för att sätta animationstillstånd
    void SetAnimationState()
    {
        // Reset alla booleaner
        isIdle = false;
        isAttacking = false;
        isRunningAndRushing = false;
        isRunning = false;
        isDead = false;
        isJumping = false;

        // Sätt nya booleaner beroende på tillstånd
        switch (currentState)
        {
            case EnemyState.Idle:
                isIdle = true;
                break;
            case EnemyState.Attack:
                isAttacking = true;
                break;
            case EnemyState.RunAndRush:
                isRunningAndRushing = true;
                break;
            case EnemyState.Run:
                isRunning = true;
                break;
            case EnemyState.Death:
                isDead = true;
                break;
            case EnemyState.Jump:
                isJumping = true;
                break;
        }

        // Uppdatera animationstillstånd i Animator
        animator.SetBool("IsIdle", isIdle);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsRunningAndRushing", isRunningAndRushing);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsDead", isDead);
        animator.SetBool("IsJumping", isJumping);
    }
}
