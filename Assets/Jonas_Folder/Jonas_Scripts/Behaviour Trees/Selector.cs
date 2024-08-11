using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    protected List<Node> nodes = new List<Node>();

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (var node in nodes)
        {
            switch (node.Evaluate())
            {
                case NodeState.RUNNING:
                    nodestate = NodeState.RUNNING;
                    return nodestate;
                case NodeState.SUCCESS:
                    nodestate = NodeState.SUCCESS;
                    return nodestate;
                case NodeState.FAILURE:
                    break;
                default:
                    break;
            }
        }
        nodestate = NodeState.FAILURE;
        return nodestate;
    }
}
