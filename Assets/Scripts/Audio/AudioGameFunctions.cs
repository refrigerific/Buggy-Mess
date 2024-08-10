using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioGameFunctions : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;
    [Space(40)] 
    
    public Button mainMenuButton;
    public TriggerSettings menuButtonPressed;
    [Space(40)]
    
    //public RuntimeSetSO combatList;
    public float combatWaitEndTimer = 4f;
    public int combatants;
    [Space (40)] 
    
    public TriggerSettings enterCombat;
    public TriggerSettings exitCombat;
    private bool combatState;
    private bool lastCombatState;

    private void Update()
    {
        //combatants = combatList.Count;

        /*if (combatList.Count > 0)
            combatState = true;
        else
            combatState = false;
        */
        if (combatState != lastCombatState)
            SetCombatMusic(combatState);

        lastCombatState = combatState;
    }
    
    private void Start()
    {
        enterCombat.paramID = MusicManager.Instance.FillParamID(enterCombat);
        exitCombat.paramID = MusicManager.Instance.FillParamID(exitCombat);
        SceneManager.activeSceneChanged += ClearCombatList;
    }

    

    public void SetPaused()
    {
        //Audiomanager.Instance.GetCurrentMusic().setPaused(true);
    }

    public void SetUnpaused()
    {
        //Audiomanager.Instance.GetCurrentMusic().setPaused(false);
    }

    public void SetMusicVolume()
    {

        MusicManager.Instance.SetVolume("Music", musicSlider.value);
    }

    public void SetSFXVolume()
    {
        MusicManager.Instance.SetVolume("SFX", sfxSlider.value);
    }

    public void SetMasterVolume()
    {
        MusicManager.Instance.SetVolume("Master", masterSlider.value);
    }

    public void SetCombatMusic(bool combatState)
    {
        //Debug.Log(combatState);
        switch (combatState)
        {
            case true:
                MusicManager.Instance.TriggerMusic(enterCombat);
                break;
            
            case false:
                StartCoroutine(CombatWaitTimer());
                //Audiomanager.Instance.TriggerMusic(exitCombat);
                break;
        }
    }

    private IEnumerator CombatWaitTimer()
    {
        yield return new WaitForSeconds(combatWaitEndTimer);
        
        if(!combatState)
            MusicManager.Instance.TriggerMusic(exitCombat);
    }

    public void ClearCombatList(Scene current, Scene next)
    {
        //combatList.objects.Clear();
    }

    public void MenuButtonMusicTrigger()
    {
        MusicManager.Instance.TriggerMusic(menuButtonPressed);
    }
}
