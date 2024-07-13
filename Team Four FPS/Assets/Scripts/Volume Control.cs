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


    // Start is called before the first frame update
    void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }
   

    private void HandleSliderValueChanged(float value)
    {
        audioMixer.SetFloat(volumePara, Mathf.Log10(value) * multiplier);
        if (slider.value == 1)
        {
            audioMixer.SetFloat(volumePara, 20f);
        }
        else if (slider.value == 0)
        {
            audioMixer.SetFloat(volumePara, -80f);
        }
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
