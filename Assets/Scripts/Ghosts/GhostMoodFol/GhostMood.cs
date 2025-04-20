using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using UnityEngine;

namespace Ghosts.Mood
{
    public class GhostMood : MonoBehaviour
    {
        [SerializeField]
        private GhostInfo _ghostInfo;
        [SerializeField]
        private GhostState _attackState, _idleState, _ghostEventState;
        [SerializeField]
        private GhostStateMachine _ghostStateMachine;
        [SerializeField]
        private AttackChecker _attackChecker;
        [SerializeField]
        private AttackPatrol _attackPatrol;
        private const int SanityDivider = 2;
        private const float SmudgeTime = 140f;

        public bool IsHunting = false;
        private bool _subscribedToGhostSetUp = false;
        private bool _isUnderSmudgeEffect = false;

        private float _huntTime = 0f;

        private SanityHandler _playerSanity;
        private GameObjectivesService _gameObjectives;

        public float _ghostAnger = 0f;
        public float _ghostFinalAnger = 0f;
        void Start()
        {
            _gameObjectives = AllServices.Container.Single<GameObjectivesService>();
            _ghostStateMachine.ChangeState(_idleState);
            if (_ghostInfo.SetedUp) { SetUp(); _subscribedToGhostSetUp = false; }
            else { _ghostInfo.GhostSetedUp += SetUp; _subscribedToGhostSetUp = true; }
        }

        private void OnDestroy()
        {
            if (_subscribedToGhostSetUp) _ghostInfo.GhostSetedUp -= SetUp;
        }

        private void Update()
        {
            if(_playerSanity != null) _ghostInfo.FinalGhostAnger = Mathf.Max(0f, ( (100 - _playerSanity.Sanity) * 0.8f) + (_ghostAnger / 3));

            _ghostFinalAnger = _ghostInfo.FinalGhostAnger;
        }

        public void ChangePlayerSanity(float sanityToAdd)
        {
            _playerSanity.TakeSanity(sanityToAdd);
        }
        public void SetGhostAnger(float ghostAnger)
        {
            _ghostAnger = ghostAnger;
        }

        public void StartHunting(float huntTime)
        {
            _huntTime = huntTime;
            IsHunting = true;
            _ghostStateMachine.ChangeState(_attackState);

            Invoke(nameof(StopHunting), huntTime);
        }

        public void StopHunting()
        {
            IsHunting = false;
            _ghostStateMachine.ChangeState(_idleState);
        }

        public void StartGhostEvent()
        {
            _ghostStateMachine.ChangeState(_ghostEventState);
        }

        public void StopGhostEvent()
        {
            _ghostStateMachine.ChangeState(_idleState);
        }

        public void SmudgeEffect()
        {
            _gameObjectives.GhostSmudged();
            _attackChecker.SmudgeEffect(SmudgeTime);
            if(_ghostStateMachine._currState is AttackState)
            {
                _attackPatrol.SmudgeEffect();
            }
        }

        private void SetUp()
        {
            _playerSanity = _ghostInfo.PlayerSanity;
        }
    }
}