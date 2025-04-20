using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostStateMachine : MonoBehaviour
{
    public GhostState _currState;
    public void ChangeState(GhostState newState)
    {
        if (_currState == newState) return;
        if(_currState != null) _currState.ExitState();
        newState.EnterState();
        _currState = newState;
    }
}
