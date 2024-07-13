using System;
using System.Collections;
using System.Collections.Generic;
using TackleBox.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TackleBox.Audio
{
    public class VolumeControl : MonoBehaviour
    {
        [SerializeField] string volumePara = "MasterVolume";
        [SerializeField] Slider slider;
        [SerializeField] float multiplier = 20f;
        [SerializeField] Toggle toggle;
        [SerializeField] GameObject musicPreview;
        bool disableToggleEvent;

        public void HandleToggleValueChanged(bool enableSound)
        {
            if (disableToggleEvent)
                return;

            if (slider != null)
                slider.value = enableSound ? .05f : .95f;

            if (enableSound)
                slider.value = .05f;
            else
                slider.value = .95f;

        }

        public void HandleSliderValueChanged(float value)
        {
            float volue = Mathf.Clamp(Mathf.Log10(value)* multiplier, -80, 110);
            AudioManager.Instance.AudioMixer.SetFloat(volumePara, volue);
            
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
    }
}