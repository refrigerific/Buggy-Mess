using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public enum TriggerTypes
{
    Play,
    Stop,
    StopAll,
    TogglePlay,
    TogglePause,
    SetParameter, 
    SetGlobalParameter,
    LoadBank,
    UnloadBank,
    ActivateObject,
    DeactivateObject
}

[Serializable] public struct MusicTrack     
{
    public string name;
    public EventInstance instance;
}

[Serializable] public struct TriggerSettings           //exists in audiotriggers and audioinitsettings. fill in relevant info in editor (relevant, for example "eventName", "paramName" and "paramValue" when triggerType is SetParameter)
{
    public TriggerTypes triggerType;
    public EventReference eventName;
    [BankRef] public string bankName;
    public string paramName;
    public float paramValue;
    public bool ignoreSeekSpeed;
    public GameObject activationObject;
    public PARAMETER_ID paramID;
}

public class MusicManager: MonoBehaviour
{
    
    [Header("Characters")]
    //public PlayerAudio playerAudio;
    //public EnemyAudio enemyAudio;
    
    public static MusicManager Instance;
    public List<MusicTrack> musicTracks = new List<MusicTrack>(); //list of all music currently loaded (added when banks are loaded, don't add things manually). can be made public to see in editor
    public List<EventReference> currentMusic = new List<EventReference>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerMusic(TriggerSettings opt)          //MAIN METHOD
    {
        switch (opt.triggerType)
        {
            case TriggerTypes.Play:
                PlayMusic(opt.eventName);
                break;
            case TriggerTypes.Stop:
                StopMusic(opt.eventName, true);
                break;
            case TriggerTypes.StopAll:
                StopAllMusic();
                break;
            case TriggerTypes.TogglePlay:
                ToggleMusic(opt.eventName);
                break;
            case TriggerTypes.TogglePause:
                TogglePause(opt.eventName);
                break;
            case TriggerTypes.SetParameter:
                SetParameterByID(opt.eventName, opt.paramID, opt.paramValue, false);
                //SetParameterByName(opt.eventName, opt.paramName, opt.paramValue, false);
                break;
            case TriggerTypes.SetGlobalParameter:
                SetGlobalParameter(opt.paramID, opt.paramValue, opt.ignoreSeekSpeed);
                break;
            case TriggerTypes.LoadBank:               //Only music in loaded banks can be manipulated
                LoadBank(opt.bankName,true);
                break;
            case TriggerTypes.UnloadBank:
                LoadBank(opt.bankName,false);
                break;
            case TriggerTypes.ActivateObject:       //Intended for audiotriggers, but technically works with any objects
                ActivateObject(opt.activationObject, true);
                break;
            case TriggerTypes.DeactivateObject:
                ActivateObject(opt.activationObject, false);
                break;
            default:
                break;
        }
    }

    #region TriggerMusicMethods
    private void PlayMusic(EventReference r)
    {
        EventInstance i = GetMusicInstance(r);
        i.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
            return;
        
        GetMusicInstance(r).start();
        currentMusic.Add(r);
    }

    private void StopMusic(EventReference r, bool clearFromList)
    {
        EventInstance i = GetMusicInstance(r);
        i.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            return;
        
        i.stop(STOP_MODE.ALLOWFADEOUT);
        if (clearFromList)
            currentMusic.Remove(r);
    }

    private void StopAllMusic()
    {
        foreach (EventReference e in currentMusic)
        {
            StopMusic(e, false);
        }
        currentMusic.Clear();
    }

    private void ToggleMusic(EventReference r)
    {
        EventInstance i = GetMusicInstance(r);
        i.getPlaybackState(out PLAYBACK_STATE state);
        
        if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
            i.stop(STOP_MODE.ALLOWFADEOUT);
        
        else if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            i.start();
    }

    private void TogglePause(EventReference r)
    {
        EventInstance i = GetMusicInstance(r);
        i.getPaused(out bool paused);
        i.setPaused(!paused);
    }

    private void SetParameterByName(EventReference r, string paramName, float paramValue, bool ignoreSeekSpeed)             //unused, but keep it in case it gets a purpose later
    {
        GetMusicInstance(r).setParameterByName(paramName, paramValue, ignoreSeekSpeed);
    }

    private void SetParameterByID(EventReference r, PARAMETER_ID id, float paramValue, bool ignoreSeekSpeed)
    {
        GetMusicInstance(r).setParameterByID(id, paramValue, ignoreSeekSpeed);
    }

