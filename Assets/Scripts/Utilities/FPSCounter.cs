using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private int[] _fpsBuffer;
    private int _fpsBufferIndex;
    private TextMeshProUGUI _fpsText;

    private int fps;
    private int _frameRange = 10;
    private void Awake()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
        DontDestroyOnLoad(this.gameObject.transform.parent);
    }
    private void Update()
    {
        if (_fpsBuffer == null || _frameRange != _fpsBuffer.Length)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFps();

        _fpsText.text = fps.ToString();
    }

    private void InitializeBuffer()
    {
        _fpsBuffer = new int[_frameRange];
        _fpsBufferIndex = 0;
    }

    private void UpdateBuffer()
    {
        _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (_fpsBufferIndex >= _frameRange)
        {
            _fpsBufferIndex = 0;
        }
    }

    private void CalculateFps()
    {
        int sum = 0;

        for (int i = 0; i < _frameRange; i++)
        {
            int fps = _fpsBuffer[i];
            sum += fps;
        }

        fps = sum / _frameRange;
    }
}
