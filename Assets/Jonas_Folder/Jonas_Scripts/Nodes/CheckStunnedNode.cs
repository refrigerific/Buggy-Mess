using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStunnedNode : Node
{
    private StunManager stunManager;

    public CheckStunnedNode(StunManager stunManager)
    {
        this.stunManager = stunManager;
    }

    public override NodeState Evaluate()
    {
        if (stunManager.IsStunned())
        {
            return NodeState.FAILURE; // If stunned, fail the node
        }
        return NodeState.SUCCESS; // If not stunned, succeed and continue
    }
}
