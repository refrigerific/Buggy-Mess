using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PillBugChaseNode : Node
{
    private Transform target;
    private NavMeshAgent agent;
    private PillbugAI pillbugAI;

    public PillBugChaseNode(Transform target, NavMeshAgent agent, PillbugAI pillbugAI)
    {
        this.target = target;
        this.agent = agent;
        this.pillbugAI = pillbugAI;
    }

    
    public override NodeState Evaluate()
    {
        pillbugAI.SetColor(Color.yellow);
        float distance = Vector3.Distance(target.position, agent.transform.position);

        if (distance > 0.2f)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = true;
            return NodeState.SUCCESS;
        }
    }

    //TODO: Kan addera ett annat beteende hur Pillbugsen rör sig mot spelaren!
}
