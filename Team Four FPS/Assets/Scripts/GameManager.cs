using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("<=====GM_UI_OUTOFBOUNDS=====>")]

    [SerializeField] GameObject OutofBoundsUI;
    [SerializeField] TMP_Text OutofBoundsTimer;
    [SerializeField] GameObject OutofBoundsBackground;

    [Header("<=====GM_UI_GAME_MENUS=====>")]

    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text menuLoseCause;

    [Header("<=====GM_UI_GRENADE=====>")]

    [SerializeField] GameObject grenadeIcon;

    [SerializeField] TMP_Text grenadeCountText;

    public bool inGrenadeRadius;

    [Header("<=====WIN_LOSE_AUDIO=====>")]

    [SerializeField] AudioSource menuAudio; 

    [SerializeField] AudioClip[] winAudio;
    [SerializeField] float winVolume;

    [SerializeField] AudioClip[] loseAudio; 
    [SerializeField] float loseVolume; 

    [Header("<=====GM_UI_PLAYER&ENEMY=====>")]

    public playerController PlayerScript;

    public GameObject Player;

    public Camera MainCamera;

    public Image playerHPBar;
    public Image plrArmorHPBar;
    public GameObject plrArmorHPBack;

    public TMP_Text ammoCurrent, ammoMaximum;

    [SerializeField] TMP_Text enemyCountText;

    public bool isPaused;

    [Header("<=====GM_UI_STOPWATCH=====>")]
    [SerializeField] TMP_Text StopwatchCurr;
    [SerializeField] TMP_Text StopwatchBest;
    bool stopWatchActive;
    float currentTime;

    int enemyCount;
    int grenadeCount;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        stopWatchActive = true;
        Player = GameObject.FindWithTag("Player");
        PlayerScript = Player.GetComponent<playerController>();
        MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
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

        if (stopWatchActive)
        {
            currentTime += Time.deltaTime;
            StopwatchCurr.text = timerConvertor(currentTime);
        }

    }

    public void statePause()
    {
        grenadeIcon.SetActive(false);
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        stopWatchActive = false;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        MenuActive.SetActive(isPaused);
        MenuActive = null;
        stopWatchActive = true;
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

            menuAudio.PlayOneShot(winAudio[Random.Range(0, winAudio.Length)], winVolume); 
        }
    }

    public void GameLoss()
    {
        statePause();
        MenuActive = menuLose;
        MenuActive.SetActive(isPaused);

        menuAudio.PlayOneShot(loseAudio[Random.Range(0, loseAudio.Length)], loseVolume); 
    }

    public void GameLoss(string cause)
    {
        menuLoseCause.text = cause;
        GameLoss();
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

    public void OutofBoundsToggle(bool state)
    {
        OutofBoundsUI.SetActive(state);
    }

    public void updateOutofBoundsDisplay(float timeRemaining)
    {
        OutofBoundsTimer.text = timerConvertor(timeRemaining);
    }

    public string timerConvertor(float currTime)
    {
        int seconds = Mathf.FloorToInt(currTime);
        int milliseconds = Mathf.FloorToInt((currTime - seconds) * 100);
        return string.Format("{0:00}:{1:00}:{2:00}", seconds / 60, seconds % 60, milliseconds);
    }
}