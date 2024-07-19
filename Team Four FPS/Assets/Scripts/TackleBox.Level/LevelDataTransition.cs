using System;
using System.Collections;
using System.Collections.Generic;
using TackleBox.Guns;
using UnityEngine;

namespace TackleBox.Level
{
    public class LevelDataTransition : MonoBehaviour
    {
        public static LevelDataTransition Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
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


    }
}