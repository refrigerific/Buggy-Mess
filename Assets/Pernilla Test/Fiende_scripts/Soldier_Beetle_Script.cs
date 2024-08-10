using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Olika tillst�nd f�r fienden
    private enum EnemyState { Walk, AttackAndWalk, RunAndFlee, WalkAndDodge, Death, Dodge, Attack }
    private EnemyState currentState;

    // Avst�ndstr�sklar f�r olika beteenden
    public float walkDistance = 10f;
    public float runAndFleeDistance = 7f;
    public float attackDistance = 3f;

    // H�lsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 50f; // Tr�skel f�r att starta d�dsanimation

    void Start()
    {
        // H�mta Animator-komponenten fr�n GameObject
        animator = GetComponent<Animator>();

        // S�tt initialt tillst�nd
        currentState = EnemyState.Walk;
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

    // Anropas n�r fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Spela dodge-animation om fienden inte redan �r i d�dsanimation
        if (currentState != EnemyState.Death)
        {
            SetAnimationState(EnemyState.Dodge);
        }
    }

    // Metod f�r att s�tta animationstillst�nd
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

        // S�tt nya animationstrigger beroende p� tillst�nd
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
