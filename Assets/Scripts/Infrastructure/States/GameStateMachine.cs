using Infrastructure.Services;
using Infrastructure.States.GameStates;
using System;
using System.Collections.Generic;

namespace Infrastructure.States
{
    public class GameStateMachine
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        public GameStateMachine(AllServices services, ICoroutineRunner coroutineRunner)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootState)] = new BootState(services, coroutineRunner, this),
                [typeof(LobbyState)] = new LobbyState(this, services.Single<GameFactory>()),

                [typeof(LoadLevelState)] = new LoadLevelState(this, services.Single<GameFactory>(),
                services.Single<LevelSetUp>(), services.Single<SceneLoader>(), services.Single<StaticDataService>(), services.Single<GameObjectivesService>()),

                [typeof(GameFlowState)] = new GameFlowState(this, services.Single<LevelSetUp>(), services.Single<SceneLoader>(), 
                services.Single<GameFlowService>(), services.Single<GameObjectivesService>())
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }
        public IExitableState GetCurrentState() => _activeState;
        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
          _states[typeof(TState)] as TState;
    }
}