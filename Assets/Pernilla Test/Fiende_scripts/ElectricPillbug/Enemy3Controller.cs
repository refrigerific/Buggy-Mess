using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3Controller : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Olika tillst�nd f�r fienden
    private enum EnemyState { Idle, Attack, RunAndRush, Run, Death, Jump }
    private EnemyState currentState;

    // Booleaner f�r animationstillst�nd
    private bool isIdle = false;
    private bool isAttacking = false;
    private bool isRunningAndRushing = false;
    private bool isRunning = false;
    private bool isDead = false;
    private bool isJumping = false;

    // Avst�ndstr�sklar f�r olika beteenden
    public float idleDistance = 15f;
    public float runDistance = 10f;
    public float runAndRushDistance = 5f;
    public float attackDistance = 2f;

    // H�lsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 0f; // Fienden d�r vid 0 h�lsa

    void Start()
    {
        // H�mta Animator-komponenten fr�n GameObject
        animator = GetComponent<Animator>();

        // S�tt initialt tillst�nd
        currentState = EnemyState.Idle;
        SetAnimationState();
    }

    void Update()
    {
        // Ber�kna avst�ndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillr�ckligt med skada f�r att d�
        if (enemyHealth <= damageThreshold)
        {
            currentState = EnemyState.Death;
            SetAnimationState();
            return;
        }

        // Uppdatera fiendens tillst�nd baserat p� avst�ndet till spelaren
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

    // Anropas n�r fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Kontrollera om fienden ska d�
        if (enemyHealth <= damageThreshold)
        {
            currentState = EnemyState.Death;
            SetAnimationState();
        }
    }

    // Metod f�r att s�tta animationstillst�nd
    void SetAnimationState()
    {
        // Reset alla booleaner
        isIdle = false;
        isAttacking = false;
        isRunningAndRushing = false;
        isRunning = false;
        isDead = false;
        isJumping = false;

        // S�tt nya booleaner beroende p� tillst�nd
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

        // Uppdatera animationstillst�nd i Animator
        animator.SetBool("IsIdle", isIdle);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsRunningAndRushing", isRunningAndRushing);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsDead", isDead);
        animator.SetBool("IsJumping", isJumping);
    }
}
