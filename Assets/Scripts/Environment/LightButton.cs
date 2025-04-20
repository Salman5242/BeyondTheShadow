using UnityEngine;
using Items.Logic;
using System;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LightButton : MonoBehaviour, IClickable
{
    public Action _onLightOn, _onLightOff;

    [SerializeField] private Light[] _connectedLights;

    [SerializeField] private bool _isEnabled = true;

    [SerializeField]
    private AudioClip _switchSound;
    [SerializeField]
    private float _volume;

    [SerializeField]
    private AudioClip _explodeSound;
    [SerializeField]
    private float _explodeVolume = 0.1f;
    [SerializeField]
    private GameObject _sparkParticles;

    private float[] _startLightPower;

    private AudioSource _audioSource;
    private bool _isExploded = false;
    private bool _ghostAttack = false;

    private float _flickTime = 0.3f;
    private int _stepsCount = 10;
    private float _change;
    private float _powerMod;
    private float _newLightPower;
    private bool _lightsGoUp;




    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_isEnabled)
        {
            EnableAllLights();
        }
        else
        {
            DisableAllLights();
        }


        _startLightPower = new float[_connectedLights.Length];
        for (int i = 0; i < _connectedLights.Length; i++)
        {
            _startLightPower[i] = _connectedLights[i].intensity;
        }
    }
    public void OnClick()
    {
        if (!_isEnabled && !_ghostAttack && !_isExploded)
        {
            EnableAllLights();
            _isEnabled = true;
            _onLightOn?.Invoke();
        }
        else if (_isEnabled && !_ghostAttack && !_isExploded)
        {
            DisableAllLights();
            _isEnabled = false;
            _onLightOff?.Invoke();
        }
        _audioSource.PlayOneShot(_switchSound, _volume);
    }
    public void ExplodeLight()
    {
        if (!_isExploded && _isEnabled)
        {
            DisableAllLights();
            AudioHelper.PlayClipAtPoint(_explodeSound, _connectedLights[0].transform.position, _explodeVolume);
            foreach (Light light in _connectedLights) Instantiate(_sparkParticles, light.transform.position, Quaternion.Euler(90f, 0f, 0f));
            _isExploded = true;
        }
    }
    public void GhostAttackOffLight()
    {
        //DisableAllLights();
        //_onLightOff?.Invoke();
        //_isEnabled = false;
        StartCoroutine(nameof(LightFlick));
        _ghostAttack = true;
    }
    public void EndGhostAttack()
    {
        StopCoroutine(nameof(LightFlick));
        for (int i = 0; i < _connectedLights.Length; i++)
        {
            _connectedLights[i].intensity = _startLightPower[i];
        }
        DisableAllLights();
        _isEnabled = false;
        _ghostAttack = false;
    }

    IEnumerator LightFlick()
    {
        _lightsGoUp = true;
        while (true)
        {
            if (_lightsGoUp) _flickTime = UnityEngine.Random.Range(0.4f, 0.6f);
            else _flickTime = UnityEngine.Random.Range(0.15f, 0.25f);

            yield return new WaitForSeconds(_flickTime);
            if (_isEnabled)
            {
                if (_lightsGoUp)
                {
                    _powerMod = UnityEngine.Random.Range(0.4f, 0.8f);
                    _lightsGoUp = false;
                }
                else
                {
                    _powerMod = UnityEngine.Random.Range(-0.2f, 0.25f);
                    _powerMod = Mathf.Max(0, _powerMod);
                    _lightsGoUp = true;
                }

                _newLightPower = _startLightPower[0] * _powerMod;

                _change = (_connectedLights[0].intensity - _newLightPower) / _stepsCount;
                StartCoroutine(nameof(ChangeLightPower));
            }
        }
    }
    IEnumerator ChangeLightPower()
    {
        for (int i = 0; i < _stepsCount; i++)
        {
            foreach (Light light in _connectedLights)
            {
                light.intensity -= _change;
            }

            yield return new WaitForSeconds(_flickTime / _stepsCount);
        }
    }

    public bool CheckIfEnabled()
    {
        return _isEnabled;
    }
    private void EnableAllLights()
    {
        foreach (Light light in _connectedLights)
        {
            light.enabled = true;
        }
    }
    private void DisableAllLights()
    {
        foreach (Light light in _connectedLights)
        {
            light.enabled = false;
        }
    }
}