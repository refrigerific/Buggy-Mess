using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil variables")]
    [SerializeField][Range(-20f, 20f)] private float recoilAmountX = 2f;  // Recoil on the X-axis (Vertical)
    [SerializeField][Range(-20f, 20f)] private float recoilAmountY = 0.5f;  // Recoil on the Y-axis (Horizontal)
    [SerializeField][Range(-20f, 20f)] private float recoilAmountZ = 0.2f;  // Recoil on the Z-axis (Depth)
    [SerializeField] private float recoilSpeed = 10f;
    [SerializeField] private float recoverySpeed = 5f;
    [Header("Knockback variables")]
    [SerializeField] private bool recoilKnockback = false;
    [SerializeField] private float recoilKnockbackAmount = 10f;
    [SerializeField] private float knockbackDuration = 0.5f;
    [Header("Weapon kickback variables")]
    [SerializeField] private bool gunKickback = false;
    [SerializeField][Range(0f, 1f)] private float gunKickbackAmount = 0.2f;
    [SerializeField] private float gunKickbackSpeed = 15f;
    private CharacterController playerController;

    private Vector3 originalRotation;
    private Vector3 originalPosition;
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private Vector3 currentKickback;
    private Vector3 knockbackVelocity;
    private Vector3 knockbackTarget;
    private bool isKnockbackActive = false;
    private float knockbackTimer = 0f;

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
        originalPosition = transform.localPosition;

        playerController = GetComponentInParent<CharacterController>();

        if (playerController == null)
        {
            Debug.LogError("CharacterController not found! Ensure this script is attached to the player or has access to the player.");
        }
    }

    private void Update()
    {
        // Smooth the recoil application
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSpeed);
        transform.localEulerAngles = originalRotation + currentRecoil;

        // Smooth the recoil recovery
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoverySpeed);

        // If gun kickback is enabled, apply the backward movement
        if (gunKickback)
        {
            currentKickback = Vector3.Lerp(currentKickback, Vector3.zero, Time.deltaTime * gunKickbackSpeed);
            transform.localPosition = originalPosition + currentKickback;
        }

        // Handle knockback if active
        if (isKnockbackActive)
        {
            knockbackTimer += Time.deltaTime;

            if (knockbackTimer < knockbackDuration)
            {
                Vector3 smoothedMovement = Vector3.SmoothDamp(Vector3.zero, knockbackTarget, ref knockbackVelocity, knockbackDuration);
                playerController.Move(smoothedMovement * Time.deltaTime);
            }
            else
            {
                isKnockbackActive = false;
                knockbackTimer = 0f;
            }
        }
    }

    public void ApplyRecoil()
    {
        // Apply independent recoil to each axis
        targetRecoil += new Vector3(recoilAmountX, Random.Range(-recoilAmountY, recoilAmountY), recoilAmountZ);

        // If gun kickback is enabled, apply kickback movement on Z-axis
        if (gunKickback)
        {
            currentKickback += new Vector3(0, 0, -gunKickbackAmount);  // Move gun backward
        }

        if (recoilKnockback && playerController != null)
        {
            Vector3 knockbackDirection = -transform.forward;
            knockbackTarget = knockbackDirection * recoilKnockbackAmount;
            isKnockbackActive = true;
            knockbackTimer = 0f;
        }
    }
}
