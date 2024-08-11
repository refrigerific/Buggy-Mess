using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBugHealthNode : Node
{
    private PillbugAI pillbugAI;
    private Health health;

    public PillBugHealthNode(PillbugAI pillbugAI, Health health)
    {
        this.pillbugAI = pillbugAI;
        this.health = health;
    }
   
    public override NodeState Evaluate()
    {
        if (health.GetCurrentHealth() <= health.GetThresholdHealth())
        {
            pillbugAI.IsRunningAway = true;
        }
        else
        {
            pillbugAI.IsRunningAway = false;
        }

        return health.GetCurrentHealth() <= health.GetThresholdHealth() ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
