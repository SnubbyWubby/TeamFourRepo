using System;
using System.Collections;
using System.Collections.Generic;
using TackleBox.Guns;
using TackleBox.SaveSystem;
using TackleBox.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace TackleBox.Level
{
    public class LevelDataTransition : MonoBehaviour
    {
        private static LevelDataTransition _instance;

        public List<gunStats> gunList;
        public int? HP;
        public int? armHP;
        public int? selectedGun;
        public int? grenadeCount;

        public static LevelDataTransition Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Search for an existing instance in the scene
                    _instance = FindObjectOfType<LevelDataTransition>();

                    // If none exists, create a new GameObject and attach SaveManager to it
                    if (_instance == null)
                    {
                        GameObject saveManager = new GameObject("LevelDataTransition");
                        _instance = saveManager.AddComponent<LevelDataTransition>();
                    }

                    // Mark the instance to not be destroyed on scene load
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        private LevelDataTransition(playerController playerController = null) { }

        public void savePlayerStats(playerController playerController)
        {
            if (playerController != null)
            {
                gunList = playerController.gunList;
                HP = playerController.HP;
                armHP = playerController.armHP;
                selectedGun = playerController.selectedGun;
                grenadeCount = playerController.grenadeCount;
            }
        }

        public void loadPlayerStats(playerController playerController)
        {
            if (playerController != null)
            {
                if (this.gunList != null)
                    playerController.gunList = this.gunList;

                playerController.HP = this.HP ?? playerController.HP;
                playerController.armHP = this.armHP ?? playerController.armHP;
                playerController.selectedGun = this.selectedGun ?? playerController.selectedGun;
                playerController.grenadeCount = this.grenadeCount ?? playerController.grenadeCount;

                //playerController.gunList = this.gunList;
                //playerController.HP = this.HP;
                //playerController.armHP = this.armHP;
                //playerController.selectedGun = this.selectedGun;
                //playerController.grenadeCount = this.grenadeCount;
                //Debug.Log("Ree");
            }

        }
    }
}