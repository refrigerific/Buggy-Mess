using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPillBugRangeNode : Node
{
    private float range;
    private Transform target;
    private Transform origin;
    private ExplodingPillBugAI explodingPillBugAi;

    public ExplodingPillBugRangeNode(float range, Transform target, Transform origin, ExplodingPillBugAI explodingPillBugAI)
    {
        this.range = range;
        this.target = target;
        this.origin = origin;
        this.explodingPillBugAi = explodingPillBugAI;
    }

    public override NodeState Evaluate()
    {
        //Ful lösning
        if (explodingPillBugAi.HasMovedOnes == true)
        {
            range = explodingPillBugAi.AttackRange;
        }

        Debug.Log("Range node " + range);
        float distance = Vector3.Distance(target.position, origin.position);

        //if (distance < range)
        //{
        //    if (!AudioGameFunctions.Instance.enemiesInCombatList.Contains(origin.gameObject)) { AudioGameFunctions.Instance.enemiesInCombatList.Add(origin.gameObject); }

        //}
        //else
        //{
        //    AudioGameFunctions.Instance.enemiesInCombatList.Remove(origin.gameObject);
        //}

        return distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
