using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private Node topNode;
    private bool isRunning;

    public BehaviorTree(Node topNode)
    {
        this.topNode = topNode;
    }

    public void Start()
    {
        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
    }

    public void Evaluate()
    {
        if (isRunning && topNode != null)
        {
            topNode.Evaluate();
        }
    }
}
