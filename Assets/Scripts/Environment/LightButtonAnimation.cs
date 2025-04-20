using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButtonAnimation : MonoBehaviour
{
    [SerializeField] LightButton _lightButton;

    [SerializeField] private Transform _rotationAxis;
    [SerializeField] private float _enabledButtonAngle = 10f;

    private void Start()
    {
        _lightButton._onLightOn += TurnOn;
        _lightButton._onLightOff += TurnOff;

        if (_lightButton.CheckIfEnabled()) TurnOn();
        else TurnOff();
    }
    private void OnDestroy()
    {
        _lightButton._onLightOn -= TurnOn;
        _lightButton._onLightOff -= TurnOff;
    }
    private void TurnOn()
    {
        _rotationAxis.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
    private void TurnOff()
    {
        _rotationAxis.transform.localRotation = Quaternion.Euler(-_enabledButtonAngle, 0f, 0f);
    }
}
