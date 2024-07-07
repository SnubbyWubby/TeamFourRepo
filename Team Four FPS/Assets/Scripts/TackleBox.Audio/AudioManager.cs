using System;
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
                    // Look for an existing instance of SoundManager
                    _instance = FindObjectOfType<AudioManager>();

                    if (_instance == null)
                    {
                        // Create a new instance if none is found
                        GameObject soundManager = new GameObject("SoundManager");
                        _instance = soundManager.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }



        // Ensure this class can't be instantiated from the outside
        private AudioManager() { }

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
                //DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(_instance.gameObject);
            }
        }
    }
}