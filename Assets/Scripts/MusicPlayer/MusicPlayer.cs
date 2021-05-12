using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private bool startPlay = false;

    [Header("Song/Entry")]
    [SerializeField]
    private AudioSource soundtrack;

    [Header("Loop")]
    [SerializeField]
    private AudioSource[] segments;

    void Awake()
    {
        if (soundtrack == null) soundtrack = GetComponent<AudioSource>();

        if(soundtrack != null)
        {
            soundtrack.loop = false;
            soundtrack.playOnAwake = false;
        }

        foreach(AudioSource segment in segments)
        {
            segment.loop = false;
            segment.playOnAwake = false;
        }

        if (startPlay) Play();
    }

    public void Play()
    {
        Stop();

        StartCoroutine(PlaySong());
    }

    public void Stop()
    {
        StopAllCoroutines();

        soundtrack.Stop();

        foreach (AudioSource segment in segments) segment.Stop();
    }

    IEnumerator PlaySong()
    {
        if(soundtrack != null)
        {
            soundtrack.Play();

            Debug.Log($"Now playing: {soundtrack.clip.name}");

            yield return new WaitForSeconds(soundtrack.clip.length);
        }

        while(segments.Length > 0)
        {
            foreach(AudioSource segment in segments)
            {
                segment.Play();

                Debug.Log($"Now playing: {segment.clip.name}");

                yield return new WaitForSeconds(segment.clip.length);
            }
        }
    }
}
