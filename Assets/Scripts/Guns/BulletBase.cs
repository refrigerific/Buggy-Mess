using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    [Header("General Specifics")]
    [SerializeField] protected float speed = 50f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float lifeTime = 5f;
    [Header("Bullet Specifics")]
    [SerializeField] protected GameObject hitEffectPrefab;//Gör en lista och bytt till typ partikeleffekter
    //[SerializeField] protected TrailRenderer trailRenderer;

    protected Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        //if (trailRenderer != null)
        //{
        //    trailRenderer.time = 0.2f;
        //    trailRenderer.startWidth = 0.1f;
        //    trailRenderer.endWidth = 0f;
        //    trailRenderer.startColor = Color.white;
        //    trailRenderer.endColor = new Color(1, 1, 1, 0);
        //}

        Destroy(gameObject, lifeTime);
    }

    protected abstract void OnHit(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        OnHit(other);       
        Destroy(gameObject);
    }
}
