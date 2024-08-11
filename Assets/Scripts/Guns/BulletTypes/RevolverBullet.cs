using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBullet : BulletBase
{
    [SerializeField] private float impactEffectDuration = 2f;

    protected override void OnHit(Collider other)
    {
        //Kolla beroende på vad man träffar så byts effekten
        //if (hitEffectPrefab != null)
        //{
        //    GameObject impactEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        //    Destroy(impactEffect, impactEffectDuration);
        //}

        string tag = other.tag;
        if (hitEffectsByTag.TryGetValue(tag, out GameObject hitEffect))
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            //Destroy(hitEffect.gameObject, impactEffectDuration);
        }
        else
        {
            Debug.LogWarning("No hit effect found for tag: " + tag);
        }
    }
   
}
