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

    void Start()
    {
        // Hämta Animator-komponenten från GameObject
        animator = GetComponent<Animator>();

        // Sätt initialt tillstånd till Walking
        SetAnimationState("Walk");
    }

    void Update()
    {
        // Beräkna avståndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillräckligt med skada för att dö
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState("Death");
            return;
        }

        // Uppdatera fiendens tillstånd baserat på avståndet till spelaren
        if (distanceToPlayer <= attackDistance)
        {
            SetAnimationState("Attack");
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= walkDistance)
        {
            SetAnimationState("Walk");
        }
        else
        {
            // Om spelaren är längre bort än walkDistance, stoppa animationen
            // Vi sätter fortfarande tillståndet till "Walk" här
            // Om detta inte är önskat kan du använda en annan animation eller stanna
            SetAnimationState("Walk");
        }
    }

    // Anropas när fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Kontrollera om fienden ska dö
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState("Death");
        }
    }

    // Metod för att sätta animationstillstånd
    void SetAnimationState(string state)
    {
        // Reset alla animationstriggers
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Death");

        // Sätt animationstrigger beroende på tillstånd
        animator.SetTrigger(state);
    }
}
