using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverBullet : BulletBase
{
    [SerializeField] private float impactEffectDuration = 2f;
    protected override void OnHit(Collider other)
    {       
        //Kolla beroende p� vad man tr�ffar s� byts effekten
        if (hitEffectPrefab != null)
        {
            GameObject impactEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(impactEffect, impactEffectDuration);
        }
    }
   
}
