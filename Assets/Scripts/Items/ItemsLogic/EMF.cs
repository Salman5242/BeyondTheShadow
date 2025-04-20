using Infrastructure;
using Infrastructure.Services;
using Items.ItemsLogic;
using Items.Logic;
using System;
using System.Collections;
using UnityEngine;

public class EMF : MonoBehaviour, IMainUsable
{
    [SerializeField]
    private Light[] _lightPoints;


    [SerializeField]
    private AudioClip _slowPeep;
    [SerializeField]
    private AudioClip _fastPeep;

    private AudioSource _audioSource;


    private bool _isEmfEnabled = false;
    private int _currEmfLvl = 1;
    private int _EmfLvlFound = 1;

    public const float EmfRadius = 1.5f;
    private const float EmfCheckCD = 0.1f;

    private GameObjectivesService _gameObjectives;

    private void Start()
    {
        _gameObjectives = AllServices.Container.Single<GameObjectivesService>();
        _audioSource = GetComponent<AudioSource>();
    }
    public void OnMainUse()
    {
        SwitchEnable();
    }

    private void OnEnable()
    {
        if (_isEmfEnabled)
        {
            StartCoroutine(nameof(CheckForInteractions));
            _audioSource.Play();
        }
    }
    private void OnDisable()
    {
        StopCoroutine(nameof(CheckForInteractions));
    }
    private void SwitchEnable()
    {
        if (_isEmfEnabled)
        {
            _isEmfEnabled = false;
            _audioSource.Stop();
            StopCoroutine(nameof(CheckForInteractions));
            TurnOff();
        }
        else
        {
            _isEmfEnabled = true;
            _audioSource.Play();
            TurnOn();
            StartCoroutine(nameof(CheckForInteractions));
        }
    }

    private IEnumerator CheckForInteractions()
    {
        while (true)
        {
            yield return new WaitForSeconds(EmfCheckCD);
            CheckForInter();
            ShowLights();
        }
    }

    private void CheckForInter()
    {
        _EmfLvlFound = 1;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, EmfRadius);
        foreach (var hitCollider in hitColliders)
        {
            InteractionScript _interection = hitCollider.GetComponent<InteractionScript>();
            if(_interection) if (_EmfLvlFound < _interection.EmfLvl) _EmfLvlFound = _interection.EmfLvl;

        }
        if (_EmfLvlFound >= 2) _gameObjectives.EMFLevelFound(_EmfLvlFound);
        _currEmfLvl = _EmfLvlFound;

        if (_EmfLvlFound <= 1) _audioSource.Pause();
        else if (_EmfLvlFound > 1 && _EmfLvlFound <= 3)
        {
            _audioSource.clip = _slowPeep;
            if (!_audioSource.isPlaying) _audioSource.Play();
        }
        else
        {
            _audioSource.clip = _fastPeep;
            if (!_audioSource.isPlaying) _audioSource.Play();
        }
    }

    private void ShowLights()
    {
        if (!_isEmfEnabled) _currEmfLvl = 0;
        for(int i = 0; i < _currEmfLvl; i++)
        {
            _lightPoints[i].gameObject.SetActive(true);
        }
        for(int i = _currEmfLvl; i< _lightPoints.Length; i++)
        {
            _lightPoints[i].gameObject.SetActive(false);
        }
    }

    private void TurnOn()
    {
        _currEmfLvl = 1;
        ShowLights();
    }

    private void TurnOff()
    {
        _currEmfLvl = 0;
        ShowLights();
    }

}
