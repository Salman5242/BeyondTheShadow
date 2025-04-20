using Ghosts.Mood;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAttack : MonoBehaviour
{
    private AttackChecker _attackChecker;

    public bool StartAttack = false;


    private void Start()
    {
        _attackChecker = GetComponent<AttackChecker>();
    }
    void Update()
    {
        if (StartAttack)
        {
            _attackChecker.MakeGhostHunt();
            StartAttack = false;
        }
    }
}
