using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBullet : BulletBase
{
    [SerializeField] private float impactEffectDuration = 2f;
    protected override void OnHit(Collider other)
    {
        //Health health = other.gameObject.GetComponent<Health>();
        //if (health != null)
        //{
        //    Debug.Log("G�r damage");
        //    health.Damage(damage);
        //}

        //Kolla beroende p� vad man tr�ffar s� byts effekten
        if (hitEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(impactEffect, impactEffectDuration);
        }
    }
   
}
