using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadingScene : MonoBehaviour
{
    // Start is called before the first frame update
     


    public GameObject floatingScene;
    public Image floatingBarFill;
    public bool slow;
    public int optionalSceneLoad;


    public void floatScene(int sceneID)
    {

        StartCoroutine(LoadSceneAsync(sceneID));

    }
    IEnumerator LoadSceneAsync(int sceneID)
    {

        floatingScene.SetActive(true);
        StartCoroutine(GottaWaitFast());



        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        operation.allowSceneActivation = false;

        while (!operation.isDone && !slow)
        {

            float currProgress = Mathf.Clamp01(operation.progress / 0.9f);
            floatingBarFill.fillAmount = currProgress;

            yield return null;

        }

        //new LevelDataTransition(GameManager.Instance.PlayerScript);

        operation.allowSceneActivation = true;



    }
    IEnumerator GottaWaitFast()
    {
        slow = false;
        yield return new WaitForSeconds(5f);
        slow = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            floatScene(optionalSceneLoad);
            if(optionalSceneLoad == 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
    }

}
