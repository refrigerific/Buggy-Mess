using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Avst�ndstr�sklar f�r beteenden
    public float walkDistance = 10f;
    public float attackDistance = 2f;

    // H�lsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 0f; // Fienden d�r vid 0 h�lsa

    void Start()
    {
        // H�mta Animator-komponenten fr�n GameObject
        animator = GetComponent<Animator>();

        // Startar i walking-l�ge
        SetAnimationState("Walk");
    }

    void Update()
    {
        // Ber�kna avst�ndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillr�ckligt med skada f�r att d�
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState("Death");
            return;
        }

        // Uppdatera fiendens tillst�nd baserat p� avst�ndet till spelaren
        if (distanceToPlayer <= attackDistance)
        {
            SetAnimationState("Attack");
        }
        else if (distanceToPlayer > attackDistance && distanceToPlayer <= walkDistance)
        {
            SetAnimationState("Walk");
        }
    }

    // Anropas n�r fienden tar skada
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        // Kontrollera om fienden ska d�
        if (enemyHealth <= damageThreshold)
        {
            SetAnimationState("Death");
        }
    }

    // Metod f�r att s�tta animationstillst�nd
    void SetAnimationState(string state)
    {
        // Reset alla animationstriggers
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Death");

        // S�tt animationstrigger beroende p� tillst�nd
        animator.SetTrigger(state);
    }
}
