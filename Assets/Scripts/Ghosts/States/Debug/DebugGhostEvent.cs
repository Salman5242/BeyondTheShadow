using Ghosts.Mood;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGhostEvent : MonoBehaviour
{
    private GhostEventChecker _ghostEventChecker;

    public bool StartEvent = false;


    private void Start()
    {
        _ghostEventChecker = GetComponent<GhostEventChecker>();
    }
    void Update()
    {
        if (StartEvent)
        {
            _ghostEventChecker.MakeGhostEvent();
            StartEvent = false;
        }
    }
}
