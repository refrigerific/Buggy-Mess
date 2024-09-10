using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilSpeed = 10f;
    [SerializeField] private float recoverySpeed = 5f;
    [SerializeField] private bool recoilKnockback = false;
    [SerializeField] private float recoilKnockbackAmount = 10f;
    [SerializeField] private float knockbackDuration = 0.5f;
    [SerializeField] private CharacterController playerController;

    private Vector3 originalRotation;
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    private Vector3 knockbackVelocity;
    private Vector3 knockbackTarget;
    private bool isKnockbackActive = false;
    private float knockbackTimer = 0f;

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSpeed);
        transform.localEulerAngles = originalRotation + currentRecoil;
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoverySpeed);

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
        targetRecoil += new Vector3(0, 0, recoilAmount);

        if (recoilKnockback && playerController != null)
        {
            Vector3 knockbackDirection = -transform.forward;
            knockbackTarget = knockbackDirection * recoilKnockbackAmount;
            isKnockbackActive = true;
            knockbackTimer = 0f;
        }
    }
}
