using System;
using TackleBox.SaveSystem;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace TackleBox.Audio
{
    public class AudioManager : MonoBehaviour
    {

        // Static instance of AudioManager which allows it to be accessed by any other script
        private static AudioManager _instance;

        public AudioMixer AudioMixer;
        public AudioMixerGroup[] AudioMixerGroups;
        [SerializeField] Audio[] AudioList;
        [SerializeField] Music[] MusicList;

        static AudioSource _audioSource;
        static AudioSource _musicSource;

        // Property to access the instance of the AudioManager
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
                        GameObject audioManager = new GameObject("AudioManager");
                        _instance = audioManager.AddComponent<AudioManager>();
                    }

                }
                return _instance;
            }
        }

        public static AudioSource AudioSource
        {
            get
            {
                if (_instance == null)
                    _instance = Instance;


                if (_audioSource == null)
                {
                    _audioSource = _instance.gameObject.AddComponent<AudioSource>();
                    _audioSource.outputAudioMixerGroup = _instance.GetAudioGroup("Sound Effects");
                    _audioSource.loop = false;
                }

                return _audioSource;
            }
        }

        public static AudioSource MusicSource
        {
            get
            {
                if (_instance == null)
                    _instance = Instance;

                if (_musicSource == null)
                {
                    _musicSource = _instance.gameObject.AddComponent<AudioSource>();
                    _musicSource.outputAudioMixerGroup = _instance.GetAudioGroup("Music");
                    _musicSource.loop = true;
                }

                return _musicSource;
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
                if (audio && audio.ID.ToLower() == audioID.ToLower()) return audio;

            return ScriptableObject.CreateInstance<Audio>();
        }
        public Music GetMusicByID(string musicID)
        {
            foreach (Music music in MusicList)
                if (music && music.ID.ToLower() == musicID.ToLower()) return music;

            return ScriptableObject.CreateInstance<Music>();
        }

        public AudioMixerGroup GetAudioGroup(string GroupID)
        {
            foreach (AudioMixerGroup group in AudioMixerGroups)
                if (group && group.name.ToLower() == GroupID.ToLower()) return group;

            return null;
        }

        public void PlayOneShot(string audioID, AudioSource source = null)
        {
            Audio sound = GetSoundByID(audioID);
            if (!string.IsNullOrEmpty(sound.name))
                sound.PlayOneShot(source);
        }

        // Ensure that the instance is not destroyed when the scene changes
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(_instance.gameObject);
            }
        }
    }
}