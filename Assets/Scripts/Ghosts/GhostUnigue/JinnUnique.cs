using Ghosts;
using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using UnityEngine;
using Utilities.Constants;

public class JinnUnique : MonoBehaviour
{
    [SerializeField]
    private float _minGhostSpeed;

    private AttackPatrol _attackPatrol;
    private GhostInfo _ghostInfo;

    private float _normalGhostSpeed;
    private float _speedPlusPerTick = 0.1f;
    private float _speedMinusPerTick = 0.13f;

    private float _waitForSpeedChange = 0.2f;
    private WaitForSeconds _jinnCheckCD;

    void Start()
    {
        _attackPatrol = GetComponentInParent<AttackPatrol>();
        _ghostInfo = GetComponentInParent<GhostInfo>();

        _jinnCheckCD = new WaitForSeconds(_waitForSpeedChange);
        _normalGhostSpeed = _ghostInfo.GhostData.GhostAttackSpeed;

        StartCoroutine(nameof(JinnSpeedCalc));
    }

    private IEnumerator JinnSpeedCalc()
    {
        while (true)
        {
            if (_attackPatrol.IsAttacking)
            {
                if (_attackPatrol.IsFollowing)
                    _attackPatrol.ChangeGhostSpeed(Mathf.Max((_attackPatrol.GhostCurrAttackSpeed - _speedMinusPerTick), _minGhostSpeed));
                else
                    _attackPatrol.ChangeGhostSpeed(Mathf.Min((_attackPatrol.GhostCurrAttackSpeed + _speedPlusPerTick), _normalGhostSpeed));
            }
            yield return _jinnCheckCD;
        }
    }
}
