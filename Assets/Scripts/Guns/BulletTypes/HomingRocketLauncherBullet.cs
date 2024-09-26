using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocketLauncherBullet : BulletBase
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private LayerMask explosionLayers;
    [SerializeField] private GameObject explosionEffect;
    [Header("Homing variables")]
    [SerializeField] [Range(0.1f, 2f)] private float lockOnDelay = 0.5f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float detectionRadius = 50f; // Radius to detect enemies
    [SerializeField] private LayerMask enemyLayerMask;    // Define which layers are considered enemies

    private Transform target;
  
    private void OnEnable()
    {
        target = null;  // Reset the target when the rocket is spawned
        rb.velocity = Vector3.zero;  // Reset velocity to prevent it from moving immediately in the previous direction
        StartCoroutine(AcquireTargetAfterDelay());
        StartCoroutine(RemoveRoutine());
    }

    private void Update()
    {
        if (target != null)
        {
            HomeInOnTarget();
        }
        else
        {
            // Move forward if no target is assigned
            rb.velocity = transform.forward * speed;
        }
    }

    private void HomeInOnTarget()
    {
        // Calculate direction to the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Calculate a smooth rotation step to create the curve effect
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Move the rocket in the new direction
        rb.velocity = transform.forward * speed;
    }

    private void FindClosestEnemy()
    {
        // Find all colliders in the detection radius that belong to the enemy layer
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // Loop through all found enemies and find the closest one that is active
        foreach (Collider enemy in enemiesInRange)
        {
            // Check if the enemy is active in the hierarchy (not disabled)
            if (!enemy.gameObject.activeInHierarchy)
            {
                continue; // Skip this enemy if it's disabled
            }

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        // If we found an enemy, set it as the target
        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
        else
        {
            Debug.Log("No active enemies found within range.");
        }
    }

    protected override void OnHit(Collider other)
    {
        string tag = other.tag;
        if (hitEffectsByTag.TryGetValue(tag, out GameObject hitEffect))
        {
            if (hitEffect == enemyHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.enemyImpactObject);
            }
            else if (hitEffect == wallHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.wallImpactObject);
            }
            else if (hitEffect == groundHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.groundImpactObject);
            }

            Explode();
            StartCoroutine(RemoveRoutine());
        }
        else
        {
            Debug.LogWarning("No hit effect found for tag: " + tag);
        }
    }

    private void Explode()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            ObjectPooling.SpawnObject(explosionEffect, transform.position, Quaternion.identity, ObjectPooling.PoolType.explosionObject);
        }

        // Find objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);

        foreach (Collider nearbyObject in colliders)
        {
            // Apply damage or other effects to objects with a certain tag
            // You can add a damage component or interface to handle the damage on the objects

            Health health = nearbyObject.gameObject.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log("Gör explosition damage");
                health.TakeDamage(damage);
            }

            // Optionally apply explosion force
            //Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            //}
        }
    }

    private IEnumerator RemoveRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Explode();
        ObjectPooling.ReturnObjectToPool(gameObject);
    }

    private IEnumerator AcquireTargetAfterDelay()
    {
        // Add a small delay before acquiring a target (this allows time for the rocket to initialize properly)
        yield return new WaitForSeconds(lockOnDelay);

        FindClosestEnemy();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Draw a wireframe sphere for the explosion radius
        Gizmos.color = Color.yellow; // Visualize the detection radius for enemies
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
