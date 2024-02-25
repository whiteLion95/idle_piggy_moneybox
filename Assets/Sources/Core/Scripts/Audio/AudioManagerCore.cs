using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyLibrary.Audio
{
    public class AudioManagerCore : MonoBehaviour
    {
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected List<AudioData> audioData;

        protected Dictionary<string, AudioClip> audioClips;

        public static AudioManagerCore Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = this;
            InitClips();
        }

        private void InitClips()
        {
            audioClips = new Dictionary<string, AudioClip>();

            foreach (AudioData data in audioData)
            {
                audioClips.Add(data.Name, data.Clip);
            }
        }

        public void PlayOneShot(string audioName)
        {
            AudioClip clip = audioClips[audioName];
            audioSource.PlayOneShot(clip);
        }
    }

    [System.Serializable]
    public struct AudioData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AudioClip Clip { get; private set; }
    }
}