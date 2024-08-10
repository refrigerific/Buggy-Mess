using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternateEnemyController : MonoBehaviour
{
    // Referens till Animator-komponenten
    private Animator animator;

    // Referens till spelarens transform
    public Transform playerTransform;

    // Avst�ndstr�sklar f�r beteenden
    public float walkDistance = 15f;
    public float attackDistance = 3f;

    // H�lsa och skada
    private float enemyHealth = 100f;
    public float damageThreshold = 0f; // Fienden d�r vid 0 h�lsa

    // Booleaner f�r animationstillst�nd
    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        // H�mta Animator-komponenten fr�n GameObject
        animator = GetComponent<Animator>();

        // S�tt initialt tillst�nd till Walking
        isWalking = true;
        SetAnimationState();
    }

    void Update()
    {
        // Ber�kna avst�ndet mellan fienden och spelaren
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kontrollera om fienden har tagit tillr�ckligt med skada f�r att d�
        if (enemyHealth <= damageThreshold)
        {
            isDead = true;
            SetAnimationState();
            return;
        }

        // Uppdatera fiendens tillst�nd baserat p� avst�ndet till spelaren
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
            // Om spelaren �r l�ngre bort �n walkDistance, stoppa animationen
            // Om detta inte �r �nskat kan du anv�nda en annan animation eller stanna
            isWalking = false;
            isAttacking = false;
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
            isDead = true;
            SetAnimationState();
        }
    }

    // Metod f�r att s�tta animationstillst�nd
    void SetAnimationState()
    {
        // S�tt booleaner f�r animationstillst�nd
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsDead", isDead);
    }
}
