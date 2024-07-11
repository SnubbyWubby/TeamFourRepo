using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TackleBox.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] string PlayScene;

        public void play()
        {
            if (!string.IsNullOrEmpty(PlayScene))
                SceneManager.LoadScene(PlayScene);


        }

        //public void options()
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

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