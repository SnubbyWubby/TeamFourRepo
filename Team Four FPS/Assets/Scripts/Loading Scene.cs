using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Level;
using TackleBox.SaveSystem;
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
    public GameObject objective;



    public void floatScene(int sceneID)
    {

        StartCoroutine(LoadSceneAsync(sceneID));
       

    }
    IEnumerator LoadSceneAsync(int sceneID)
    {
        if (objective != null)
            objective.SetActive(false);
        floatingScene.SetActive(true);
        StartCoroutine(GottaWaitFast());

        if (FindObjectOfType<GameManager>() != null)
        {
            GameManager.Instance.ignorePlayer = true;
            LevelDataTransition.Instance.savePlayerStats(GameManager.Instance.PlayerScript);
        }
            
        float currProgress = 0;


        

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        operation.allowSceneActivation = false;

        while (!operation.isDone && !slow)
        {
            currProgress = Mathf.Lerp(currProgress, operation.progress /.9f, .25f);
            
            floatingBarFill.fillAmount = currProgress;

            yield return new WaitForSeconds(.25f);

        }
        if (operation.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneID));
        }
        if (FindObjectOfType<GameManager>() != null)
            GameManager.Instance.ignorePlayer = false;

        floatingBarFill.fillAmount = 1;
        yield return new WaitForSeconds(1f);

        operation.allowSceneActivation = true;

    }
    IEnumerator GottaWaitFast()
    {
        slow = false;
        yield return new WaitForSeconds(3f);
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
