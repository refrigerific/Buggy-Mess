using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilSpeed = 10f;
    [SerializeField] private float recoverySpeed = 5f;

    private Vector3 originalRotation;
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSpeed);

        transform.localEulerAngles = originalRotation + currentRecoil;

        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoverySpeed);
    }

    public void ApplyRecoil()
    {
        targetRecoil += new Vector3(0, 0, recoilAmount);
    }
}
