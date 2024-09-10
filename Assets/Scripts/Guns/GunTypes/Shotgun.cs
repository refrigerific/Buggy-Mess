using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Shotgun Specifics")]
    [SerializeField] private int pelletCount = 10;
    [SerializeField] [Range(0f, 0.2f)]private float spreadAmount = 0.1f;
    [SerializeField] private GameObject bulletCasingPrefab;
    [SerializeField] private Transform casingEjectionPoint;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform barrelEnd;
    [SerializeField] private BulletBase bulletPrefab;
    [SerializeField] private Recoil recoil;
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Update()
    {
        if (isReloading) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Fire();
            }
        }
        else
        {
            //Kan addera ett ljud om ammot �r slut, typ ett klick ljud :D
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }

        if (currentAmmo == 0) { Reload(); }
        UpdateAmmoUI();
    }

    public override void Fire()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        // Play muzzle flash
        //muzzleFlash.Play();

        // Fire pellets with spread
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spread = barrelEnd.forward + new Vector3(
                Random.Range(- spreadAmount, spreadAmount),
                Random.Range(- spreadAmount, spreadAmount),
                0f
            );
            ObjectPooling.SpawnObject(bulletPrefab.gameObject, barrelEnd.position, Quaternion.LookRotation(spread), ObjectPooling.PoolType.shotgunBullet);
        }

        // Play fire sound "TODO"
        //RuntimeManager.PlayOneShotAttached(shotgunAudio.fire, gameObject);//TODO - Byt till rätt ljud!

        //Addera screen shake?
        recoil.ApplyRecoil();
    }

    public override void Reload()
    {
        if (isReloading) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        //RuntimeManager.PlayOneShotAttached(weaponAudio.reload, gameObject);//TODO - Byt till rätt ljud!

        // Eject shell
        ObjectPooling.SpawnObject(bulletCasingPrefab.gameObject, casingEjectionPoint.position, casingEjectionPoint.rotation, ObjectPooling.PoolType.bulletCaseObjects);
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

    //Help methods
    private void OnDrawGizmos()
    {
        if (barrelEnd == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(barrelEnd.position, barrelEnd.forward * 5f);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spread = barrelEnd.forward + new Vector3(
                Random.Range(-spreadAmount, spreadAmount),
                Random.Range(-spreadAmount, spreadAmount),
                0f
            );

            // Draw each spread ray
            Gizmos.DrawRay(barrelEnd.position, spread.normalized * 5f);
        }
    }
}
