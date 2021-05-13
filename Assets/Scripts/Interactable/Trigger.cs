using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum DoorOutput { Open, Close, Toggle }

public enum MusicOutput { Play, Stop }

public abstract class Triggerable : MonoBehaviour { }

public class Trigger : MonoBehaviour
{
    public bool triggerOnce = true;

    public float delay = 0f;

    public Triggerable target; // Main target, which is any script inheriting from Triggerable

    // Door parameters
    [HideInInspector]
    public DoorOutput doorOutput;

    [HideInInspector]
    public bool doorLockOnMove;

    [HideInInspector]
    public bool doorForceMove; // Door will move wether it's locked or not
    //

    // Music parameters
    [HideInInspector]
    public MusicOutput musicOutput;

    [HideInInspector]
    public SongNames musicToPlay;
    //

    // Private variables
    private bool canTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setTrigger();
        }
    }

    public void setTrigger()
    {
        if (!canTrigger || target == null) return;

        StartCoroutine(setTriggerRoutine());
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
        else if(target.GetType() == typeof(MusicDirector))
        {
            var music = (MusicDirector)target;

            switch(musicOutput)
            {
                case MusicOutput.Play: music.Play(musicToPlay); break;

                case MusicOutput.Stop: music.Stop(); break;
            }
        }

        Debug.Log("TRIGGERED!");

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
            else if(trigger.target.GetType() == typeof(MusicDirector))
            {
                trigger.musicOutput = (MusicOutput)EditorGUILayout.EnumPopup("Output", trigger.musicOutput);

                trigger.musicToPlay = (SongNames)EditorGUILayout.EnumPopup("Play", trigger.musicToPlay);
            }
            
        }
    }
}