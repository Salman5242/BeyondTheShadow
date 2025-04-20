using Items.ItemsLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Constants;

namespace Ghosts.Mood
{
    public class AttackChecker : MonoBehaviour
    {
        private const int MinAttackDurationDivider = 6;
        private const int MaxAttackDurationDivider = 3;
        [SerializeField]
        private GhostInfo _ghostInfo;
        [SerializeField]
        private GhostMood _ghostMood;
        [SerializeField]
        private GhostStateMachine _ghostState;

        [SerializeField]
        private float _ghostAttackCheckCD = 5f;

        private bool _attackInCD = false;
        private bool _underSmudgeEffect = false;
        private bool _subscribedToGhostSetUp = false;

        private GhostDataSO _ghostData;
        private LevelSizeConst.LevelSize _levelSize;

        private List<Crucifix> _crucifixInRoom = new List<Crucifix>();

        private float _minHuntDuration;
        private float _maxHuntDuration;

        private float _huntDurationModifier;
        private float _cantAttackTime;
        private SanityHandler _playerSanity;
        private WaitForSeconds GhostAttackCheckCD;
        void Start()
        {
            if (_ghostInfo.SetedUp) { SetUp(); _subscribedToGhostSetUp = false; }
            else { _ghostInfo.GhostSetedUp += SetUp; _subscribedToGhostSetUp = true; }

            GhostAttackCheckCD = new WaitForSeconds(_ghostAttackCheckCD);
        }

        private void OnDestroy()
        {
            if (_subscribedToGhostSetUp) _ghostInfo.GhostSetedUp -= SetUp;
        }

        public void AddCrucifix(Crucifix crucifix)
        {
            _crucifixInRoom.Add(crucifix);
        }

        public void RemoveCrucifix(Crucifix crucifix)
        {
            _crucifixInRoom.Remove(crucifix);
        }

        public void MakeGhostHunt()
        {
            StartHunting(CalculateAttackTime());
        }

        public void SmudgeEffect(float smudgeTime)
        {
            _underSmudgeEffect = true;
            Invoke(nameof(StopSmudgeEffect), smudgeTime);
        }

        private void StopSmudgeEffect() => _underSmudgeEffect = false;

        private IEnumerator CheckForAttackInum()
        {
            while (true)
            {
                yield return GhostAttackCheckCD;
                CheckForAttack();
            }
        }

        private void CheckForAttack()
        {
            if (_attackInCD || _underSmudgeEffect) return;

            if (ShouldHunt())
            {
                if(!CheckForCrucifix())
                StartHunting(CalculateAttackTime());
            }
        }

        private float CalculateAttackTime()
        {
            return UnityEngine.Random.Range( (_minHuntDuration * _huntDurationModifier) + _ghostInfo.FinalGhostAnger / MinAttackDurationDivider, (_maxHuntDuration * _huntDurationModifier) + _ghostInfo.FinalGhostAnger / MaxAttackDurationDivider);
        }

        private void StartHunting(float attackTime)
        {
            if (!(_ghostState._currState is IdleState)) return;
            if (_ghostInfo.PlayerRoom.CurrRoom == LevelRooms.LevelRoomsEnum.NoRoom) return;

            _attackInCD = true;

            _ghostMood.StartHunting(attackTime);
            Invoke(nameof(ReloadAttackCD), _ghostData.MinAttackCD);
        }

        private void ReloadAttackCD()
        {
            _attackInCD = false;
        }

        private bool ShouldHunt()
        {
            if (_playerSanity.Sanity < _ghostInfo.GhostData.MinSanityToAttack)
            {
                float attackChance = _ghostInfo.FinalGhostAnger * _ghostData.AttackChanceCoef;

                if (RandomGenerator.CalculateChance(attackChance) == true) return true;
            }
            return false;
        }

        private bool CheckForCrucifix()
        {
            foreach(Crucifix crucifix in _crucifixInRoom)
            {
                if (crucifix.CanBeConsumed())
                {
                    if (RandomGenerator.CalculateChance(crucifix.CrucifixPreventHuntChance) == true)
                    {
                        crucifix.ConsumeCrucifix();
                        return true;
                    }
                }
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
            _huntDurationModifier = _ghostInfo.CurrDifficulty.HuntDurationModifier;

            _attackInCD = true;
            Invoke(nameof(ReloadAttackCD), _ghostInfo.CurrDifficulty.GameGracePeriod);
            StartCoroutine(nameof(CheckForAttackInum));
        }
    }
}