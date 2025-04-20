using Infrastructure.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Constants;
namespace Infrastructure.States.GameStates
{
    public class LobbyState : IState
    {

        private readonly GameStateMachine _stateMachine;
        private readonly GameFactory _gameFactory;

        public LobbyState(GameStateMachine stateMachine, GameFactory gameFactory)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
        }
        public void Enter()
        {
            InitGameWorld();
        }

        public void Exit()
        {
        }


        private async void InitGameWorld()
        {
            GameObject hero = await _gameFactory.CreateHero(GameObject.FindWithTag(Tags.InitialPoint));
             _gameFactory.CreateTargetUI();
        }
    }
}