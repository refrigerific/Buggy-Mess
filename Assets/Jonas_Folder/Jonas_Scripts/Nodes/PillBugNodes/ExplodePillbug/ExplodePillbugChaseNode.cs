using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodePillbugChaseNode : Node
{
    private Transform target;
    private NavMeshAgent agent;
    private ExplodingPillBugAI explodingPillbugAI;

    public ExplodePillbugChaseNode(Transform target, NavMeshAgent agent, ExplodingPillBugAI explodingPillbugAI)
    {
        this.target = target;
        this.agent = agent;
        this.explodingPillbugAI = explodingPillbugAI;
    }


    public override NodeState Evaluate()
    {

        if (!explodingPillbugAI.HasMovedOnes)
        {
            explodingPillbugAI.SetColor(Color.yellow);
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
                explodingPillbugAI.HasMovedOnes = true;
                return NodeState.SUCCESS;
            }
        }
        else
        {
            return NodeState.SUCCESS;
        }
    }

    //TODO: Kan addera ett annat beteende hur Pillbugsen rör sig mot spelaren!
}
