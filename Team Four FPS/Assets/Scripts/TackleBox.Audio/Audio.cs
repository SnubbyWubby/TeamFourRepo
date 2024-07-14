using UnityEngine;

namespace TackleBox.Audio
{
    [CreateAssetMenu(fileName = "Audio", menuName = "TackleBox.Audio/Audio", order = 1)]

    public class Audio : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] AudioClip[] audio;
        [SerializeField][Range(0,1)] float Volume = 1;

        public string ID
        {
            get
            {
                return _name;
            }
        }

        public void PlayOneShot(AudioSource source)
        {
            var audioSource = source != null ? source : AudioManager.AudioSource;
            //Play Audio
            if (string.IsNullOrEmpty(_name))
                return;

            if (audio.Length > 1)
                audioSource.PlayOneShot(audio[Random.Range(0, audio.Length)], Volume);
            else
                audioSource.PlayOneShot(audio[0], Volume);
        }
    }
}
