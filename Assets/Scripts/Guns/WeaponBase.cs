using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] public string weaponName;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadDuration;

    [Header("References")]
    [SerializeField] protected Animator weaponAnimator;

    [Header("Audio settings")]
    public WeaponAudioData weaponAudio;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float reloadEndTime = 0f; // Time when reloading completes
    protected float nextFireTime = 0f;

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }

    protected virtual void Update()
    {
        if (isReloading && Time.time >= reloadEndTime)
        {
            FinishReload();
        }
    }

    public virtual void Fire()
    {
        if (!isReloading && Time.time >= nextFireTime && currentAmmo > 0)
        {
            currentAmmo--;
            nextFireTime = Time.time + fireRate;
            Debug.Log($"{weaponName} fired. Ammo left: {currentAmmo}");
            // Play fire sound or animation here
        }
    }

    public virtual void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            reloadEndTime = Time.time + reloadDuration;
            Debug.Log($"{weaponName} is reloading...");
            // Play reload sound or animation here
        }
    }

    public void CancelReload()
    {
        if (isReloading)
        {
            isReloading = false;
            // Optionally, stop any reload animation or sound here
            Debug.Log($"{weaponName} reload canceled.");
        }
    }

    private void FinishReload()
    {
        isReloading = false;
        currentAmmo = maxAmmo;
        Debug.Log($"{weaponName} reloaded.");
        // End reload sound or animation here
    }
}
