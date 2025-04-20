using UnityEngine;
using Infrastructure.Services;
using Infrastructure;
using UnityEngine.AI;
using System.Collections.Generic;
using Utilities.Constants;
using Utilities;
using System.Collections;

public class PatrolWalk : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private float _stoppingDistance = 0.3f;
    [SerializeField]
    private GhostInfo _ghostInfo;

    [SerializeField]
    private bool _goNextPoint = false;

    public LevelRooms.LevelRoomsEnum _GhostRoom;

    Transform bestPoint = null;

    private float _ghostNormalSpeed;
    private int _patrolRandomMultiplier = 3;
    private Transform[] _patrolPoints;
    private LevelSetUp _levelSetUp;

    private Transform _currDestination = null;
    private Transform _levelTransformPoint;

    private List<int> _randomPointsList;
    private bool _isStopped = true;

    private void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();
        SetUpGhostData();
    }
    private void OnEnable()
    {
        StartPatrolling();
    }

    private void OnDisable()
    {
        StopPatrolling();
    }
    private void Update()
    {
        if (_isStopped) return;

        //if (_agent.remainingDistance == float.PositiveInfinity)
        //{
        //    // NavMesh.SamplePosition()
        //     Debug.LogWarning("Infinity");
        //}

        if ( (_agent.remainingDistance <= _stoppingDistance || _currDestination == null) && !_agent.pathPending)
        {
            ChoosePoint();
            SetDestination();
        }
    }

    public void StartPatrolling()
    {
        _agent.speed = _ghostNormalSpeed;
        SwitchStopState(false);
       // StartCoroutine(CheckAllPoints());
    }

    private void StopPatrolling()
    {
        SwitchStopState(true);
    }

    public void SwitchStopState(bool isStopped)
    {
        _isStopped = isStopped;

        if (!_agent.isOnNavMesh) return;
        if (isStopped)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }
        else
        {
            _agent.isStopped = false;
        }
    }

    private void ChoosePoint()
    {
        _randomPointsList = RandomGenerator.GenerateRandom((_patrolPoints.Length / _patrolRandomMultiplier), 0, _patrolPoints.Length);

        bestPoint = null;
        float bestDistance = float.MaxValue;
        float currDistanceToPoints;

        foreach (int point in _randomPointsList)
        {
            if (_patrolPoints[point] != _currDestination)
            {
                currDistanceToPoints = Vector3.Distance(_patrolPoints[point].position, _levelTransformPoint.position);
                currDistanceToPoints += (Mathf.Abs(_patrolPoints[point].position.y - _levelTransformPoint.position.y) * 10);
                if (currDistanceToPoints < bestDistance)
                {
                    bestPoint = _patrolPoints[point];
                    bestDistance = currDistanceToPoints;
                }
            }
        }
        if (bestPoint != null) {
            // Debug.Log("Point chosen = " + bestPoint.name);
            _currDestination = bestPoint; }
    }

    private void SetDestination()
    {
        if (_currDestination != null) { _agent.SetDestination(_currDestination.position); }
        else { Debug.Log("Curr destination = null"); }
    }

    private void SetUpGhostData()
    {
        _patrolRandomMultiplier = _ghostInfo.GhostData.PatrolRandomMultiplier;
        _ghostNormalSpeed = _ghostInfo.GhostData.GhostNormalSpeed;

        _GhostRoom = _levelSetUp.CurrGhostRoom;
        _patrolPoints = _levelSetUp.GetGhostPatrolPoints();
        _levelTransformPoint = _levelSetUp.CurrGhostRoomTransform;

        _agent.speed = _ghostNormalSpeed;
    }
}
