using Infrastructure;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Infrastructure.Services
{
    public class InputSystem : MonoBehaviour, IService
    {
        public Vector2 Axis { get; private set; }
        public Vector2 CameraAxis { get; private set; }
        public bool IsRunning = false;

        public Action PickUpItemAction, DropItemAction, MainUseAction, MainUseCanceledAction,
        PrimaryUseAction, SecondaryUseAction, SwitchItemAction, CrouchAction, JournalOpenAction, BoardOpenAction,
        FlashLightAction;

        public bool IsControlLocked { get; private set; }
        private MainInputAction _mainInputAction;

        private void Awake()
        {
            _mainInputAction = new MainInputAction();
            _mainInputAction.Player.Enable();

            BindFuncs();
            DontDestroyOnLoad(this);
        }


        private void Update()
        {
            ReadControl();
        }


        private void ReadControl()
        {
            if (!IsControlLocked)
            {
                CameraAxis = _mainInputAction.Player.Camera.ReadValue<Vector2>();
                Axis = _mainInputAction.Player.Movement.ReadValue<Vector2>();
                IsRunning = _mainInputAction.Player.Run.ReadValue<float>() == 1 ? true : false;
            }
        }

        public void LockControl()
        {
            IsControlLocked = true;
            CameraAxis = Vector2.zero;
            Axis = Vector2.zero;
            IsRunning = false;
        }

        public void UnLockControl()
        {
            IsControlLocked = false;
        }
        private void BindFuncs()
        {
            _mainInputAction.Player.DropItem.performed += DropItemCallBack;
            _mainInputAction.Player.Pickup.performed += PickUpItemCallBack;

            _mainInputAction.Player.MainUse.performed += MainUseCallBack;
            _mainInputAction.Player.MainUse.canceled += MainUseCanceledCallBack;

            _mainInputAction.Player.PrimaryUse.performed += PrimaryUseCallBack;
            _mainInputAction.Player.SecondaryUse.performed += SecondaryUseCallBack;
            _mainInputAction.Player.SwitchItem.performed += SwitchItemUseCallBack;

            _mainInputAction.Player.Flashlight.performed += FlashLightUseCallBack;
            _mainInputAction.Player.Crouch.performed += CrouchCallBack;
            _mainInputAction.Player.Journal.performed += JournalOpen;

            _mainInputAction.Player.BoardOpen.performed += BoardOpenCallBack;
        }

        private void JournalOpen(InputAction.CallbackContext obj)
        {
            if (JournalOpenAction != null) JournalOpenAction.Invoke();
        }
        private void CrouchCallBack(InputAction.CallbackContext obj)
        {
            if (CrouchAction != null) CrouchAction.Invoke();
        }

        private void DropItemCallBack(InputAction.CallbackContext obj)
        {
            if (DropItemAction != null) DropItemAction.Invoke();
        }

        private void PickUpItemCallBack(InputAction.CallbackContext obj)
        {
            if (PickUpItemAction != null) PickUpItemAction.Invoke();
        }

        private void MainUseCallBack(InputAction.CallbackContext obj)
        {
            if (MainUseAction != null) MainUseAction.Invoke();
        }

        private void MainUseCanceledCallBack(InputAction.CallbackContext obj)
        {
            if (MainUseCanceledAction != null) MainUseCanceledAction.Invoke();
        }

        private void PrimaryUseCallBack(InputAction.CallbackContext obj)
        {
            if (PrimaryUseAction != null) PrimaryUseAction.Invoke();
        }

        private void SecondaryUseCallBack(InputAction.CallbackContext obj)
        {
            if (SecondaryUseAction != null) SecondaryUseAction.Invoke();
        }

        private void FlashLightUseCallBack(InputAction.CallbackContext obj)
        {
            if (FlashLightAction != null) FlashLightAction.Invoke();
        }

        private void SwitchItemUseCallBack(InputAction.CallbackContext obj)
        {
            if (SwitchItemAction != null) SwitchItemAction.Invoke();
        }

        private void BoardOpenCallBack(InputAction.CallbackContext obj)
        {
            if (BoardOpenAction != null) BoardOpenAction.Invoke();
        }
    }
}