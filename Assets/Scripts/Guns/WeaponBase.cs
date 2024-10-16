using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] protected string weaponName;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadDuration;

    [Header("References")]
    [SerializeField] protected Animator weaponAnimator;

    [Header("Audio settings")]
    public WeaponAudioData weaponAudio;

    protected int currentAmmo;
    protected bool isReloading = false;
    protected float nextFireTime = 0f;

    //Kan ha flera specifika metoder
    public virtual void Fire()
    {
        //RuntimeManager.PlayOneShotAttached(fire, gameObject);
    }
    public virtual void Reload()
    {
        //RuntimeManager.PlayOneShotAttached(reload, gameObject);
    }

    protected virtual void Start()
    {
        currentAmmo = maxAmmo;
    }
}
