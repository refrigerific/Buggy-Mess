using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using FMODUnity;

public class Revolver : WeaponBase
{
    [Header("Revolver Specifics")]
    [SerializeField] private GameObject bulletCasingPrefab;
    [SerializeField] private Transform casingEjectionPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private BulletBase bulletPrefab;
    [SerializeField] private Recoil recoil;
    [SerializeField] private TextMeshProUGUI ammoText;

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
            //Kan addera ett ljud om ammot �r slut, typ ett klick ljud :D
        }

        if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }

        if(currentAmmo == 0) { Reload(); }
        UpdateAmmoUI();
    }

    public override void Fire()
    {
        currentAmmo--;
        if (bulletCasingPrefab != null && casingEjectionPoint != null)
        {
            Instantiate(bulletCasingPrefab, casingEjectionPoint.position, casingEjectionPoint.rotation);
        }

        //muzzleFlash.Play();TODO
        //Temp
        //audioSource.PlayOneShot(fireClip);
        //weaponAnimator.SetTrigger("Fire");
        //
        //TODO: Objectpoola
        BulletBase bulletInstance = Instantiate(bulletPrefab, barrelEnd.position, barrelEnd.rotation);
        
        RuntimeManager.PlayOneShotAttached(fire, gameObject);

        

        recoil.ApplyRecoil();
    }

    public override void Reload()
    {
        Debug.Log("Reloading!");
        if (isReloading) return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        RuntimeManager.PlayOneShotAttached(reload, gameObject);
        
        isReloading = true;

        //ReloadLjuden och animation
        //weaponAnimator.SetTrigger("Reload");
        //audioSource.PlayOneShot(reloadClip);

        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString();
        }
    }
}
