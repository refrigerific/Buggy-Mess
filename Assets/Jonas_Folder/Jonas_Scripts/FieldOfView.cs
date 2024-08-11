using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform detectionPoint;
    [SerializeField] [Range(10, 100)] private float fieldOfViewAngle = 90f;
    [SerializeField] private float viewDistance = 10f;

    [SerializeField] private bool seesPlayer;

    public bool SeesPlayer { get { return seesPlayer; } }


    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            seesPlayer = true;            
        }
        else
        {
            seesPlayer = false;         
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fieldOfViewAngle * 0.5f)
        {
            // Check if player is within view distance
            RaycastHit hit;
            if (Physics.Raycast(detectionPoint.transform.position, direction, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {         
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(detectionPoint.transform.position, viewDistance);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle / 2, detectionPoint.transform.up) * detectionPoint.transform.forward * viewDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle / 2, detectionPoint.transform.up) * detectionPoint.transform.forward * viewDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(detectionPoint.transform.position, detectionPoint.transform.position + fovLine1);
        Gizmos.DrawLine(detectionPoint.transform.position, detectionPoint.transform.position + fovLine2);
        Gizmos.DrawLine(detectionPoint.transform.position + fovLine1, detectionPoint.transform.position + fovLine2);
    }
}
