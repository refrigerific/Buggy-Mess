using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExplodeNode : Node
{
    private NavMeshAgent agent;
    private ExplodingPillBugAI explodingPillbugAI;
    private GameObject explosionZonePrefab;
    private FieldOfView fov;
    private Transform target;
    private BezierProjectileMovement bezierProjectile;
    private float attackTime;
    private float attackRange;
    private Vector3 explosionSize;

    private bool isAttacking = false;
    private float currentAttackTime = 0f;
    private float currentCooldownTime = 0f;

    public ExplodeNode(NavMeshAgent agent, ExplodingPillBugAI explodingPillbugAI, GameObject explosionZonePrefab, FieldOfView fov, float attackTime, Transform target, float attackRange, Vector3 explosionSize, BezierProjectileMovement bezierProjectile)
    {
        this.agent = agent;
        this.explodingPillbugAI = explodingPillbugAI;
        this.explosionZonePrefab = explosionZonePrefab;
        this.fov = fov;
        this.attackTime = attackTime;
        this.target = target;
        this.attackRange = attackRange;
        this.explosionSize = explosionSize;
        this.bezierProjectile = bezierProjectile;
    }

    public override NodeState Evaluate()
    {
        explodingPillbugAI.HasMovedOnes = true;
        explodingPillbugAI.SetColor(Color.green);
        if (!isAttacking && currentCooldownTime <= 0f)
        {
            StartAttack();
        }

        if (isAttacking)
        {
            UpdateAttackTimer();
        }

        if (explodingPillbugAI.HasMovedOnes == true)
        {
            explodingPillbugAI.AttackRange = 100;
            attackRange = 100;
            Debug.Log(attackRange);
        }

        return NodeState.SUCCESS;
    }

    private void StartAttack()
    {
        agent.isStopped = true;
        explosionZonePrefab.SetActive(true);
        isAttacking = true;
        currentAttackTime = 0f;
        explosionZonePrefab.transform.localScale = Vector3.zero;
    }

    private void UpdateAttackTimer()
    {
        currentAttackTime += Time.deltaTime;

        if (currentAttackTime <= attackTime)
        {
            float scaleFactor = currentAttackTime / attackTime;
            explosionZonePrefab.transform.localScale = Vector3.Lerp(Vector3.zero, explosionSize, scaleFactor);
        }
        else
        {
            EndAttack();
        }
    }

    private void EndAttack()
    {
        if (explodingPillbugAI.IsSpawned == true)
        {
            bezierProjectile.gameObject.SetActive(false);
        }
        explodingPillbugAI.gameObject.SetActive(false);
        explosionZonePrefab.SetActive(false);
        isAttacking = false;

        //AudioGameFunctions.Instance.enemiesInCombatList.Remove(explodingPillbugAI.gameObject);
    }
}
