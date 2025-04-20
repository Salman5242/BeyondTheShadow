using Infrastructure.Services;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.States.GameStates
{
    public class BootState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly AllServices _services;
        private readonly ICoroutineRunner _coroutineRunner;
        private DataSaveLoader _dataSaveLoader;
        public BootState(AllServices services, ICoroutineRunner coroutineRunner, GameStateMachine gameStateMachine)
        {
            _services = services;
            _coroutineRunner = coroutineRunner;
            _gameStateMachine = gameStateMachine;
            RegisterServices();
            LockFPS();
            LoadGameInfo();
        }

        public void Enter()
        {
            CheckLobbyScene();
        }

        public void Exit()
        {
        }

        private async void RegisterServices()
        {
            RegisterStaticDataService();
            _services.RegisterSingle<GameObjectivesService>(new GameObjectivesService(_services.Single<StaticDataService>()));

            _services.RegisterSingle<GameFlowService>(new GameFlowService(_coroutineRunner, _services.Single<GameObjectivesService>()));
            _services.RegisterSingle<AssetProvider>(new AssetProvider());
            _services.RegisterSingle<DataSaveLoader>(new DataSaveLoader());
            _services.RegisterSingle<SceneLoader>(new SceneLoader(_coroutineRunner));
            _services.RegisterSingle<GameFactory>(new GameFactory(_services.Single<AssetProvider>()));

            _services.RegisterSingle<LevelSetUp>(new LevelSetUp(_services.Single<GameObjectivesService>()));
            GameObject inputSystemGameObject = await _services.Single<GameFactory>().CreateInputSystem();
            _services.RegisterSingle<InputSystem>(inputSystemGameObject.GetComponent<InputSystem>());

            GameObject audioManager = await _services.Single<GameFactory>().CreateAudioManager();
            _services.RegisterSingle<AudioManager>(audioManager.GetComponent<AudioManager>());
        }

        private void RegisterStaticDataService()
        {
            StaticDataService staticData = new StaticDataService();
            staticData.Load();
            _services.RegisterSingle(staticData);
        }

        private void LockFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 140;
        }

        private void LoadGameInfo()
        {
            _dataSaveLoader = _services.Single<DataSaveLoader>();
            _dataSaveLoader.LoadInfo();
        }

        private void CheckLobbyScene()
        {
            //_gameStateMachine.Enter<LobbyState>();
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                _gameStateMachine.Enter<LobbyState>();
            }
        }
    }
}