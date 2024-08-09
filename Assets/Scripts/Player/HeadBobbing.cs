using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [SerializeField] private float bobFrequency = 1.5f; // Frequency of the bobbing
    [SerializeField] private float bobHeight = 0.1f;    // Height of the bobbing
    [SerializeField] private float bobSwayAngle = 0.5f; // Angle of the sway
    [SerializeField] private float smoothing = 5.0f;    // Smoothing factor for when the player stops moving

    private float timer = 0.0f;
    private Vector3 startPosition;
    private bool isMoving;
    private Vector3 currentBobPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        isMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        if (isMoving)
        {
            timer += Time.deltaTime * bobFrequency;

            float xPos = startPosition.x + Mathf.Sin(timer) * bobSwayAngle;
            float yPos = startPosition.y + Mathf.Abs(Mathf.Sin(timer)) * bobHeight;

            currentBobPosition = new Vector3(xPos, yPos, startPosition.z);
        }
        else
        {
            timer = 0.0f;

            currentBobPosition = Vector3.Lerp(currentBobPosition, startPosition, Time.deltaTime * smoothing);
        }

        transform.localPosition = currentBobPosition;
    }
}
