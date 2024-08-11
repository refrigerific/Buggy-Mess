using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeNode : Node
{
    private NavMeshAgent agent;
    private PillbugAI pillbugAI;
    private GameObject meleeZonePrefab;
    private FieldOfView fov;
    private Animator animator;
    private float attackCooldown;
    private float attackTime;


    private bool isAttacking = false;
    private float currentAttackTime = 0f;
    private float currentCooldownTime = 0f;

    public MeleeNode(NavMeshAgent agent, PillbugAI pillbugAI, GameObject meleeZonePrefab, FieldOfView fov, float attackCooldown, float attackTime, Animator animator)
    {
        this.agent = agent;
        this.pillbugAI = pillbugAI;
        this.meleeZonePrefab = meleeZonePrefab;
        this.fov = fov;
        this.attackCooldown = attackCooldown;
        this.attackTime = attackTime;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {        
        if (fov.SeesPlayer)
        {
            agent.isStopped = true;
            pillbugAI.SetColor(Color.green);
            animator.SetBool("IsRunning", false);
            if (!isAttacking && currentCooldownTime <= 0f)
            {
                StartAttack();
            }

            if (isAttacking)
            {
                UpdateAttackTimer();
            }

            if (currentCooldownTime > 0f)
            {
                UpdateCooldown();
            }
        
            return NodeState.RUNNING;
        }
        else
        {
            agent.isStopped = false;
            return NodeState.FAILURE;
        }
    }
    
    private void StartAttack()
    {
        isAttacking = true;
        //animator.SetTrigger("AttackTrigger");
        currentAttackTime = 0f;
        meleeZonePrefab.SetActive(true);       
    }

    private void UpdateAttackTimer()
    {
        currentAttackTime += Time.deltaTime;

        if (currentAttackTime >= attackTime)
        {
            EndAttack();
        }
    }

    private void EndAttack()
    {
        meleeZonePrefab.SetActive(false);
        isAttacking = false;
        currentCooldownTime = attackCooldown;
    }

    private void UpdateCooldown()
    {
        currentCooldownTime -= Time.deltaTime;

        if (currentCooldownTime <= 0f)
        {
            currentCooldownTime = 0f;
        }
    }
   
}
