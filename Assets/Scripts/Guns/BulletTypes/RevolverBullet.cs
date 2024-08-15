using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBullet : BulletBase
{
    private void OnEnable()
    {
        StartCoroutine(RemoveRoutine());
    }

    protected override void OnHit(Collider other)
    {
        string tag = other.tag;
        if (hitEffectsByTag.TryGetValue(tag, out GameObject hitEffect))
        {
            //TODO: Refaktorera
            if(hitEffect == enemyHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.enemyImpactObject);
            }
            else if(hitEffect == wallHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.wallImpactObject);
            }
            else if (hitEffect == groundHitEffectPrefab)
            {
                ObjectPooling.SpawnObject(hitEffect, transform.position, hitEffect.transform.rotation, ObjectPooling.PoolType.groundImpactObject);
            }

            StartCoroutine(RemoveRoutine());
        }
        else
        {
            Debug.LogWarning("No hit effect found for tag: " + tag);
        }
    }

    private IEnumerator RemoveRoutine()
    {

        yield return new WaitForSeconds(lifeTime);

        ObjectPooling.ReturnObjectToPool(gameObject);
    }

}
