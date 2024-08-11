using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNode : Node
{
    private BeetleAI ai;
    private Health health;
    private float healthRestoreRate;

    public HealthNode(BeetleAI ai, Health health, float healthRestoreRate)
    {
        this.ai = ai;
        this.health = health;
        this.healthRestoreRate = healthRestoreRate;
    }

    public override NodeState Evaluate()
    {
        if(health.GetCurrentHealth() <= health.GetThresholdHealth())
        {
            ai.IsRunningAway = true;
        }
        else
        {
            ai.IsRunningAway = false;
        }

        return health.GetCurrentHealth() <= health.GetThresholdHealth() ? NodeState.SUCCESS : NodeState.FAILURE;
    }

}
