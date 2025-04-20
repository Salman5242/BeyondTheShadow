using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVLight : MonoBehaviour
{
    [SerializeField]
    private Light _light;

    [SerializeField]
    public Material _revealableMaterial;

    [SerializeField]
    private float _lightAngle = 360f;

    private Vector3 _curPos;
    private Vector3 _lastPos = new Vector3(0f, 0f, 0f);
    private float _epsilon = 0.001f;
    private float _distance;

    private bool _isEnabled = false;

    private void Start()
    {
        _lastPos = transform.position;
        DisableUVLight();
    }

    IEnumerator CheckPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if (_isEnabled)
            {
                _curPos = transform.position;
                _distance = (_lastPos - _curPos).magnitude;

                if (_distance > _epsilon)
                {
                    _lastPos = _curPos;
                    ChangeMaterialParameters();
                }
            }
        }
    }

    public void DisableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", 0f);
        ChangeMaterialParameters();
        _isEnabled = false;
        StopCoroutine(nameof(CheckPosition));
    }
    public void EnableUVLight()
    {
        _revealableMaterial.SetFloat("lightAngle", _lightAngle);
        ChangeMaterialParameters();
        _isEnabled = true;
        StartCoroutine(nameof(CheckPosition));
    }
    private void ChangeMaterialParameters()
    {
        _revealableMaterial.SetVector("lightPosition", _light.transform.position);
        _revealableMaterial.SetVector("lightDirection", -_light.transform.forward);
    }
}
