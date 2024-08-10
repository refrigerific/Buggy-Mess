using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Revolver : WeaponBase
{
    [Header("Revolver Specifics")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private BulletBase bulletPrefab;

    private void Update()
    {
        if(isReloading) return;

        if(Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            if(currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Fire();
            }
        }
        else
        {
            //Kan addera ett ljud om ammot är slut, typ ett klick ljud :D
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }

        if(currentAmmo == 0) { Reload(); }
    }

    public override void Fire()
    {
        currentAmmo--;
        //muzzleFlash.Play();TODO
        //Temp
        audioSource.PlayOneShot(fireClip);
        //weaponAnimator.SetTrigger("Fire");
        //
        //TODO: Objectpoola
        BulletBase bulletInstance = Instantiate(bulletPrefab, barrelEnd.position, barrelEnd.rotation);
    }

    public override void Reload()
    {
        Debug.Log("Reloading!");
        if (isReloading) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        //ReloadLjuden och animation
        //weaponAnimator.SetTrigger("Reload");
        //audioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
