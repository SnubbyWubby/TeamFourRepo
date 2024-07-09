using System;
using TackleBox.SaveSystem;
using UnityEngine;

namespace TackleBox.Audio
{
    public class AudioManager : MonoBehaviour
    {

        // Static instance of SoundManager which allows it to be accessed by any other script
        private static AudioManager _instance;
        [SerializeField] private Audio[] AudioList;

        // Property to access the instance of the SoundManager
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Search for an existing instance in the scene
                    _instance = FindObjectOfType<AudioManager>();

                    // If none exists, create a new GameObject and attach SaveManager to it
                    if (_instance == null)
                    {
                        GameObject soundManager = new GameObject("SoundManager");
                        _instance = soundManager.AddComponent<AudioManager>();
                    }

                    // Mark the instance to not be destroyed on scene load
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        // Make sure the instance is null if this object is destroyed
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public Audio GetSoundByID(string audioID)
        {
            foreach (Audio audio in AudioList)
            {
                if (audio && audio.ID.ToLower() == audioID.ToLower()) return audio;
            }
            return ScriptableObject.CreateInstance<Audio>();
        }

        // Ensure that the instance is not destroyed when the scene changes
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(_instance.gameObject);
            }
        }
    }
}