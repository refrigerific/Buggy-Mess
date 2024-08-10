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
        SetAnimationState(currentState);
    }

    void Update()
    {
        // Ber�kna avst�ndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillr�ckligt med skada f�r att d�
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState(EnemyState.Death);
            return;
        }

        // Uppdatera fiendens tillst�nd baserat p� avst�ndet till spelaren
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
            // H�r kan du l�gga till logik f�r att g�ra att fienden hoppar ibland
            SetAnimationState(EnemyState.Jump);
        }
        else if (distanceToPlayer <= attackDistance)
        {
            SetAnimationState(EnemyState.Attack);
        }
    }

    // Anropas n�r fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Kontrollera om fienden ska d�
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState(EnemyState.Death);
        }
    }

    // Metod f�r att s�tta animationstillst�nd
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

        // S�tt nya animationstrigger beroende p� tillst�nd
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
