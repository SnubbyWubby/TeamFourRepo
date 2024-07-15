using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    public GameObject floatingScene;
    public Image floatingBarFill;
    public bool slow;

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
        
        operation.allowSceneActivation = true;



    }
    IEnumerator GottaWaitFast()
    {
        slow = false;
        yield return new WaitForSeconds(5f);
        slow = true;    
    }
}
