using UnityEngine;
using Cinemachine;
using Infrastructure;
using Utilities.Constants;
using Items.Logic;
using Infrastructure.Services;

namespace Environment
{
    public class BoardClickable : MonoBehaviour, IClickable
    {
        [SerializeField] private CinemachineVirtualCamera _boardCameraCinemachime;

        private InputSystem _inputSystem;
        private bool _isInBoard = false;

        private void Start()
        {
            _inputSystem = AllServices.Container.Single<InputSystem>();
            _inputSystem.BoardOpenAction += OnSpacePressed;
        }
        private void OnDestroy()
        {
            _inputSystem.BoardOpenAction -= OnSpacePressed;
        }
        public void OnClick()
        {
            if (!_isInBoard)
            {
                LookAtBoard();
            }
        }
        public void OnSpacePressed()
        {
            if (!_isInBoard) LookAtBoard();
            else DecreasePriority();
        }

        public void DecreasePriority()
        {
            _boardCameraCinemachime.Priority = CameraPriorities.DisabledState;
            UnlockInputControl();
            _isInBoard = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        public void LookAtBoard()
        {
            if (_boardCameraCinemachime.gameObject.activeInHierarchy == false) _boardCameraCinemachime.gameObject.SetActive(true);
            if (_inputSystem == null) _inputSystem = AllServices.Container.Single<InputSystem>();

            _boardCameraCinemachime.Priority = CameraPriorities.ActiveState;
            _inputSystem.LockControl();
            _isInBoard = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void SetInBoard()
        {
            _isInBoard = true;
        }
        public void UnlockInputControl()
        {
            _inputSystem.UnLockControl();
        }
    }
}