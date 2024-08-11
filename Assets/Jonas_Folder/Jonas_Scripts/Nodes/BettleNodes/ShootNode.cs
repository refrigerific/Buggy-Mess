using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootNode : Node
{
    private NavMeshAgent agent;
    private BeetleAI ai;
    private Transform player;
    private GameObject projectilePrefab;
    private Transform shootingPoint;
    private FieldOfView fov;
    private float shootingForce;
    private float fireDelay;
    private float spreadAngle;

    private bool canFire = true;
    private float elapsedTime = 0;

    public ShootNode(NavMeshAgent agent, BeetleAI ai, Transform target, GameObject projectilePrefab, Transform shootingPoint, float shootingForce, float fireDelay, float spreadAngle, FieldOfView fov)
    {
        this.agent = agent;
        this.ai = ai;
        player = target;
        this.projectilePrefab = projectilePrefab;
        this.shootingPoint = shootingPoint;
        this.shootingForce = shootingForce;
        this.fireDelay = fireDelay;
        this.spreadAngle = spreadAngle;
        this.fov = fov;
    }

    public override NodeState Evaluate()
    {
        if (fov.SeesPlayer)
        {
            agent.isStopped = true;
            ai.SetColor(Color.green);
            if (canFire)
            {
                Shoot();
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
        else if(!fov.SeesPlayer)
        {
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }

    void Shoot()
    {
        //TODO: Objectpoola senare!
        //Also hör om vi kör rigidbody movement för bullets!       

        Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);

        GameObject projectile = Object.Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation * spreadRotation);

        Vector3 direction = (player.transform.position - shootingPoint.position).normalized;


        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            //projectileRigidbody.AddForce(direction * shootingForce, ForceMode.Force);
            projectileRigidbody.AddForce(direction * shootingForce, ForceMode.Impulse);
            projectileRigidbody.velocity = projectile.transform.forward * shootingForce;
        }
        else
        {
            Debug.LogError("Projectile rigidbody not found!");
        }
    }
}
