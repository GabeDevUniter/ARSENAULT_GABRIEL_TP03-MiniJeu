using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum MixerAudio { Music, SFX, Dialog}

/// <summary>
/// Changing a certain audio mixer group's volume
/// </summary>
public class VolumeChanger : MonoBehaviour
{
    [SerializeField]
    private AudioMixer AudioMixer;

    [SerializeField]
    private MixerAudio mixerAudio;

    private Slider slider;

    private Toggle toggle;

    static Dictionary<MixerAudio, float> CurrentAudio = new Dictionary<MixerAudio, float>();

    static Dictionary<MixerAudio, bool> MuteAudio = new Dictionary<MixerAudio, bool>();

    private string audioName;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        audioName = System.Enum.GetName(typeof(MixerAudio), mixerAudio);

        if(CurrentAudio.ContainsKey(mixerAudio))
        {
            slider.value = CurrentAudio[mixerAudio];
        }
        else
        {
            AudioMixer.GetFloat(audioName, out float currentValue);

            slider.value = currentValue;
        }

        slider.onValueChanged.AddListener(SetVolume);

        toggle = GetComponentInChildren<Toggle>();

        if (MuteAudio.ContainsKey(mixerAudio))
        {
            toggle.isOn = MuteAudio[mixerAudio];
        }

        toggle.onValueChanged.AddListener(ToggleVolume);

        InitializeDictionaries();
    }

    private void Update()
    {
        CurrentAudio[mixerAudio] = slider.value;
        MuteAudio[mixerAudio] = toggle.isOn;
    }

    void InitializeDictionaries()
    {
        if(!CurrentAudio.ContainsKey(mixerAudio))
        {
            CurrentAudio.Add(mixerAudio, slider.value);
        }
        if(!MuteAudio.ContainsKey(mixerAudio))
        {
            MuteAudio.Add(mixerAudio, toggle.isOn);
        }
    }

    void SetVolume(float value)
    {
        if(toggle.isOn) AudioMixer.SetFloat(audioName, value);
    }

    void ToggleVolume(bool value)
    {
        AudioMixer.SetFloat(audioName, value ? CurrentAudio[mixerAudio] : -80f);
    }
}
