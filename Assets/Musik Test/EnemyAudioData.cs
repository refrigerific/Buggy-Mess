using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/Enemy", fileName = "New Enemy Sheet")]
public class EnemyAudioData : ScriptableObject
{
    public EventReference hurt;
    public EventReference death;
    public EventReference hurtFeedback;                   //temp - hope it can be in weapon sheet
    public EventReference deathFeedback;                   //temp - hope it can be in weapon sheet
}