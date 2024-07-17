using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;
using TackleBox.Audio;

namespace TackleBox.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("<=====COMPONENTS=====>")]

        [SerializeField] string PlayScene;
        [SerializeField] private GameObject noFileFound = null;
        [SerializeField] private GameObject confirmPrompt = null;
        [SerializeField] string BackgroundMusic = "Main Menu";

        [Header("<=====OPTIONS_MENU=====>")]

        [SerializeField] private TMP_Text volTextValue = null;
        [SerializeField] private TMP_Text brightTextValue = null; 
        [SerializeField] private Slider volSlider = null;
        [SerializeField] private Slider brightSlider = null; 
        [SerializeField] private float volDefault = 1.0f;
        [SerializeField] private float brtDefault = 1.0f; 

        [SerializeField] private TMP_Text ctrlSenTextValue = null; 
        [SerializeField] private Slider ctrlSenSlider = null;
        [SerializeField] private Toggle invertYToggle = null;
        [SerializeField] private Toggle fullScrnToggle; 
        [SerializeField] private TMP_Dropdown drpdwnQuality;
        [SerializeField] private float senDefault = 0.5f;

        public float mainControlSen = 0.5f;
        public TMP_Dropdown drpdwnResolution;

        private Resolution[] scrResolution;   
        private string loadCurrGame;
        private int levelQuality; 
        private float lvlBrightness;
        private bool truFullScreen;

        private void Start() 
        {
            scrResolution = Screen.resolutions; 
            drpdwnResolution.ClearOptions();

            List<string> options = new List<string>();
            int currIdxResolution = 0;

            for (int num = 0; num < scrResolution.Length; num++) 
            { 
                string resOption = scrResolution[num].width + "X" + scrResolution[num].height; 
                options.Add(resOption);

                if (scrResolution[num].width == Screen.width && scrResolution[num].height == Screen.height) 
                {
                    currIdxResolution = num;    
                }
            }

            AudioManager.Instance.GetMusicByID(BackgroundMusic).PlayMusic();

            drpdwnResolution.AddOptions(options);
            drpdwnResolution.value = currIdxResolution;
            drpdwnResolution.RefreshShownValue(); 
        }

        public void SetResolution(int resIndex) 
        {
            Resolution resolution = scrResolution[resIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); 
        }

        public void play()
        {
            if (!string.IsNullOrEmpty(PlayScene))
                SceneManager.LoadScene(PlayScene);
        }

        public void LoadGame()  
        {
            if (PlayerPrefs.HasKey("LevelSaved"))
            {
                loadCurrGame = PlayerPrefs.GetString("LevelSaved");

                SceneManager.LoadScene(loadCurrGame); 
            }
            else
            {
                noFileFound.SetActive(true); 
            }
        }

        public void options() {}

        public void SetVolume(float volume) 
        {
            AudioListener.volume = volume; 
            volTextValue.text = volume.ToString("0.00"); 
        }

        public void ApplyVolume() 
        {
            PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
            StartCoroutine(SaveBoxConfirm());  
        }

        public void SetControlSensitivity(float sensitivity) 
        {
            mainControlSen = sensitivity; 
            ctrlSenTextValue.text = sensitivity.ToString("0.00");
        }

        public void ApplyGamePlay() 
        {
            if (invertYToggle.isOn) 
            {
                PlayerPrefs.SetFloat("MasterInvertY", 1.0f); // Invert Y Controller Button
            }
            else
            {
                PlayerPrefs.SetFloat("MasterInvertY", 0.0f); // Did Not Invert Y Controller Button
            }

            PlayerPrefs.SetFloat("MasterSensitivity", mainControlSen);
            StartCoroutine(SaveBoxConfirm());
        }

        public void SetBrightness(float bright) 
        {
            lvlBrightness = bright;
            brightTextValue.text = bright.ToString("0.0"); 
        }

        public void SetFullScreen(bool fullscreen) { truFullScreen = fullscreen; }

        public void SetQuality(int qtyIndex) { levelQuality = qtyIndex; }

        public void ApplyGraphics() 
        {
            // Able To Change Your Brightness Level With Your Liking 
            PlayerPrefs.SetFloat("MasterBrightness", lvlBrightness);

            PlayerPrefs.SetInt("MasterQuality", levelQuality);
            QualitySettings.SetQualityLevel(levelQuality);

            PlayerPrefs.SetInt("MasterFullScreen", (truFullScreen ? 1 : 0));
            Screen.fullScreen = truFullScreen;

            StartCoroutine(SaveBoxConfirm()); 
        }

        public void ResetButton(string typeMenu) 
        {
            if (typeMenu == "Audio") 
            { 
                AudioListener.volume = volDefault;
                volSlider.value = volDefault;
                volTextValue.text = volDefault.ToString("0.0"); 
                ApplyVolume();
            }

            if (typeMenu == "GamePlay") 
            {
                ctrlSenTextValue.text = senDefault.ToString("0.0");
                ctrlSenSlider.value = senDefault;
                mainControlSen = senDefault; 
                invertYToggle.isOn = false;
                ApplyGamePlay(); 
            }

            if (typeMenu == "Graphics") 
            {
                // Reset Brightness Level 
                brightSlider.value = brtDefault;
                brightTextValue.text = brtDefault.ToString("0.0");

                drpdwnQuality.value = 1;
                QualitySettings.SetQualityLevel(1);

                fullScrnToggle.isOn = false;
                Screen.fullScreen = false;

                Resolution currResolution = Screen.currentResolution;
                Screen.SetResolution(currResolution.width, currResolution.height, Screen.fullScreen);
                drpdwnResolution.value = scrResolution.Length; 

                ApplyGraphics(); 
            }
        }

        public IEnumerator SaveBoxConfirm()   
        { 
            confirmPrompt.SetActive(true);
            yield return new WaitForSeconds(1);
            confirmPrompt.SetActive(false);
        }

        public void quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}