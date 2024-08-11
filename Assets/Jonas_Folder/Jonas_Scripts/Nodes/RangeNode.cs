using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range;
    private Transform target;
    private Transform origin;
    private Animator animator;

    public RangeNode(float range, Transform target, Transform origin, Animator animator)
    {
        this.range = range;
        this.target = target;
        this.origin = origin;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(target.position, origin.position);

        //if(distance < range)
        //{
        //    if (!AudioGameFunctions.Instance.enemiesInCombatList.Contains(origin.gameObject)) { AudioGameFunctions.Instance.enemiesInCombatList.Add(origin.gameObject); }
            
        //    //animator.SetBool("IsRunning", true);//kommentera bort om du vill prova slugboi
        //}
        //else
        //{
        //    AudioGameFunctions.Instance.enemiesInCombatList.Remove(origin.gameObject);
        //}
        return distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;
    }   
}
