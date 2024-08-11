using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBoiShootNode : Node
{
    private GrenadierAI grenadierAI;
    private GameObject projectilePrefab;
    private Transform shootingPoint;
    private float fireDelay;

    private bool canFire = true;
    private float elapsedTime = 0;

    public TailBoiShootNode(GrenadierAI grenadierAI, Transform shootingPoint, GameObject projectilePrefab, float fireDelay)
    {
        this.grenadierAI = grenadierAI;
        this.shootingPoint = shootingPoint;     
        this.projectilePrefab = projectilePrefab;
        this.fireDelay = fireDelay;        
    }

    public override NodeState Evaluate()
    {
        if(grenadierAI.IsRunningAway == true)
        {
            return NodeState.FAILURE;
        }

        if (canFire)
        {
            BezierAttack();
            canFire = false;
            elapsedTime = 0f;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= fireDelay)
            {
                canFire = true;
            }
        }
        return NodeState.RUNNING;
    }

    private void BezierAttack()
    {
        //TODO: ObjectPoola!
        GameObject projectile = Object.Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {

        }
        else
        {
            Debug.LogError("Projectile rigidbody not found!");
        }
    }

}
