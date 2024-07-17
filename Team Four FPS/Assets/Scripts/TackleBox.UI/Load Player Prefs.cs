using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using TackleBox.UI;

public class LoadPlayerPrefs : MonoBehaviour
{
    [Header("<=====PLAYER_PREFS_MENU=====>")]

    [SerializeField] private MainMenu menuScript; 

    [SerializeField] private TMP_Text volTextValue = null;
    [SerializeField] private TMP_Text ctrlSenTextValue = null; 
    [SerializeField] private TMP_Text brightTextValue = null;
    [SerializeField] private TMP_Dropdown qtyDropDDown;

    [SerializeField] private Slider volSlider = null;  
    [SerializeField] private Slider ctrlSenSlider = null; 
    [SerializeField] private Slider brightSlider = null;

    [SerializeField] private Toggle invertY = null; 
    [SerializeField] private Toggle fullScreen = null; 

    [SerializeField] private bool truUser = false;

    // Awake is called before the first frame update
    private void Awake()
    {
        if (truUser) 
        {
            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                float volLocal = PlayerPrefs.GetFloat("MasterVolume");
                volTextValue.text = volLocal.ToString("0.0");
                volSlider.value = volLocal; 
                AudioListener.volume = volLocal;
            }
            else 
            {
                menuScript.ResetButton("Audio");
            }

            if (PlayerPrefs.HasKey("MasterQuality")) 
            {
                int qtyLocal = PlayerPrefs.GetInt("MasterQuality");
                qtyDropDDown.value = qtyLocal;
                QualitySettings.SetQualityLevel(qtyLocal); 
            }

            if (PlayerPrefs.HasKey("MasterFullScreen")) 
            {
                int fulScrLocal = PlayerPrefs.GetInt("MasterFullScreen");

                if (fulScrLocal == 1)
                {
                    Screen.fullScreen = true;
                    fullScreen.isOn = true;
                }
                else 
                { 
                    Screen.fullScreen = false;
                    fullScreen.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("MasterBrightness")) 
            {
                float brgLocal = PlayerPrefs.GetFloat("MasterBrightness");
                brightTextValue.text = brgLocal.ToString("0.0");
                brightSlider.value = brgLocal;
            }

            if (PlayerPrefs.HasKey("MasterSensitivity")) 
            {
                float styLocal = PlayerPrefs.GetFloat("MasterSensitivity");
                ctrlSenTextValue.text = styLocal.ToString("0.0");
                ctrlSenSlider.value = styLocal;
                //menuScript.mainControlSen = Mathf.RoundToInt(styLocal);
            }

            if (PlayerPrefs.HasKey("MasterInvertY")) 
            {
                if (PlayerPrefs.GetFloat("MasterInvertY") == 1.0f) 
                { 
                    invertY.isOn = true;
                }
                else
                {
                    invertY.isOn = false;
                }
            }
        }
    }
}