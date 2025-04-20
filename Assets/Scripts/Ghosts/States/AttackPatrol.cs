using UnityEngine;
using Infrastructure.Services;
using Infrastructure;
using UnityEngine.AI;
using Utilities.Constants;
using System.Collections;
using Ghosts.Mood;

namespace Ghosts
{
    public class AttackPatrol : MonoBehaviour
    {
        public float GhostCurrAttackSpeed
        {
            get { return _ghostCurrAttackSpeed; }
        }

        public bool IsAttacking
        {
            get { return _isAttacking; }
        }
        public bool IsFollowing
        {
            get { return _isFollowing; }
        }
        [SerializeField]
        private AudioSource _audioSteps;

        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private float _stoppingDistance = 0.3f;
        [SerializeField]
        private GhostInfo _ghostInfo;

        [SerializeField]
        private LineOfSight _lineOfSight;
        [SerializeField]
        private float _checkForLineCD;

        private GhostMood _ghostMood;

        private string AudioStepsGroup = "GhostSteps";
        private const float SmudgeEffectTime = 5f;
        private bool _playerKilled = false;

        private GameFlowService _gameFlow;
        private GameObjectivesService _gameObjectives;

        private Transform _playerPoint;
        private Transform _heroTransform;
        private PlayerCheckResult _playerCheckResult;
        private PlayerAudioControl _audioControl;

        private WaitForSeconds _checkForLineWait;

        private bool _isGhostDisabled = false;

        private bool _dataSetedUp = false;

        private float _ghostCurrAttackSpeed;
        private float _ghostAttackSpeed;
        private float _distanceToPlayer = 0f;

        private float _sanityPlayerMinus;
        private bool _underSmudgeEffect = false;
        private Transform[] _patrolPoints;
        private LevelSetUp _levelSetUp;

        private Transform _currDestination = null;

        private float _currAudioSpeed;
        private float _ghostDisableTime = 3f;

        private int _randomPointNum;
        private bool _isAttacking = false;
        private bool _isFollowing = false;
        private float _maxAggroDistance;
        private float _heartBeatVolumePercent = 0f;

        private bool _hadFollowed = false;
        private Vector3 _playerPointDistance;
        private Vector3 _ghostPointDistance;

        private float _avgGhostSpeed = 2.5f;
        private const float MaxVolumeDistanceHeartBeat = 2f;
        private const float MinVolumeDistanceHeartBeat = 7f;

        private const float DistanceToKill = 0.5f;
        private void OnEnable()
        {
            if (!_dataSetedUp) SetUpGhostData();
        }
        private void OnDestroy()
        {
            _ghostInfo.GhostSetedUp -= SetUpParams;
        }
        private void Update()
        {
            if (!_isAttacking) return;
            if (_isGhostDisabled)  return; 

            CheckDistanceToPlayer();
            SetHeartBeatVolume();
            SetAudioSpeed();

            if (_isFollowing && !_underSmudgeEffect)
            {
                _hadFollowed = true;
                _currDestination = _playerPoint;

                CheckForKill();
                SetDestination();
                return;
            }
            if (_agent.remainingDistance <= _stoppingDistance || _currDestination == null)
            {
                ChoosePoint();
                SetDestination();
            }
        }

        public void StartAttackPatrolling()
        {
            _ghostMood.ChangePlayerSanity(_sanityPlayerMinus);
            _hadFollowed = false;
            _isGhostDisabled = true;
            _isFollowing = false;

            ChangeGhostSpeed(_ghostAttackSpeed);

            SwitchAttackState(true);
            StartCoroutine(nameof(CheckForPlayerVisible));
            Invoke(nameof(EnableAttackAfterCD), _ghostDisableTime);
        }

        public void StopAttackPatrolling()
        {
            if(_hadFollowed && !_playerKilled) _gameObjectives.EscapeGhostDuringHunt();
            _audioSteps.Pause();
            SwitchAttackState(false);
            StopCoroutine(nameof(CheckForPlayerVisible));
        }


        public void SwitchAttackState(bool isAttacking)
        {
            _isAttacking = isAttacking;
            if (!_agent.isOnNavMesh) return;
            if (isAttacking)
            {
                _audioControl.StartHeartBeat();
                _agent.isStopped = false;
            }
            else
            {
                _audioControl.StopHeartBeat();
                _agent.ResetPath();
                _agent.isStopped = true;
            }
        }

