using System;
using System.Collections;
using System.Collections.Generic;
using TackleBox.Guns;
using TackleBox.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace TackleBox.Level
{
    public class LevelDataTransition : MonoBehaviour
    {
        public static LevelDataTransition Instance { get; private set; }

        public List<gunStats> gunList;
        public int HP;
        public int armHP;
        public int selectedGun;
        public int grenadeCount;

        public LevelDataTransition(playerController playerController = null)
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                if (playerController)
                    savePlayerStats(playerController);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void DestroyInstance()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
                Instance = null;
            }
        }

        public void savePlayerStats(playerController playerController)
        {
            gunList = playerController.gunList;
            HP = playerController.HP;
            armHP = playerController.armHP;
            selectedGun = playerController.selectedGun;
            grenadeCount = playerController.grenadeCount;
        }

        public void loadPlayerStats(playerController playerController)
        {
            playerController.gunList = this.gunList;
            playerController.HP = this.HP;
            playerController.armHP = this.armHP;
            playerController.selectedGun = this.selectedGun;
            playerController.grenadeCount = this.grenadeCount;
        }
    }
}