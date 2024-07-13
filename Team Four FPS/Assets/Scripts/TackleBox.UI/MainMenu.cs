using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro; 

namespace TackleBox.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("<=====COMPONENTS=====>")]

        [SerializeField] string PlayScene;
        [SerializeField] private GameObject noFileFound = null;
        [SerializeField] private GameObject confirmPrompt = null;
        [SerializeField] private GameObject promptConfirm1 = null; 

        [SerializeField] AudioSource menuAudio;

        [Header("<=====OPTIONS_MENU=====>")]

        [SerializeField] private TMP_Text volTextValue = null; 
        [SerializeField] private Slider volSlider = null;
        [SerializeField] private float volDefault = 1.0f;

        [SerializeField] private TMP_Text ctrlSenTextValue = null; 
        [SerializeField] private Slider ctrlSenSlider = null;
        [SerializeField] private Toggle invertYToggle = null; 
        [SerializeField] private float senDefault = 0.5f;
        public float mainControlSen = 0.5f; 

        private string loadCurrGame;

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
            PlayerPrefs.SetFloat("Master-Volume", AudioListener.volume);
            StartCoroutine(SaveBox1Confirm());  
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
                PlayerPrefs.SetFloat("Master-InvertY", 1.0f); // Invert Y Controller Button
            }
            else
            {
                PlayerPrefs.SetFloat("Master-InvertY", 0.0f); // Did Not Invert Y Controller Button
            }

            PlayerPrefs.SetFloat("Master-Sensitivity", mainControlSen);
            StartCoroutine(SaveBox2Confirm());
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
        }

        public IEnumerator SaveBox1Confirm()   
        {
            confirmPrompt.SetActive(true);
            yield return new WaitForSeconds(1);
            confirmPrompt.SetActive(false);
        }

        public IEnumerator SaveBox2Confirm() 
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