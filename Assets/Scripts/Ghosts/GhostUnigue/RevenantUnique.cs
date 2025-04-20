using Ghosts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevenantUnique : MonoBehaviour
{
    [SerializeField]
    private float _maxGhostSpeed;

    private AttackPatrol _attackPatrol;
    private GhostInfo _ghostInfo;

    private float _normalGhostSpeed;
    private float _speedPlusPerTick = 0.08f;
    private float _speedMinusPerTick = 0.15f;

    private float _waitForSpeedChange = 0.2f;
    private WaitForSeconds _revCheckCD;

    void Start()
    {
        _attackPatrol = GetComponentInParent<AttackPatrol>();
        _ghostInfo = GetComponentInParent<GhostInfo>();

        _revCheckCD = new WaitForSeconds(_waitForSpeedChange);
        _normalGhostSpeed = _ghostInfo.GhostData.GhostAttackSpeed;

        StartCoroutine(nameof(RevSpeedCalc));
    }

    private IEnumerator RevSpeedCalc()
    {
        while (true)
        {
            if (_attackPatrol.IsAttacking)
            {
                if (_attackPatrol.IsFollowing)
                    _attackPatrol.ChangeGhostSpeed(Mathf.Min((_attackPatrol.GhostCurrAttackSpeed + _speedPlusPerTick), _maxGhostSpeed));
                else
                    _attackPatrol.ChangeGhostSpeed(Mathf.Max((_attackPatrol.GhostCurrAttackSpeed - _speedMinusPerTick), _normalGhostSpeed));
            }
            yield return _revCheckCD;
        }
    }
}
