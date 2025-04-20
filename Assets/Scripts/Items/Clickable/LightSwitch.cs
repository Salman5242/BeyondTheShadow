using UnityEngine;
using Items.Logic;

public class LightSwitch : MonoBehaviour, IClickable
{
    [SerializeField] private Light[] _connectedLights;

    [SerializeField] private Transform _rotationAxis;
    [SerializeField] private float _enabledButtonAngle = 5f;

    [SerializeField] private bool _isEnabled = true;

    [SerializeField]
    private AudioClip _switchSound;
    [SerializeField]
    private float _volume;

    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_isEnabled)
        {
            foreach (Light light in _connectedLights)
            {
                light.enabled = true;
            }
        }
        else
        {
            foreach (Light light in _connectedLights)
            {
                light.enabled = false;
                _rotationAxis.transform.localRotation = Quaternion.Euler(-10f, 0f, 0f);
            }
        }
    }
    public void OnClick()
    {
        if (!_isEnabled)
        {
            foreach (Light light in _connectedLights)
            {
                light.enabled = true;               
            }
            _isEnabled = true;
            _rotationAxis.Rotate(_enabledButtonAngle, 0f, 0f);
        }
        else
        {
            foreach (Light light in _connectedLights)
            {
                light.enabled = false;        
            }
            _isEnabled = false;
            _rotationAxis.Rotate(-_enabledButtonAngle, 0f, 0f);
        }
        _audioSource.PlayOneShot(_switchSound, _volume);
    }

}
