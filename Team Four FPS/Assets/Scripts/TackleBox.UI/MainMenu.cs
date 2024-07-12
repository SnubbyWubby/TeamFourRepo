using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

namespace TackleBox.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("<=====VIDEO_GAME_MENUS=====>")]

        [SerializeField] string PlayScene;
        public string playNewGame;
        private string loadCurrGame;

        [SerializeField] private GameObject noFileFound = null; 

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
                noFileFound.SetActive(false); 
            }
        }

        public void options()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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