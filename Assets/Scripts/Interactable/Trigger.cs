﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum TriggerCondition { Collider, Interaction, LevelEnd, GameOver }

public enum DoorOutput { Open, Close, Toggle }

public enum MusicOutput { Play, Stop }

public abstract class Triggerable : MonoBehaviour { }

public class Trigger : MonoBehaviour
{
    #region Public Fields

    [Header("Trigger Settings")]
    public bool triggerOnce = true;

    public float delay = 0f;

    public TriggerCondition condition;

    [Header("Target Output")]
    public Triggerable target; // Main target, which is any script inheriting from Triggerable

    #endregion

    #region Door Parameters

    [HideInInspector]
    public DoorOutput doorOutput;

    [HideInInspector]
    public bool doorLockOnMove;

    [HideInInspector]
    public bool doorForceMove; // Door will move wether it's locked or not

    #endregion

    #region Music parameters

    [HideInInspector]
    public MusicOutput musicOutput;

    [HideInInspector]
    public SongNames musicToPlay;

    #endregion

    #region Private variables

    private bool canTrigger = true;

    static private Dictionary<TriggerCondition, List<Trigger>> globalTriggers = new Dictionary<TriggerCondition, List<Trigger>>()
    { 
        { TriggerCondition.LevelEnd, new List<Trigger>() },
        { TriggerCondition.GameOver, new List<Trigger>() }
    };

    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Triggers with global conditions will be added to the static dictionary
        if (condition == TriggerCondition.LevelEnd || condition == TriggerCondition.GameOver)
            globalTriggers[condition].Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setTrigger(TriggerCondition.Collider);
        }
    }
    #endregion

    public void setTrigger(TriggerCondition condition)
    {
        if (!canTrigger || target == null || this.condition != condition) return;

        StartCoroutine(setTriggerRoutine());
    }

    static public void setTriggerGlobal(TriggerCondition condition)
    {
        foreach (Trigger trigger in globalTriggers[condition])
            trigger.setTrigger(condition);
    }

    IEnumerator setTriggerRoutine()
    {
        yield return new WaitForSeconds(delay);

        if (target.GetType() == typeof(Door))
        {
            var door = (Door)target;

            if (doorForceMove) door.locked = false;

            switch (doorOutput)
            {
                case DoorOutput.Open: door.Open(); break;

                case DoorOutput.Close: door.Close(); break;

                case DoorOutput.Toggle: door.Toggle(); break;
            }

            door.locked = doorLockOnMove;
        }
        else if (target.GetType() == typeof(MusicDirector))
        {
            var music = (MusicDirector)target;

            switch (musicOutput)
            {
                case MusicOutput.Play: music.Play(musicToPlay); break;

                case MusicOutput.Stop: music.Stop(); break;
            }
        }

        //Debug.Log("TRIGGERED!");

        canTrigger = !triggerOnce;
    }
}

[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var trigger = target as Trigger;

        if (trigger.target != null)
        {
            //Debug.Log(trigger.target.GetType());

            if (trigger.target.GetType() == typeof(Door))
            {
                trigger.doorOutput = (DoorOutput)EditorGUILayout.EnumPopup("Output", trigger.doorOutput);

                trigger.doorLockOnMove = EditorGUILayout.Toggle("Lock On Move", trigger.doorLockOnMove);

                trigger.doorForceMove = EditorGUILayout.Toggle("Force Move", trigger.doorForceMove);
            }
            else if (trigger.target.GetType() == typeof(MusicDirector))
            {
                trigger.musicOutput = (MusicOutput)EditorGUILayout.EnumPopup("Output", trigger.musicOutput);

                if(trigger.musicOutput == MusicOutput.Play)
                    trigger.musicToPlay = (SongNames)EditorGUILayout.EnumPopup("Play", trigger.musicToPlay);
            }

        }
    }
}