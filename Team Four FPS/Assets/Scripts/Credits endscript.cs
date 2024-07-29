using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;

public class Creditsendscript : MonoBehaviour
{
     [SerializeField] GameObject uiThing;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        uiThing.SetActive(false);
        Cursor.visible = true;
        GameManager.Instance.ignorePlayer = true;

    }


}