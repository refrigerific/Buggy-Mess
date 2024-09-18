using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    [Header("General Specifics")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float speed = 50f;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float lifeTime = 5f;
    [Header("Bullet Specifics")]
    [SerializeField] protected GameObject enemyHitEffectPrefab;
    [SerializeField] protected GameObject wallHitEffectPrefab;
    [SerializeField] protected GameObject groundHitEffectPrefab;

    [SerializeField] protected Dictionary<string, GameObject> hitEffectsByTag;


    private Coroutine removeCoroutine;
    private void Awake()
    {
        //StartCoroutine(RemoveRoutine());

        Debug.Log($"Bullet {gameObject.name} enabled");

        // Restart the coroutine every time the bullet is enabled
        if (removeCoroutine != null)
        {
            StopCoroutine(removeCoroutine);
        }
        removeCoroutine = StartCoroutine(RemoveRoutine());

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
        //StartCoroutine(RemoveRoutine());
    }

    private void OnEnable()
    {
        Debug.Log("OnEnabled Started");

        // Restart the coroutine every time the bullet is enabled
        if (removeCoroutine != null)
        {
            StopCoroutine(removeCoroutine);
        }
        removeCoroutine = StartCoroutine(RemoveRoutine());
    }

    private void OnDisable()
    {
        // Ensure that coroutine is stopped when object is disabled
        if (removeCoroutine != null)
        {
            StopCoroutine(removeCoroutine);
        }
    }

    public void InitializeBullet(Vector3 initialVelocity)
    {
        // Apply the initial velocity to the Rigidbody
        if (rb != null)
        {
            rb.velocity = initialVelocity != Vector3.zero ? initialVelocity : transform.forward * speed;
        }
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

        ObjectPooling.ReturnObjectToPool(gameObject);
    }

    private IEnumerator RemoveRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        ObjectPooling.ReturnObjectToPool(gameObject);
    }
}
