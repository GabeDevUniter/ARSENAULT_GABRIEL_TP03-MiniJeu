using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Names of the game's songs. The names must match with the name of the MusicPlayer's GameObject
public enum SongNames { Prelude, InAction, LevelEnd}

/// <summary>
/// Main director for the music players. It stores every MusicPlayer in the
/// scene and decides who should play and who should stop
/// </summary>
public class MusicDirector : Triggerable
{

    // Dictionary containing the MusicPlayer scripts which can be accessed by their
    // GameObjects' name
    private Dictionary<string, MusicPlayer> songs = new Dictionary<string, MusicPlayer>();

    void Awake()
    {
        MusicPlayer[] players = GetComponentsInChildren<MusicPlayer>();

        foreach(MusicPlayer player in players)
        {
            songs.Add(player.gameObject.name, player);
        }
    }

    public void Play(SongNames name)
    {
        Play(System.Enum.GetName(typeof(SongNames), name));
    }

    public void Play(string name)
    {
        Stop();
        
        songs[name].Play();
    }

    public void Stop()
    {
        foreach(MusicPlayer song in songs.Values)
        {
            song.Stop();
        }
    }
}
