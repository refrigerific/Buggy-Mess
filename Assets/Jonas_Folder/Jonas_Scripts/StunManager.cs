using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunManager : MonoBehaviour
{
    [SerializeField] private float stunDuration = 2.0f;
    [Tooltip("Chans att stunna. 0,1 = 10% chans")]
    [SerializeField, Range(0f, 1f)] private float stunChance = 0.2f; // 20% default chance to stun

    private float stunTimer = 0f;
    private bool isStunned = false;

    private IStunnable stunnableEnemy;

    public float StunDuration
    {
        get { return stunDuration; }
        set { stunDuration = value; }
    }

    public float StunChance
    {
        get { return stunChance; }
        set { stunChance = value; }
    }

    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0f)
            {
                EndStun();
            }
        }
    }

    public void Initialize(IStunnable enemy)
    {
        stunnableEnemy = enemy;
    }

    public void ApplyStun(float duration = -1f)
    {
        if (Random.value <= stunChance) // Random.value returns a value between 0 and 1
        {
            if (duration > 0f)
            {
                stunDuration = duration;
            }

            stunTimer = stunDuration;
            isStunned = true;

            if (stunnableEnemy != null)
            {
                stunnableEnemy.SetStunned(true);
            }
        }
    }

    private void EndStun()
    {
        isStunned = false;

        if (stunnableEnemy != null)
        {
            stunnableEnemy.SetStunned(false);
        }
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}
