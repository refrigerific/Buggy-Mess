using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    [Header("General Specifics")]
    [SerializeField] protected float speed = 50f;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float lifeTime = 5f;
    [Header("Bullet Specifics")]
    //[SerializeField] protected GameObject hitEffectPrefab;//Gör en lista och bytt till typ partikeleffekter
    [SerializeField] protected GameObject enemyHitEffectPrefab;
    [SerializeField] protected GameObject wallHitEffectPrefab;
    [SerializeField] protected GameObject groundHitEffectPrefab;

    [SerializeField] protected Dictionary<string, GameObject> hitEffectsByTag;

    protected Rigidbody rb;

    private void Awake()
    {
        hitEffectsByTag = new Dictionary<string, GameObject>
        {
            { "Enemy", enemyHitEffectPrefab },
            { "Wall", wallHitEffectPrefab },
            { "Ground", groundHitEffectPrefab }
            // Add more tag-prefab pairs as needed
        };
    }
    protected virtual void Start()
    {        
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
       
        Destroy(gameObject, lifeTime);
    }

    protected abstract void OnHit(Collider other);

    protected virtual void OnTriggerEnter(Collider other)
    {
        Health health = other.gameObject.GetComponent<Health>();
        if (health != null)
        {
            Debug.Log("Gör damage");
            health.TakeDamage(damage);
        }
        OnHit(other);       
        Destroy(gameObject);
    }
}
