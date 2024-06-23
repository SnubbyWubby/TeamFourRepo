using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] GameObject grenadeIcon;

    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text grenadeCountText;
    public Image playerHPBar;

    public GameObject Player;
    public playerController PlayerScript;

    public bool isPaused;
    public bool inGrenadeRadius;

    int enemyCount;
    int grenadeCount;

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
                showGrenadeWarning();
            }

        }

        if (!isPaused)
        {
            showGrenadeWarning();
        }
    }

    public void statePause()
    {
        grenadeIcon.SetActive(false);
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
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
        if(enemyCount <= 0)
        {
            statePause();
            MenuActive = menuWin;
            MenuActive.SetActive(isPaused);
        }
    }
    public void GameLoss()
    {
        statePause();
        MenuActive = menuLose;
        MenuActive.SetActive(isPaused);
    }

    public void updateGrenadeCount(int amount)
    {
        grenadeCount += amount;
        grenadeCountText.text = grenadeCount.ToString("F0");
    }

    public void showGrenadeWarning()
    {
        grenadeIcon.SetActive(inGrenadeRadius);
    }
}
