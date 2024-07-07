using UnityEngine;

namespace TackleBox.Audio
{
    [CreateAssetMenu]

    public class Audio : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] AudioClip[] audio;
        [SerializeField] float Volume;

        public string ID
        {
            get
            {
                return _name;
            }
        }

        public void PlayOneShot(AudioSource source)
        {
            //Play Audio
            if (string.IsNullOrEmpty(_name))
                return;

            if (audio.Length > 1)
                source.PlayOneShot(audio[Random.Range(0, audio.Length)], Volume);
            else
                source.PlayOneShot(audio[0], Volume);
        }
    }
}
