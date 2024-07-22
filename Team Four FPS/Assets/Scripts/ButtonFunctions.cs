using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TackleBox.SaveSystem;

namespace TackleBox
{
    public class buttonFunctions : MonoBehaviour
    {
        public void resume()
        {
            GameManager.Instance.stateUnpause();
        }

        public void restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameManager.Instance.stateUnpause();
        }

        public void mainmenu()
        {
            SceneManager.LoadScene("Main Menu");
        }
       

        public void quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public void nextLevel()
        {
            int nextScene = GameManager.Instance.currentSceneID++;
            SceneManager.LoadScene(nextScene);
        }
    }
}