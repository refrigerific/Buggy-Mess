using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MusicInitSettings : MonoBehaviour
{
    [NonReorderable] public TriggerSettings[] audioOptions;
    

    private void Start()                        //Works like audiotriggers, but when the object starts (usually when the scene starts)
    {
        for (int i = 0; i < audioOptions.Length; i++)
        {
            audioOptions[i].paramID = MusicManager.Instance.FillParamID(audioOptions[i]);
            MusicManager.Instance.TriggerMusic(audioOptions[i]);
        }
    }
}
