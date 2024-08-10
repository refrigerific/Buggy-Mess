using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MusicTrigger : MonoBehaviour
{
    public string targetTag;
    [NonReorderable] public TriggerSettings[] audioOptions;

    private void Start()
    {
        for (int i = 0; i < audioOptions.Length; i++)
        {
            audioOptions[i].paramID = MusicManager.Instance.FillParamID(audioOptions[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag)
        {
            foreach (TriggerSettings t in audioOptions)
            {
                MusicManager.Instance.TriggerMusic(t);
            }
        }
    }
}
