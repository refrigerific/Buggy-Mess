using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToCoverNode : Node
{
    private NavMeshAgent agent;
    private BeetleAI ai;

    public GoToCoverNode(NavMeshAgent agent, BeetleAI ai)
    {
        this.agent = agent;
        this.ai = ai;
    }

    public override NodeState Evaluate()
    {
        Transform coverSpot = ai.GetBestCoverSpot();

        if(coverSpot == null)
        {
            Debug.Log("Hittar ingen cover!");
            return NodeState.FAILURE;
        }

        ai.SetColor(Color.blue);
        float distance = Vector3.Distance(coverSpot.position, agent.transform.position);

        if (distance > 0.2f)
        {
            agent.isStopped = false;
            agent.SetDestination(coverSpot.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
