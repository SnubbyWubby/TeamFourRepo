using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.UI;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Main Menu");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

        }  
    }
}
