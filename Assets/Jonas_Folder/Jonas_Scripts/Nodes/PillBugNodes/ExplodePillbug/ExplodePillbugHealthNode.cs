using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePillbugHealthNode : Node
{
    private ExplodingPillBugAI explodingPillbugAI;
    private Health health;

    public ExplodePillbugHealthNode(ExplodingPillBugAI explodingPillbugAI, Health health)
    {
        this.explodingPillbugAI = explodingPillbugAI;
        this.health = health;
    }

    public override NodeState Evaluate()
    {
        if (health.GetCurrentHealth() <= health.GetThresholdHealth())
        {
            explodingPillbugAI.IsRunningAway = true;
        }
        else
        {
            explodingPillbugAI.IsRunningAway = false;
        }

        return health.GetCurrentHealth() <= health.GetThresholdHealth() ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
