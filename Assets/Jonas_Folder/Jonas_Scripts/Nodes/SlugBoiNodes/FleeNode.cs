using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeNode : Node
{
    private GrenadierAI grenadierAI;
    private NavMeshAgent navMeshAgent;
    private List<Transform> fleeDestinations;
    private bool hasSelectedDestination = false;

    public FleeNode(GrenadierAI enemy, NavMeshAgent agent, List<Transform> fleeDestinations)
    {
        grenadierAI = enemy;
        navMeshAgent = agent;
        this.fleeDestinations = fleeDestinations;
    }

    public override NodeState Evaluate()
    {
        if (fleeDestinations == null || fleeDestinations.Count == 0)
        {
            Debug.LogError("No flee destinations assigned!");
            return NodeState.FAILURE;
        }

        if (!hasSelectedDestination)
        {
            int randomIndex = Random.Range(0, fleeDestinations.Count);
            Transform randomDestination = fleeDestinations[randomIndex];

            navMeshAgent.SetDestination(randomDestination.position);

            grenadierAI.IsRunningAway = true;

            hasSelectedDestination = true;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            hasSelectedDestination = false;
        }

        return NodeState.RUNNING;
    }
}

