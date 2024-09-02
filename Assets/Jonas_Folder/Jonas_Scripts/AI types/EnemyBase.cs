using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IStunnable
{
    public abstract void SetStunned(bool stunned);
    public abstract bool IsStunned();
}
