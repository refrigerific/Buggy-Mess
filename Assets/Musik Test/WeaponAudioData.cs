using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/Weapon", fileName = "New Weapon Sheet")]
public class WeaponAudioData : ScriptableObject
{
    public EventReference fire;
    public EventReference reload;
    public EventReference surfaceHit;                   //what if all different surface hits were in one fmod event, and a parameter sent to it made it play the sound of the correct surface
}
