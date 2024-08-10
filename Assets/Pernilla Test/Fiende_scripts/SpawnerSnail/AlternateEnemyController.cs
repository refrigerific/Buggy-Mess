using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateEnemyController : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Avståndströsklar för beteenden
    public float walkDistance = 15f;
    public float attackDistance = 3f;

    // Hälsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 0f; // Fienden dör vid 0 hälsa

    // Booleaner för animationstillstånd
    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        // Hämta Animator-komponenten från GameObject
        animator = GetComponent<Animator>();

        // Sätt initialt tillstånd till Walking
        isWalking = true;
        SetAnimationState();
    }

    void Update()
    {
        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            isDead = true;
            SetAnimationState();
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer <= attackDistance)
        {
            isAttacking = true;
            isWalking = false;
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= walkDistance)
        {
            isWalking = true;
            isAttacking = false;
        }
        else
        {
            // Om spelaren är längre bort än walkDistance, stoppa animationen
            // Om detta inte är önskat kan du använda en annan animation eller stanna
            isWalking = false;
            isAttacking = false;
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
            isDead = true;
            SetAnimationState();
        }
    }

    // Metod för att sätta animationstillstånd
    void SetAnimationState()
    {
        // Sätt booleaner för animationstillstånd
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsDead", isDead);
    }
}
