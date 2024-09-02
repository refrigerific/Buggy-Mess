using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunManager : MonoBehaviour
{
    [SerializeField] private float stunDuration = 2.0f;

    private float stunTimer = 0f;
    private bool isStunned = false;

    private IStunnable stunnableEnemy;

    public float StunDuration
    {
        get { return stunDuration; }
        set { stunDuration = value; }
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
        if (duration > 0f)
        {
            stunDuration = duration;
        }

        stunTimer = stunDuration;
        isStunned = true;
        Debug.Log($"Gets stunned for {stunDuration} seconds!");

        if (stunnableEnemy != null)
        {
            stunnableEnemy.SetStunned(true);
        }
    }

    private void EndStun()
    {
        Debug.Log("Stun over!");
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
