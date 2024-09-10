using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask grappleableLayer;
    [SerializeField] private KeyCode grappleButton;
    [Header("Grapple variables")]
    [SerializeField] private float maxGrappleDistance = 100f;
    [SerializeField] private float grappleSpeed = 20f;
    [SerializeField] private float grappleStopDistance = 2.0f;
    [SerializeField] private float sideMoveSpeed = 15; // Air control during grapple
    [SerializeField][Range(0,2)] private float grappleBoostDeceleration = 0.5f;

    private Vector3 grapplePoint;
    private Vector3 grappleDirection;
    private bool isGrappling = false;
    public bool IsGrappling => isGrappling;
    private float verticalSpeed = 0.0f;
    private void Start()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleButton))
        {
            StartGrapple();
        }
        else if (Input.GetKeyUp(grappleButton))
        {
            StopGrapple();
        }
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxGrappleDistance, grappleableLayer))
        {
            grapplePoint = hit.point;
            grappleDirection = (grapplePoint - transform.position).normalized;
            isGrappling = true;

            // Enable the line renderer and set its initial positions
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    public void GrappleMovement()
    {
        if (!isGrappling) return;

        // Calculate the direction towards the grapple point
        Vector3 directionToGrapple = (grapplePoint - transform.position).normalized;

        // Calculate horizontal movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 horizontalMovement = transform.right * moveHorizontal;

        // Combine forward grapple movement with side movement
        Vector3 grappleMovement = directionToGrapple * grappleSpeed * Time.deltaTime;
        Vector3 totalMovement = grappleMovement + (horizontalMovement * sideMoveSpeed * Time.deltaTime);

        // Move the player using CharacterController
        playerController.Move(totalMovement);

        // Update the line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);

        // Check the distance to the grapple point
        if (Vector3.Distance(transform.position, grapplePoint) <= grappleStopDistance)
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        if (!isGrappling) return;

        // Get the player's current facing direction
        Vector3 facingDirection = transform.forward;

        // Calculate the player's velocity at the end of the grapple
        Vector3 momentum = facingDirection * grappleSpeed;

        // Apply momentum boost in the facing direction
        ApplyFinalBoost(momentum);

        // Stop grappling
        isGrappling = false;
        lineRenderer.enabled = false;
    }

    private void ApplyFinalBoost(Vector3 boost)
    {
        float decelerationDuration = grappleBoostDeceleration; // Duration for deceleration (adjust to your preference)
        StartCoroutine(SmoothMomentumAfterBoost(boost, decelerationDuration));
    }

    private IEnumerator SmoothMomentumAfterBoost(Vector3 boost, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Move the player using the boost
            playerController.Move(boost * Time.deltaTime);

            // Gradually reduce the boost over time
            boost = Vector3.Lerp(boost, Vector3.zero, elapsedTime / duration);

            // Apply final movement considering gravity and boost
            Vector3 finalMovement = boost + Vector3.up * verticalSpeed;
            playerController.Move(finalMovement * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