    private void SetGlobalParameter(PARAMETER_ID id, float paramValue, bool ignoreSeekSpeed)
    {
        RuntimeManager.StudioSystem.setParameterByID(id, paramValue, ignoreSeekSpeed);
    }
    
    
    private void ActivateObject(GameObject target, bool wantToActivate)
    {
        if (target.activeSelf == wantToActivate)
        {
            Debug.Log(target + " is already active/inactive");
            return;
        }
        
        target.SetActive(wantToActivate);
    }
    
    private void LoadBank(string bankName, bool wantToLoad)         //also adds all FMODevents in the bank to the musicTracks list
    {
        switch (wantToLoad)
        {
            case (true):
                if (RuntimeManager.HasBankLoaded(bankName))
                {
                    Debug.Log("bank has already loaded");
                    return;
                }
                
                RuntimeManager.LoadBank(bankName);
                RuntimeManager.StudioSystem.getBank("bank:/" + bankName, out Bank bankToLoad);                  //before, had to be "bank:/Music/" + etc...
                
                bankToLoad.getEventList(out EventDescription[] eventDescriptionsToLoad);
                foreach (EventDescription d in eventDescriptionsToLoad)
                {
                    d.getID(out GUID id);
                    d.getPath(out string path);
                    EventReference r = new EventReference();
                    r.Guid = id;
                    r.Path = path;
                    AddMusicTrack(r);
                }
                break;
            case (false):
                
                if (!RuntimeManager.HasBankLoaded(bankName))
                {
                    Debug.Log("bank has already unloaded");
                    return;
                }
                
                RuntimeManager.StudioSystem.getBank("bank:/" + bankName, out Bank bankToUnload);             //before, had to be "bank:/Music/" + etc...
                bankToUnload.getEventList(out EventDescription[] eventDescriptionsToUnload);
                
                foreach (EventDescription d in eventDescriptionsToUnload)
                {
                    d.getID(out GUID id);
                    d.getPath(out string path);
                    EventReference r = new EventReference();
                    r.Guid = id;
                    r.Path = path;
                    
                    RemoveMusicTrack(r);
                }
                
                RuntimeManager.UnloadBank(bankName);
                break;
        }
    }
    #endregion

    #region HelpMethods
    private void AddMusicTrack(EventReference r)
    {
        if (CheckDuplicateMusic(r))
            return;

        MusicTrack m;
        m.name = r.ToString();
        m.instance = RuntimeManager.CreateInstance(r);
        musicTracks.Add(m);
    }
    
    private void RemoveMusicTrack(EventReference r)
    {
        if (!CheckDuplicateMusic(r))
            return;

        foreach (MusicTrack m in musicTracks)
        {
            if (m.name == r.ToString())
            {
                m.instance.release();
                musicTracks.Remove(m);
                return;
            }
        }
    }
    private bool CheckDuplicateMusic(EventReference r)
    {
        foreach (MusicTrack m in musicTracks)
        {
            if (m.name == r.ToString())
                return true;
        }
        return false;
    }
    private EventInstance GetMusicInstance(EventReference r)
    {
        foreach (MusicTrack m in musicTracks)
        {
            if (m.name == r.ToString())
                 return m.instance;
        }

        return new EventInstance();
    }

    public PARAMETER_ID FillParamID(TriggerSettings opt)         //setting a parameter with id instead of name is apparenlty better :/
    {
        if (opt.triggerType == TriggerTypes.SetParameter)
        {
            foreach (MusicTrack m in musicTracks)
            {
                if (m.name == opt.eventName.ToString())
                {
                    m.instance.getDescription(out EventDescription eDescription);
                    eDescription.getParameterDescriptionByName(opt.paramName, out PARAMETER_DESCRIPTION pDescription);
                    opt.paramID = pDescription.id;
                    return opt.paramID;
                }
            }
        }
        else if (opt.triggerType == TriggerTypes.SetGlobalParameter)
        {
            RuntimeManager.StudioSystem.getParameterDescriptionByName(opt.paramName, out PARAMETER_DESCRIPTION d);
            opt.paramID = d.id;
        }
        return opt.paramID;
    }
    
    

    public void SetVolume(string VCAName, float volume)
    {
        RuntimeManager.StudioSystem.getVCA("vca:/" + VCAName, out VCA vca);
        vca.setVolume(volume);
    }
    #endregion
}
