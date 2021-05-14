using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum MixerAudio { Music, SFX, Dialog}

public class VolumeChanger : MonoBehaviour
{
    [SerializeField]
    private AudioMixer AudioMixer;

    [SerializeField]
    private MixerAudio mixerAudio;

    private Slider slider;

    private Toggle toggle;

    private float currentAudio { get { return slider.value; } }

    private string audioName;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        audioName = System.Enum.GetName(typeof(MixerAudio), mixerAudio);

        AudioMixer.GetFloat(audioName, out float currentValue);

        slider.value = currentValue;

        slider.onValueChanged.AddListener(SetVolume);

        toggle = GetComponentInChildren<Toggle>();

        toggle.onValueChanged.AddListener(ToggleVolume);
    }

    void SetVolume(float value)
    {
        if(toggle.isOn) AudioMixer.SetFloat(audioName, value);
    }

    void ToggleVolume(bool value)
    {
        AudioMixer.SetFloat(audioName, value ? currentAudio : -80f);
    }
}
