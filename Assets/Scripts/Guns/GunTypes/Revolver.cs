using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Revolver : WeaponBase
{
    [Header("Revolver Specifics")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletImpactPrefab;
    [SerializeField] private float shootRange = 100f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private LineRenderer bulletTrail;

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
        Vector3 shootDirection = playerCamera.transform.forward;
        currentAmmo--;
        //muzzleFlash.Play();TODO
        //Temp
        audioSource.PlayOneShot(fireClip);
        //weaponAnimator.SetTrigger("Fire");
        //

        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, shootRange))
        {
            Debug.Log($"Hit: {hit.transform.name}");



            //Impact grejen, Ändra till setactive maybe
            GameObject impact = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);

            StartCoroutine(ShowBulletTrail(hit.point));
        }
        else
        {
            Vector3 endPoint = playerCamera.transform.position + shootDirection * shootRange;
            StartCoroutine(ShowBulletTrail(endPoint));
        }

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

    private IEnumerator ShowBulletTrail(Vector3 hitPoint)
    {
        bulletTrail.SetPosition(0, barrelEnd.position);
        bulletTrail.SetPosition(1, hitPoint);

        Debug.DrawLine(barrelEnd.position, hitPoint, Color.red, 2f);

        bulletTrail.enabled = true;

        yield return new WaitForSeconds(1f);

        bulletTrail.enabled = false;
    }
}
