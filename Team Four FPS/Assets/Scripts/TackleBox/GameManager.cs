using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TackleBox.SaveSystem;
using TackleBox.Audio;
using UnityEngine.TextCore.Text;
using TackleBox.Level;

namespace TackleBox
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        [Header("<=====GM_UI_OUTOFBOUNDS=====>")]

        [SerializeField] GameObject OutofBoundsUI;
        [SerializeField] GameObject OutofBoundsBackground;

        [SerializeField] TMP_Text OutofBoundsTimer;

        [Header("<=====GM_UI_GAME_MENUS=====>")]

        [SerializeField] GameObject MenuActive;
        [SerializeField] GameObject MenuPause;
        [SerializeField] GameObject menuWin;
        [SerializeField] GameObject menuLose;
        [SerializeField] GameObject audioMenu;

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
        public GameObject plrArmorHPBack;

        public Camera MainCamera;

        public Image playerHPBar;
        public Image plrArmorHPBar;

        public TMP_Text ammoCurrent, ammoMaximum;
        [SerializeField] TMP_Text enemyCountText;

        public bool isPaused;
        public bool spawnMoreEnemies;
        public bool playerShot;

        public int roundNumber = 0;
        
        [Header("<=====DEATH CAMERA=====>")]

        public bool stopCameraRotation;
        public bool diedOnce;
        public bool impCamera;
        public bool lerpEnded;
        public bool ignoreMovement;

        Vector3 camStart;
        public Vector3 camEnd;

        [Header("<=====GM_UI_STOPWATCH=====>")]

        [SerializeField] TMP_Text StopwatchCurr;
        [SerializeField] TMP_Text StopwatchBest;

        public bool roundTransition;
        bool stopWatchActive;
        
        public int enemyCount;
        public int grenadeCount;
        public int roundPassed;

        float currentTime;

        [Header("Music:")]
        [SerializeField] string BackgroundMusic = "Gameplay 1";

        // Awake is called before the first frame update
        void Awake()
        {
            _instance = Instance;
            
            stopWatchActive = true;
            Player = GameObject.FindWithTag("Player");
            PlayerScript = Player.GetComponent<playerController>();
            MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            enemyCount = 0;
            Time.timeScale = 1;

            // Load data
            SaveData data = SaveManager.Instance.Load("savefile.json");
            StopwatchBest.text = timerConvertor(data.totalTime);
        }

        void Start()
        {
            AudioManager.Instance.GetMusicByID(BackgroundMusic).PlayMusic();

            if (PlayerScript && LevelDataTransition.Instance != null)
            {
                LevelDataTransition data = LevelDataTransition.Instance;
                PlayerScript.gunList = data.gunList;
                PlayerScript.HP = data.HP;
                PlayerScript.armHP = data.armHP;
                PlayerScript.selectedGun = data.selectedGun;
                PlayerScript.grenadeCount = data.grenadeCount;
                data.DestroyInstance();
            }
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

            if (impCamera)
            {
                camStart = Camera.main.transform.localPosition;
                Camera.main.transform.localPosition = Vector3.Lerp(camStart, camEnd, 3 * Time.deltaTime);
                ignoreMovement = true;
                stopWatchActive = false;
            }

            if (lerpEnded && !isPaused)
            {
                statePause();
                MenuActive = menuLose;
                MenuActive.SetActive(isPaused);

                menuAudio.PlayOneShot(loseAudio[Random.Range(0, loseAudio.Length)], loseVolume);
            }
        }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Search for an existing instance in the scene
                    _instance = FindObjectOfType<GameManager>();
                    AudioManager AudioManager = AudioManager.Instance;
                    SaveManager SaveManager = SaveManager.Instance;

                    // If none exists, create a new GameObject and attach SaveManager to it
                    if (_instance == null)
                    {
                        GameObject gameManager = new GameObject("GameManager");
                        _instance = gameManager.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }

        public void statePause()
        {
            grenadeIcon.SetActive(false);
            isPaused = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            stopWatchActive = false;
        }

        public void stateUnpause()
        {
            isPaused = false;
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
            if (enemyCount <= 0)
            {
                StartCoroutine(levelTrans());
                roundPassed++;
                roundNumber++;
                spawnMoreEnemies = true;

                statePause();
                MenuActive = menuWin;
                MenuActive.SetActive(isPaused);

                menuAudio.PlayOneShot(winAudio[Random.Range(0, winAudio.Length)], winVolume);

                if (SaveManager.CurrentData.totalTime >= currentTime)  
                {
                    SaveManager.CurrentData.totalTime = currentTime;
                    SaveManager.Instance.Save("savefile.json");
                }
            }
        }

        public void GameLoss()
        {
            if (!diedOnce)
            {
                deathCam();
                menuAudio.PlayOneShot(loseAudio[Random.Range(0, loseAudio.Length)], loseVolume);
                diedOnce = true;
                StartCoroutine(pauseOnDeath());
            }
            
            //statePause();
            //MenuActive = menuLose;
            //MenuActive.SetActive(isPaused);

            //menuAudio.PlayOneShot(loseAudio[Random.Range(0, loseAudio.Length)], loseVolume);

            if (SaveManager.CurrentData.totalTime <= currentTime)
            {
                SaveManager.CurrentData.totalTime = currentTime;
                SaveManager.Instance.Save("savefile.json");
            }
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

        public void deathCam()
        {
            stopCameraRotation = true;
            impCamera = true;
            Camera.main.transform.localRotation = Quaternion.Euler(45f, 0, 0);   
        }

        IEnumerator pauseOnDeath()
        {
            lerpEnded = false;
            yield return new WaitForSeconds(2f); 
            lerpEnded = true;
        }

        IEnumerator levelTrans()
        {
            roundTransition = true;
            yield return new WaitForSeconds(5f); 
            roundTransition = false;
        }

        public void AudioMenu()
        {
            MenuActive = audioMenu;
            MenuActive.SetActive(true);
        }

        public void AudioMenuBack()
        {
            audioMenu.SetActive(false);
            MenuActive = MenuPause;
        }
    }
    
}