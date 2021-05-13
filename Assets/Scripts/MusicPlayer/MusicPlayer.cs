using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private bool startPlay = false;

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
        
        List<AudioSource> temp = new List<AudioSource>();

        foreach (AudioSource segment in segments)
        {
            segment.loop = false;
            segment.playOnAwake = false;

            if (segment.gameObject != gameObject) temp.Add(segment);
        }

        segments = temp.ToArray(); // Remove the entry soundtrack from the segments array

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

        if(soundtrack != null) soundtrack.Stop();

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
