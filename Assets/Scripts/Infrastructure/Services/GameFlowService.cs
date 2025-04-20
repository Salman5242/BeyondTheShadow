using Infrastructure;
using Infrastructure.States;
using Infrastructure.States.GameStates;
using System;
using System.Collections;
using UnityEngine;
using Utilities.Constants;

namespace Infrastructure.Services
{
    public class GameFlowService : IService
    {
        public bool Died = false;
        public bool IsGameEnded = false;

        public Action GameOverAction;
        public Action WinAction;

        private SceneLoader _sceneLoader;
        private GameStateMachine _gameStateMachine;
        private GameObjectivesService _gameObjectivesService;
        private ICoroutineRunner _coroutineRunner;

        private int _mapSize = 0;
        private DifficultySO _difficulty;

        private float[] _rewardValues = new float[6];
        private float[] _objectives = new float[3];
        private float _photoReward = 0f;
        private float _totalReward = 0f;
        private float _ghostReward = 20f;
        private float _insuranceReward = 0f;
        private float _insurancePercent = 0f;
        private float _deathCoef = 0.25f;
        private float _difficultyCoef = 1f;
        private float _mapSizeCoef = 1f;

        public GameFlowService(ICoroutineRunner coroutineRunner, GameObjectivesService gameObjectivesService)
        {
            _gameObjectivesService = gameObjectivesService;
            _coroutineRunner = coroutineRunner;

            GameOverAction += GameOverCall;
            WinAction += WinGameCall;
        }

        public void FirstTimeSetUp(SceneLoader sceneLoader, GameStateMachine gameStateMachine)
        {
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
        }

        public GhostDataSO GhostDataSO
        {
            get { return _currentGhostSO; }
        }

        private const float DelayBeforeGoToLobby = 2.5f;

        private GhostInfo _currentGhostInfo;
        private GhostDataSO _currentGhostSO;
        private GhostDataSO _currentGhostChoosenSO;

        public void SetUpGameFlowService(GhostInfo currentLevelGhost)
        {
            _currentGhostInfo = currentLevelGhost;
            _currentGhostSO = _currentGhostInfo.GhostData;
        }

        public void ChangeCurrentChoosenGhost(GhostDataSO ghostChoosen)
        {
            _currentGhostChoosenSO = ghostChoosen;
        }

        public bool IsChooseCorrect()
        {
            if(_currentGhostChoosenSO != null)
            {
                if (_currentGhostChoosenSO.name == _currentGhostSO.name)
                {
                    return true;
                }
            }
            return false;
        }

        public float[] GetRewardValues()
        {
            if (IsChooseCorrect()) _rewardValues[0] = _ghostReward; //obj 1 - ghost
            else _rewardValues[0] = 0f;

            for (int i = 1; i < 4; i++)
            _rewardValues[i] = _gameObjectivesService.CurrObjectives[i-1].IsDone ? _gameObjectivesService.CurrObjectives[i - 1].ObjectiveReward : 0f; //objectives

            _rewardValues[4] = _photoReward; //photo
            _rewardValues[5] = _insuranceReward; //insurance

            return _rewardValues;
        }

        public float CalculateTotalReward()
        {
            _totalReward = 0;
            for (int i = 0; i < _rewardValues.Length; i++) _totalReward += _rewardValues[i];

            _mapSizeCoef = (_mapSize + 1f) / 2f;
            _difficultyCoef = _difficulty.RewardCoef;
            _insurancePercent = _difficulty.InsurancePercent;

            _totalReward = Mathf.Round(( _totalReward * _difficultyCoef * _mapSizeCoef));

            if (Died) _totalReward = Mathf.Round((_totalReward * _deathCoef) + _insuranceReward);
            return _totalReward;
        }

        public void AddPhotoReward(float value)
        {
            _photoReward += value;
        }
        public void CalculateInsurance(float itemsCost)
        {
            if (Died) _insuranceReward =  Mathf.Round(MathfHelper.CalculatePercent(itemsCost, _insurancePercent));
        }
        public void ClearRewards()
        {
            for (int i = 0; i < _rewardValues.Length; i++)
            {
                _rewardValues[i] = 0f;
            }
            for (int i = 0; i < _objectives.Length; i++)
            {
                _objectives[i] = 0f;
            }
            _photoReward = 0f;
            _totalReward = 0f;
            _insuranceReward = 0f;
            _currentGhostChoosenSO = null;
        }

        public void SetMapSize(int mapSize)
        {
            _mapSize = mapSize;
        }
        public float GetMapSizeCoef()
        {
            return _mapSizeCoef;
        }
        public void SetDifficulty(DifficultySO difficulty)
        {
            _difficulty = difficulty;
        }
        public float GetDifficultyCoef()
        {
            return _difficultyCoef;
        }
        private void GameOverCall()
        {
            Died = true;
            _coroutineRunner.StartCoroutine(EndGame());
        } 
        private void WinGameCall()
        {
            Died = false;
            _coroutineRunner.StartCoroutine(EndGame());
        }

        private IEnumerator EndGame()
        {
            IsGameEnded = true;
            yield return new WaitForSeconds(DelayBeforeGoToLobby);
            ActivateLobby();
        }

        private void ActivateLobby()
        {
            _sceneLoader.Load(SceneNames.LobbyScene, _gameStateMachine.Enter<LobbyState>);
        }
    }
}