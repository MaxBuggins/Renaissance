using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UI_Volume : UI_Base
{
    public AudioMixerGroup audioGroup;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        float value;
        audioGroup.audioMixer.GetFloat(audioGroup.name, out value);
        slider.value = value;
    }

    public void UpdateValue(float value)
    {
        audioGroup.audioMixer.SetFloat(audioGroup.name, value);
    }
}
