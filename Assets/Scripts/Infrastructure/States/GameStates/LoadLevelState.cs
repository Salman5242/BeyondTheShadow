using GameFeatures;
using Infrastructure.Services;
using Player.Movement;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Constants;

namespace Infrastructure.States.GameStates
{
    public class LoadLevelState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly GameFactory _gameFactory;
        private readonly GameObjectivesService _gameObjectivesService;
        private readonly LevelSetUp _levelSetUp;
        private readonly SceneLoader _sceneLoader;
        private readonly StaticDataService _staticDataService;
        private GameStateMachine gameStateMachine;

        public LoadLevelState(GameStateMachine stateMachine, GameFactory gameFactory, LevelSetUp levelSetUp, SceneLoader sceneLoader, StaticDataService staticDataService, GameObjectivesService gameObjectivesService)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _levelSetUp = levelSetUp;
            _gameObjectivesService = gameObjectivesService;
            _sceneLoader = sceneLoader;
            _staticDataService = staticDataService;
        }

        public void Enter()
        {
            _gameObjectivesService.SetUpObjectives();
            _sceneLoader.Load(_levelSetUp.SelectedMap.ToString(), InitGameWorld);
            //_sceneLoader.Load(SceneNames.LevelNames.Factory.ToString(), InitGameWorld);
        }

        public void Exit()
        {
        }

        private void InitGameWorld()
        {
            _levelSetUp.InitializeLevel();
            InstantiateAll();
        }

        private void LevelCreatedEvent()
        {
            _levelSetUp.IsInitialized = true;
            _levelSetUp.OnLevelSetedUp.Invoke();
            _stateMachine.Enter<GameFlowState>();
        }

        private async void InstantiateAll()
        {
            GameObject hero = await _gameFactory.CreateHero(GameObject.FindWithTag(Tags.InitialPoint));
            GameObject ghost = await _gameFactory.CreateGhost(GameObject.FindWithTag(Tags.GhostInitialPoint));
            GameObject journal = await _gameFactory.CreateJournal();
            await _gameFactory.CreateJumpscare();

            ghost.GetComponent<GhostInfo>().SetUpGhost(_levelSetUp.SelectedDifficulty, _staticDataService.GetRandomGhost(), hero, hero.GetComponent<MoveControl>().GetPlayerHuntPoint(), _levelSetUp.CurrGhostRoom, hero.GetComponent<RoomIdentifire>(), hero.GetComponent<SanityHandler>(), _levelSetUp.CurrLevelSize, _levelSetUp.MainDoors, _levelSetUp.LightButtons);
            _levelSetUp.GhostInfo = ghost.GetComponent<GhostInfo>();
            if(_levelSetUp.GhostInfo == null) { Debug.Log("Here bochok potik"); }
            _levelSetUp.MainPlayer = hero;

            LevelCreatedEvent();
        }
    }
}