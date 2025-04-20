using System;
using System.Collections;
using UnityEngine;
using Utilities;
using Utilities.Constants;

namespace Ghosts.Mood
{
    public class GhostEventChecker : MonoBehaviour
    {
        [SerializeField]
        private GhostInfo _ghostInfo;
        [SerializeField]
        private GhostMood _ghostMood;
        [SerializeField]
        private GhostStateMachine _ghostState;
        [SerializeField]
        private float _ghostEventCheckCD = 5f;

        private bool _attackInCD = false;
        private bool _subscribedToGhostSetUp = false;
        private GhostDataSO _ghostData;

        private LevelSizeConst.LevelSize _levelSize;

        private float _minHuntDuration;
        private float _maxHuntDuration;

        private SanityHandler _playerSanity;
        private WaitForSeconds GhostAttackCheckCD;
        void Start()
        {
            if (_ghostInfo.SetedUp) {  SetUp(); _subscribedToGhostSetUp = false; }
            else { _ghostInfo.GhostSetedUp += SetUp; _subscribedToGhostSetUp = true; }

            GhostAttackCheckCD = new WaitForSeconds(_ghostEventCheckCD);
        }

        private void OnDestroy()
        {
            if (_subscribedToGhostSetUp) _ghostInfo.GhostSetedUp -= SetUp;
        }

        public void MakeGhostEvent()
        {
            StartGhostEvent();
        }


        private IEnumerator CheckForGhostEventInum()
        {
            while (true)
            {
                yield return GhostAttackCheckCD;
                CheckForGhostEvent();
            }
        }

        private void CheckForGhostEvent()
        {
            if (_attackInCD) return;
            if (ShouldDoGhostEvent()) StartGhostEvent();
        }

        private void StartGhostEvent()
        {
            if (!(_ghostState._currState is IdleState)) return;
            if (_ghostInfo.PlayerRoom.CurrRoom == LevelRooms.LevelRoomsEnum.NoRoom) return;

            _ghostMood.StartGhostEvent();
        }


        private bool ShouldDoGhostEvent()
        {
            if (_playerSanity.Sanity < _ghostInfo.GhostData.MinSanityToAttack)
            {
                float attackChance = _ghostInfo.FinalGhostAnger * _ghostData.GhostEventChanceCoef;

                if (RandomGenerator.CalculateChance(attackChance) == true) return true;
            }
            return false;
        }

        private void SetUp()
        {
            _playerSanity = _ghostInfo.PlayerSanity;
            _levelSize = _ghostInfo.LevelSize;
            _ghostData = _ghostInfo.GhostData;

            _minHuntDuration = LevelSizeConst.MapMinHuntDuration(_levelSize.ToString());
            _maxHuntDuration = LevelSizeConst.MapMaxHuntDuration(_levelSize.ToString());

            StartCoroutine(nameof(CheckForGhostEventInum));
        }
    }
}