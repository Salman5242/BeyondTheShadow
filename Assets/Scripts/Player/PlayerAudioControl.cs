using GameFeatures;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.States.GameStates;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Utilities.Constants;

public class PlayerAudioControl : MonoBehaviour
{
    [SerializeField]
    private float _activeOutsideVolume = 1f;
    [SerializeField]
    private float _activeInsideVolume = 1f;
    [SerializeField]
    private float _heartBeatMaxVolume = 1f;

    [SerializeField]
    private float _inactiveVolume = 0f;

    [SerializeField]
    private AudioSource _outsideAudioSource;
    [SerializeField]
    private AudioSource _insideAudioSource;
    [SerializeField]
    private AudioSource _heartBeatSource;

    [SerializeField]
    private AudioMixerGroup _audioMixer;

    private RoomIdentifire _roomIdentifire;

    private WaitForSeconds WaitForSeconds = new WaitForSeconds(0.5f);

    private AudioManager _audioManager;
    private bool _inHouse = false;

    private int _fadeSteps = 10;
    private float _fadeTime = 0.4f;

    private void Start()
    {
        _audioManager = AllServices.Container.Single<AudioManager>();
        _roomIdentifire = GetComponent<RoomIdentifire>();

        _outsideAudioSource.volume = _activeOutsideVolume;
        _insideAudioSource.volume = _inactiveVolume;

        if (GameBootstrapper.Instance.StateMachine.GetCurrentState() is LobbyState) _audioManager.SetAmbientVolume(40f);
        else _audioManager.SetAmbientVolume(100f);

        StartCoroutine(nameof(CheckPlayerPosition));
    }

    public void StartHeartBeat()
    {
        _heartBeatSource.Play();
        _heartBeatSource.volume = 0;
    }

    public void SetHeartBeatVolume(float percent) => _heartBeatSource.volume = (percent * _heartBeatMaxVolume) / 100f;
    public void StopHeartBeat() => _heartBeatSource.Stop();


    IEnumerator CheckPlayerPosition()
    {
        while (true)
        {
            if (_roomIdentifire.CurrRoom == LevelRooms.LevelRoomsEnum.NoRoom && _inHouse)
            {
                _inHouse = false;
                StartCoroutine(nameof(SetOutsideSoundVolumes));
            }
            else if (_roomIdentifire.CurrRoom != LevelRooms.LevelRoomsEnum.NoRoom && !_inHouse)
            {
                _inHouse = true;
                StartCoroutine(nameof(SetInsideSoundVolumes));
            }

            yield return WaitForSeconds;
        }
    }

    IEnumerator SetInsideSoundVolumes()
    {
        for (int i = 0; i < _fadeSteps; i++)
        {
            _outsideAudioSource.volume -= (_activeOutsideVolume - _inactiveVolume) / _fadeSteps;
            _insideAudioSource.volume += (_activeInsideVolume - _inactiveVolume) / _fadeSteps;

            yield return new WaitForSeconds(_fadeTime / _fadeSteps);
        }

        _insideAudioSource.volume = _activeInsideVolume;
        _outsideAudioSource.volume = _inactiveVolume;
    }
    IEnumerator SetOutsideSoundVolumes()
    {
        for (int i = 0; i < _fadeSteps; i++)
        {
            _insideAudioSource.volume -= (_activeInsideVolume - _inactiveVolume) / _fadeSteps;
            _outsideAudioSource.volume += (_activeOutsideVolume - _inactiveVolume) / _fadeSteps;

            yield return new WaitForSeconds(_fadeTime / _fadeSteps);
        }

        _outsideAudioSource.volume = _activeOutsideVolume;
        _insideAudioSource.volume = _inactiveVolume;
    }

}
