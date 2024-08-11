using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PillBugThresholdNode : Node
{
    private NavMeshAgent agent;
    private float speedIncreaseThreshold = 0.5f;
    private float movementSpeedIncrease;
    public PillBugThresholdNode(NavMeshAgent agent, float movementSpeedIncrease)
    {
        this.agent = agent;
        this.movementSpeedIncrease = movementSpeedIncrease;
    }

    public override NodeState Evaluate()
    {
        if (agent.velocity.magnitude >= speedIncreaseThreshold)
        {
            IncreaseStats();
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    private void IncreaseStats()
    {
        agent.speed = movementSpeedIncrease;
    }
}
