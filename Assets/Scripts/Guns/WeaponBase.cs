using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] protected string weaponName;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected float damagePerBullet;

    [Header("References")]
    [SerializeField] protected Animator weaponAnimator;
    //Temp
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip fireClip;
    [SerializeField] protected AudioClip reloadClip;
    //

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float nextFireTime = 0f;

    //Kan ha flera specifika metoder
    public abstract void Fire();
    public abstract void Reload();

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }
}
