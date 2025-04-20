using Infrastructure;
using Infrastructure.Services;
using Infrastructure.States.GameStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class CursorLock : MonoBehaviour
    {
        private GameFlowService _gameFlowService;
        private bool cursorLock;
        private void Start()
        {
            if (AllServices.Container.Single<GameFlowService>().IsGameEnded == true && GameBootstrapper.Instance.StateMachine.GetCurrentState() is LobbyState)
            {
                UnlockCursor();
            }
            else LockCursor();
        }
        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            cursorLock = false;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            cursorLock = true;
        }
    }
}