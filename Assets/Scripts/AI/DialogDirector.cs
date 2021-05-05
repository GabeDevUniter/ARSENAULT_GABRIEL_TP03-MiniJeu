using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The brain of the dialogs. It stores a set of dialogs and provides a
/// dialog when asked
/// </summary>
public class DialogDirector : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField]
    private float volume = 0.6f;

    [SerializeField]
    private float spatialBlend = 0.7f;

    [SerializeField]
    private float maxDistance = 12f;

    [Header("Dialog Settings")]
    [SerializeField]
    private float cooldownMin = 0f;

    [SerializeField]
    private float cooldownMax = 0f;

    [SerializeField]
    private bool startDelay = true;

    private AudioSource[] dialogs;

    private bool isWaiting = false;

    private void Awake()
    {
        dialogs = GetComponents<AudioSource>();

        // Tweaking the audio settings of every dialog at once. Saves the trouble of doing them all manually.
        foreach(AudioSource dialog in dialogs)
        {
            dialog.volume = volume;

            dialog.spatialBlend = spatialBlend;

            dialog.rolloffMode = AudioRolloffMode.Linear;

            dialog.maxDistance = maxDistance;
        }

        // Removing the disabled audio sources
        List<AudioSource> tmpDialogs = new List<AudioSource>();
        
        for(int i = 0; i < dialogs.Length; i++)
        {
            if (dialogs[i].enabled) tmpDialogs.Add(dialogs[i]);
        }

        dialogs = tmpDialogs.ToArray();
        //

        if (startDelay || !isWaiting) StartCoroutine(Delay(Random.Range(cooldownMin, cooldownMax)));
    }

    public void MuteAll()
    {
        StopAllCoroutines();

        foreach (AudioSource dialog in dialogs) dialog.Stop();

        isWaiting = false;
    }

    /// <summary>
    /// Plays a dialog after muting the ones currently playing.
    /// </summary>
    public void Play()
    {
        if (isWaiting || dialogs.Length == 0) return;

        MuteAll();

        StartCoroutine(PlayAudio(Shuffle()));
    }

    /// <summary>
    /// Plays the dialog, then starts the cooldown immediately at the end
    /// </summary>
    /// <param name="dialog"></param>
    private IEnumerator PlayAudio(AudioSource dialog)
    {
        isWaiting = true;

        dialog.Play();

        yield return new WaitForSeconds(dialog.clip.length);

        StartCoroutine(Delay(Random.Range(cooldownMin, cooldownMax)));
    }

    /// <summary>
    /// Delay before playing another dialog
    /// </summary>
    /// <param name="duration">Delay time</param>
    private IEnumerator Delay(float duration)
    {
        isWaiting = true;

        yield return new WaitForSeconds(duration);

        isWaiting = false;
    }


    private List<AudioSource> pickedDialogs = new List<AudioSource>();

    /// <summary>
    /// Returns a random audio clip from the dialog set without taking the same one twice.
    /// </summary>
    /// <returns>Picked Dialog</returns>
    private AudioSource Shuffle()
    {
        List<AudioSource> tempDialogs = new List<AudioSource>();

        if (pickedDialogs.Count == dialogs.Length) pickedDialogs.Clear();

        foreach(AudioSource dialog in dialogs)
        {
            bool isPicked = false;

            foreach(AudioSource pickedDialog in pickedDialogs)
            {
                if(dialog == pickedDialog)
                {
                    isPicked = true;
                    break;
                }
            }

            if (!isPicked) tempDialogs.Add(dialog);
        }

        AudioSource chosenDialog = tempDialogs[Random.Range(0, tempDialogs.Count)];

        pickedDialogs.Add(chosenDialog);

        return chosenDialog;
    }
}
