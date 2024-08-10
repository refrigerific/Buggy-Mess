using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    [SerializeField] protected float speed = 50f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected GameObject hitEffectPrefab;

    protected Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    protected abstract void OnHit(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        OnHit(other);

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
