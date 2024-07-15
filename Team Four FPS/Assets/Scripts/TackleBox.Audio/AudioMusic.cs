using UnityEngine;

namespace TackleBox.Audio
{
    [CreateAssetMenu(fileName = "Music", menuName = "TackleBox.Audio/Music", order = 1)]

    public class Music : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _trackName;
        [SerializeField] private string _trackArtist;
        [SerializeField] AudioClip audio;
        [SerializeField] float Volume = 0.5f;

        public string ID
        {
            get
            {
                return _name;
            }
        }

        public void PlayMusic(AudioSource source = null)
        {
            var musicSource = source != null ? source : AudioManager.MusicSource;
            //Play Audio
            if (string.IsNullOrEmpty(_name))
                return;
            if (audio != null)
            {
                musicSource.volume = Volume;
                musicSource.clip = audio;
                musicSource.Play();
            }

            //if (audio != null)
            //{
            //    source.(audio, Volume);
            //}

        }
    }
}
