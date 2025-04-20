using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using Infrastructure.Services;
using Infrastructure;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _audioMixer;

    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeTXT;
    private const float Epsilon = 0.001f;

    private AudioManager _audioManager;

    void Start()
    {
        _audioManager = AllServices.Container.Single<AudioManager>();
        LoadVolume();
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("SoundsVolume"))
        {
            ChangeVolume(PlayerPrefs.GetFloat("SoundsVolume"));
            _volumeSlider.value = PlayerPrefs.GetFloat("SoundsVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("SoundsVolume", 1f);
            ChangeVolume(1f);
            _volumeSlider.value = 1f;
        }
    }

    public void ChangeSliderParam()
    {
        ChangeVolume(_volumeSlider.value);
        PlayerPrefs.SetFloat("SoundsVolume", _volumeSlider.value);
    }



    private void ChangeVolume(float volume)
    {
        _volumeTXT.text = (int)(volume * 100) + "%";
        _audioManager.SetSoundVolume(volume * 100f);
    }

    private float CalculateMixerVolume(float volume)
    {
        if (volume <= Epsilon) return -80f;
        else return Mathf.Log10(volume) * 20;
    }
}
