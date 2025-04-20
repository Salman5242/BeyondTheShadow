using GameFeatures;
using Ghosts;
using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using UnityEngine;
using Utilities.Constants;

public class HantuUnique : MonoBehaviour
{
    [SerializeField]
    private float _speedInColdRoom;

    private LevelRooms.LevelRoomsEnum _ghostRoom;
    private LevelSetUp _levelSetUp;

    private AttackPatrol _attackPatrol;
    private GhostInfo _ghostInfo;
    private RoomIdentifire _roomIdentifire;

    private float _normalGhostSpeed;
    private float _speedPlusPerTick = 0.1f;
    private float _speedMinusPerTick = 0.09f;

    private float _waitForSpeedChange = 0.2f;
    private WaitForSeconds _jinnCheckCD;

    void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();
        _ghostRoom = _levelSetUp.CurrGhostRoom;

        _attackPatrol = GetComponentInParent<AttackPatrol>();
        _ghostInfo = GetComponentInParent<GhostInfo>();
        _roomIdentifire = GetComponentInParent<RoomIdentifire>();

        _jinnCheckCD = new WaitForSeconds(_waitForSpeedChange);
        _normalGhostSpeed = _ghostInfo.GhostData.GhostAttackSpeed;

        StartCoroutine(nameof(SpeedCalc));
    }

    private IEnumerator SpeedCalc()
    {
        while (true)
        {
            if (_attackPatrol.IsAttacking)
            {
                if (_roomIdentifire.CurrRoom == _ghostRoom)
                    _attackPatrol.ChangeGhostSpeed(Mathf.Min((_attackPatrol.GhostCurrAttackSpeed + _speedPlusPerTick), _speedInColdRoom));
                else
                    _attackPatrol.ChangeGhostSpeed(Mathf.Max((_attackPatrol.GhostCurrAttackSpeed - _speedMinusPerTick), _normalGhostSpeed));
            }
            yield return _jinnCheckCD;
        }
    }
}
