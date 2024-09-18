using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherBullet : BulletBase
{
    [SerializeField] private float explosionRadius = 5f;  // Radius of the explosion
    [SerializeField] private float explosionForce = 700f; // Force of the explosion
    [SerializeField] private LayerMask explosionLayers;   // Layers that will be affected by the explosion
    [SerializeField] private GameObject explosionEffect;  // Explosion effect prefab - Gör om senare till Particlesystem!

    private void OnEnable()
    {
        StartCoroutine(RemoveRoutine());
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
        // Spawn explosion effect - TODO: Particle grej
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // Draw a wireframe sphere for the explosion radius
    }
}
