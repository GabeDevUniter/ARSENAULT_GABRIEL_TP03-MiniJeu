using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private bool startPlay = false;

    [SerializeField]
    private float fadeIn = 0.3f;

    [SerializeField]
    private float fadeOut = 0.3f;

    [Header("Song/Entry")]
    private AudioSource soundtrack;

    [Header("Loop")]
    private AudioSource[] segments;

    void Awake()
    {
        soundtrack = GetComponent<AudioSource>();

        if(soundtrack != null)
        {
            soundtrack.loop = false;
            soundtrack.playOnAwake = false;
        }

        segments = GetComponentsInChildren<AudioSource>();
        

        // Remove the entry soundtrack from the segments array
        List<AudioSource> temp = new List<AudioSource>();

        foreach (AudioSource segment in segments)
        {
            segment.loop = false;
            segment.playOnAwake = false;

            if (segment.gameObject != gameObject) temp.Add(segment);
        }

        segments = temp.ToArray();
        //

        if (startPlay) Play();
    }

    #region Play

    public void Play()
    {
        StopAllCoroutines();

        stopAudio();

        StartCoroutine(PlaySong());
    }

    IEnumerator PlaySong()
    {
        StartCoroutine(LerpVolume(0, 1, fadeIn));

        if (soundtrack != null && soundtrack.clip != null)
        {
            soundtrack.Play();

            //Debug.Log($"Now playing: {soundtrack.clip.name}");

            while (soundtrack.time != soundtrack.clip.length) yield return null;
        }

        while (segments.Length > 0)
        {
            foreach (AudioSource segment in segments)
            {
                segment.Play();

                //Debug.Log($"Now playing: {segment.clip.name}");

                while (segment.time != segment.clip.length) yield return null;
            }
        }
    }

    #endregion

    #region Stop

    public void Stop()
    {
        StopAllCoroutines();

        StartCoroutine(StopRoutine());
    }
    
    IEnumerator StopRoutine()
    {
        StartCoroutine(LerpVolume(1, 0, fadeOut));

        yield return new WaitForSeconds(fadeOut);

        stopAudio();
    }

    private void stopAudio()
    {
        if (soundtrack != null) soundtrack.Stop();

        foreach (AudioSource segment in segments) segment.Stop();
    }

    #endregion

    #region Volume

    IEnumerator LerpVolume(float initial, float final, float duration)
    {
        float elapsed = 0f;

        setVolume(initial);

        while(elapsed < duration)
        {
            float lerp = Mathf.Lerp(initial, final, elapsed / duration);

            setVolume(lerp);

            elapsed += Time.deltaTime;

            yield return null;
        }

        setVolume(final);
    }

    private void setVolume(float volume)
    {
        if (soundtrack != null) soundtrack.volume = volume;

        foreach (AudioSource segment in segments) segment.volume = volume;
    }

    #endregion
}
