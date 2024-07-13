using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumePara = "MasterVolume";
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 20f;
    [SerializeField] Toggle toggle;
    bool disableToggleEvent;


    // Start is called before the first frame update
    void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
        toggle.onValueChanged.AddListener(HandleToggleValueChanged);
    }

    private void HandleToggleValueChanged(bool enableSound)
    {
        if (disableToggleEvent)
        {
            return;
        }
        if (enableSound) 
        {
            slider.value = .01f;
        }
        else
        {
            slider.value = .99f; 
        }
    }

    private void HandleSliderValueChanged(float value)
    {
        audioMixer.SetFloat(volumePara, Mathf.Log10(value) * multiplier);
        
        disableToggleEvent = true;
        toggle.isOn = slider.value > .01f;
        disableToggleEvent = false;
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumePara, slider.value);
    }
    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumePara, slider.value);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