        public void SmudgeEffect()
        {
            _underSmudgeEffect = true;
            Invoke(nameof(StopSmudgeEffect), SmudgeEffectTime);
        }

        public void ChangeGhostSpeed(float speed)
        {
            _agent.speed = speed;
            _ghostCurrAttackSpeed = speed;
        }
        private void StopSmudgeEffect()
        {
            _underSmudgeEffect = false;
        }

        private void SetAudioSpeed()
        {
            _currAudioSpeed = _agent.speed / _avgGhostSpeed;
            AudioHelper.ChangeSoundSpeed(_audioSteps, _currAudioSpeed, AudioStepsGroup);
        }
        private IEnumerator CheckForPlayerVisible()
        {
            while (true)
            {
                if (!_isAttacking) break;
                if (!_isGhostDisabled)
                {
                    _playerCheckResult = _lineOfSight.CheckForPlayer(_playerPoint, _heroTransform);
                    if (_playerCheckResult.IsPlayerVisible && _playerCheckResult.DistanceToPlayer <= _maxAggroDistance)
                    {
                        _isFollowing = true;
                    }
                    else _isFollowing = false;
                }
                yield return _checkForLineWait;
            }
            yield return null;
        }

        private void CheckDistanceToPlayer()
        {
            _distanceToPlayer = Vector3.Distance(_heroTransform.position, transform.position);
        }

        private void SetHeartBeatVolume()
        {
            // Calculate heart beat volume percent, where if _distanceToPlayer <= MaxVolumeDistanceHeartBeat => 100%, and if _distanceToPlayer >= MinVolumeDistanceHeartBeat => 0%

            _heartBeatVolumePercent = ((_distanceToPlayer - MaxVolumeDistanceHeartBeat) * 100) / (MinVolumeDistanceHeartBeat - MaxVolumeDistanceHeartBeat);
            _heartBeatVolumePercent = 100f - _heartBeatVolumePercent;
            _heartBeatVolumePercent = Mathf.Clamp(_heartBeatVolumePercent, 0f, 100f);

            _audioControl.SetHeartBeatVolume(_heartBeatVolumePercent);
        }


        private void CheckForKill()
        {
            if (_underSmudgeEffect) return;

            if ((_distanceToPlayer < DistanceToKill) && !_playerKilled)
            {
                _gameFlow.GameOverAction?.Invoke();
                _playerKilled = true;
            }
        }
        private void EnableAttackAfterCD()
        {
            _audioSteps.Play();
            AudioHelper.ChangeSoundSpeed(_audioSteps, 1, AudioStepsGroup);

            _isGhostDisabled = false;
        }
        private void ChoosePoint()
        {
            _randomPointNum = Random.Range(0, _patrolPoints.Length - 1);
            _currDestination = _patrolPoints[_randomPointNum];
        }

        private void SetDestination()
        {
            if (_currDestination != null) _agent.SetDestination(_currDestination.position);
            else { Debug.Log("Curr destination = null"); }
        }

        private void SetUpGhostData()
        {
            _ghostMood = GetComponent<GhostMood>();

            _levelSetUp = AllServices.Container.Single<LevelSetUp>();
            _gameFlow = AllServices.Container.Single<GameFlowService>();
            _gameObjectives = AllServices.Container.Single<GameObjectivesService>();

            _checkForLineWait = new WaitForSeconds(_checkForLineCD);
            _patrolPoints = _levelSetUp.GetGhostPatrolPoints();
            if (_ghostInfo.SetedUp) SetUpParams();
            else
            {
                _ghostInfo.GhostSetedUp += SetUpParams;
            }
            _dataSetedUp = true;
        }
        private void SetUpParams()
        {
            _audioControl = _ghostInfo.MainHero.GetComponent<PlayerAudioControl>();

            _sanityPlayerMinus = _ghostInfo.GhostData.PlayerSanityMinusPerHunt;

            _playerPoint = _ghostInfo.PlayerPoint;
            _heroTransform = _ghostInfo.MainHero.transform;

            _ghostAttackSpeed = _ghostInfo.GhostData.GhostAttackSpeed;
            _maxAggroDistance = _ghostInfo.GhostData.MaxDistanceToPlayerAggr;

            _ghostDisableTime = _ghostInfo.CurrDifficulty.HuntSafeTime;
            // _playerTransform = _ghostInfo.
        }

    }
}