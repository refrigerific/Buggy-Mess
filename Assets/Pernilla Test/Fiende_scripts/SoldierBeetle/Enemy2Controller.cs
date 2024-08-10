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

    // Booleaner f�r animationstillst�nd
    private bool IsWalking = false;
    private bool IsAttackingAndWalking = false;
    private bool IsRunningAndFleeing = false;
    private bool IsWalkingAndDodging = false;
    private bool IsDead = false;
    private bool IsDodging = false;
    private bool IsAttacking = false;

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
        if (distanceToPlayer > walkDistance)
        {
            currentState = EnemyState.Walk;
        }
        else if (distanceToPlayer <= walkDistance && distanceToPlayer > runAndFleeDistance)
        {
            currentState = EnemyState.AttackAndWalk;
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

    // Anropas n�r fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Spela dodge-animation om fienden inte redan �r i d�dsanimation
        if (currentState != EnemyState.Death)
        {
            currentState = EnemyState.Dodge;
            SetAnimationState();
        }
    }

    // Metod f�r att s�tta animationstillst�nd
    void SetAnimationState()
    {
        // Reset alla booleaner
        IsWalking = false;
        IsAttackingAndWalking = false;
        IsRunningAndFleeing = false;
        IsWalkingAndDodging = false;
        IsDead = false;
        IsDodging = false;
        IsAttacking = false;

        // S�tt nya booleaner beroende p� tillst�nd
        switch (currentState)
        {
            case EnemyState.Walk:
                IsWalking = true;
                break;
            case EnemyState.AttackAndWalk:
                IsAttackingAndWalking = true;
                break;
            case EnemyState.RunAndFlee:
                IsRunningAndFleeing = true;
                break;
            case EnemyState.WalkAndDodge:
                IsWalkingAndDodging = true;
                break;
            case EnemyState.Death:
                IsDead = true;
                break;
            case EnemyState.Dodge:
                IsDodging = true;
                break;
            case EnemyState.Attack:
                IsAttacking = true;
                break;
        }

        // Uppdatera animationstillst�nd i Animator
        animator.SetBool("IsWalking", IsWalking);
        animator.SetBool("IsAttackingAndWalking", IsAttackingAndWalking);
        animator.SetBool("IsRunningAndFleeing", IsRunningAndFleeing);
        animator.SetBool("IsWalkingAndDodging", IsWalkingAndDodging);
        animator.SetBool("IsDead", IsDead);
        animator.SetBool("IsDodging", IsDodging);
        animator.SetBool("IsAttacking", IsAttacking);
    }
}
