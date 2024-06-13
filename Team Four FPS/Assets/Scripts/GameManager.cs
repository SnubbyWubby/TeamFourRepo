using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text enemyCountText;

    public GameObject Player;
    public playerController PlayerScript;

    public bool isPaused;

    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Player = GameObject.FindWithTag("Player");
        PlayerScript = Player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (MenuActive == null)
            {
                statePause();
                MenuActive = MenuPause;
                MenuActive.SetActive(isPaused);
            }
            else if (MenuActive == MenuPause)
            {
                stateUnpause();
            }

        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MenuActive.SetActive(isPaused);
        MenuActive = null;
    }

}
