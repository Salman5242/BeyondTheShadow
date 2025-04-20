using UnityEngine;
using UnityEngine.Audio;

namespace Infrastructure.Services
{
    public class AudioManager : MonoBehaviour, IService
    {
        [SerializeField]
        private AudioMixerGroup _audioMixer;

        private const float Epsilon = 0.001f;

        private AudioSource _audioSource;


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(this);
        }

        public void SetSoundVolume(float soundPercent)
        {
            soundPercent /= 100f;
            soundPercent = CalculateMixerVolume(soundPercent);
            _audioMixer.audioMixer.SetFloat("SoundsVolume", soundPercent);
        }

        public void SetAmbientVolume(float soundPercent)
        {
            soundPercent /= 100f;
            soundPercent = CalculateMixerVolume(soundPercent);
            _audioMixer.audioMixer.SetFloat("AmbientVolume", soundPercent);
        }

        public void PlaySound(AudioClip _audioClip, float volume = 1f)
        {
            if (_audioClip != null)
                _audioSource.PlayOneShot(_audioClip, volume);
        }

        private float CalculateMixerVolume(float volume)
        {
            if (volume <= Epsilon) return -80f;
            else return Mathf.Log10(volume) * 20;
        }
    }
}