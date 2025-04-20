using Infrastructure;
using UnityEngine;
using System;
using System.Collections;
using Infrastructure.Services;

namespace Player.Movement
{
    public class MoveControl : MonoBehaviour
    {
        [SerializeField]
        private CharacterController _charController;

        [SerializeField]
        private Transform _playerBody;
        [SerializeField]
        private Transform _playerHead;

        [SerializeField]
        private Transform _playerHuntPoint;

        [SerializeField]
        private Transform _playerBoneHead;
        [SerializeField]
        private float _mouseSensitivity = 100f;

        [SerializeField]
        private float _normalMoveSpeed = 2.8f;
        [SerializeField]
        private float _sprintMultiplier;

        [SerializeField]
        private float _maxSprintDuration = 3.5f;
        [SerializeField]
        private float _sprintCD = 5f;

        [SerializeField]
        private AnimationControl _animConrol;
        [SerializeField]
        private AudioSource _breathingAudio;
        [SerializeField]
        private AudioSource _footstepsAudio;

        [SerializeField]
        private AudioSource _footstepsFastAudio;

        private float _sprintRestMultiplayer;
        private float _sprintRestWhileSprintingMultiplayer;
        private float _currSprintDuration = 0f;

        private float _curSpeedMultiplier = 1f;
        private float _mouseX, _mouseY;
        private Vector2 _mouseDelta;
        private float _xRotation = 0f;

        private Vector3 _headPosition;
        private Vector3 _currSpeed;

        private float _currFollowHeadTime = 0f;

        private const float Epsilon = 0.01f;
        private const float SprintCDDivider = 5f;
        private const float MinTimeForRest = 2f;
        private const float FollowHeadTime = 2f;
        private InputSystem _inputSystem;

        private float _timeFromLastSprint = 0f;
        private Transform cameraTransform;
        private float _xMove, _zMove;
        private bool _isSprintPressed;
        private bool _sprint = false;

        private float _playerHeadOffset = 0f;
        private bool _crouch = false;
        private bool _isSprintLocked = false;

        private void Start()
        {
            _inputSystem = AllServices.Container.Single<InputSystem>();
            cameraTransform = Camera.main.transform;
            _sprintRestMultiplayer = (_maxSprintDuration / _sprintCD);
            _inputSystem.CrouchAction += CrouchHandle;

            _playerHeadOffset = _playerHead.position.y - _playerBoneHead.position.y;

            _footstepsAudio.volume = 0f;
            _footstepsAudio.Play();

            _footstepsFastAudio.volume = 0f;
            _footstepsFastAudio.Play();
        }

        private void OnDestroy()
        {
            _inputSystem.CrouchAction -= CrouchHandle;
        }
        void Update()
        {
            InputMouseMove();
            InputMove();
            SprintCalc();
            FollowHead();

            PlayerRotation();
            PlayerMovement();
        }

        public Transform GetPlayerHuntPoint()
        {
            return _playerHuntPoint;
        }
        private void InputMove()
        {
            _xMove = _inputSystem.Axis.x;
            _zMove = _inputSystem.Axis.y;
            _isSprintPressed = _inputSystem.IsRunning;
        }

        private void InputMouseMove()
        {
            _mouseDelta = _inputSystem.CameraAxis;
            _mouseX = _mouseDelta.x * _mouseSensitivity;
            _mouseY = _mouseDelta.y * _mouseSensitivity;
        }

        private void CrouchHandle()
        {
            if (!_crouch)
            {
                if (!_animConrol.SitDown()) return;
                _crouch = true;
            }
            else
            {
                if (!_animConrol.StandUp()) return;
                _crouch = false;
            }
            _currFollowHeadTime = FollowHeadTime;
        }

        private void FollowHead()
        {
            if (_currFollowHeadTime > 0f)
            {
                SetHeadPosition();
                _currFollowHeadTime -= Time.deltaTime;
            }
        }


        private void SetHeadPosition()
        {
            _headPosition = _playerHead.position;
            _headPosition.y = _playerBoneHead.position.y + _playerHeadOffset;
            _playerHead.position = _headPosition;
        }

        private void SprintCalc()
        {
            if (_isSprintPressed)
            {
                if (_isSprintLocked)
                {
                    MinusCurrSprintDuration();
                    if (_currSprintDuration <= Epsilon) _isSprintLocked = false;
                }
                else
                {
                    _currSprintDuration += Time.deltaTime;
                    if (_currSprintDuration >= _maxSprintDuration)
                    {
                        _breathingAudio.Play();
                        _isSprintLocked = true;
                    }
                }
                _sprint = !_isSprintLocked;
                if (_sprint) _timeFromLastSprint = 0f;
                else _timeFromLastSprint += Time.deltaTime;
            }
            else
            {
                _sprint = false;
                if (_currSprintDuration > Epsilon) MinusCurrSprintDuration();
                else _isSprintLocked = false;
            }
            _timeFromLastSprint += Time.deltaTime;
        }


        private void MinusCurrSprintDuration()
        {
            _sprintRestWhileSprintingMultiplayer = _timeFromLastSprint >= MinTimeForRest ? 1f : _timeFromLastSprint / SprintCDDivider;
            if (_isSprintLocked) _currSprintDuration -= Time.deltaTime * _sprintRestMultiplayer;
            else _currSprintDuration -= Time.deltaTime * _sprintRestWhileSprintingMultiplayer;
            _currSprintDuration = Mathf.Max(0, _currSprintDuration);
        }


        private Vector3 VectorToForward(Vector3 cameraTransform)
        {
            Vector3 returnVector;
            returnVector.x = -cameraTransform.z;
            returnVector.y = 0;
            returnVector.z = cameraTransform.x;
            return returnVector;
        }
        private void PlayerRotation()
        {
            _playerBody.Rotate(Vector3.up * _mouseX);

            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _playerHead.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        private void PlayerMovement()
        {
            Vector3 moveDir = CalculateMove(_sprint);
            _currSpeed = moveDir * _normalMoveSpeed * _curSpeedMultiplier;

            if (_currSpeed.magnitude > 0) AdjustFootstepsAudio(_curSpeedMultiplier);
            else AdjustFootstepsAudio(_curSpeedMultiplier, true);
            _charController.Move(_currSpeed * Time.deltaTime);
        }

        private Vector3 CalculateMove(bool sprint)
        {
            if (sprint) _curSpeedMultiplier = _sprintMultiplier;

            else _curSpeedMultiplier = 1f;

            Vector3 result = transform.right * _xMove + transform.forward * _zMove;
            return result;
        }

        private void AdjustFootstepsAudio(float playerSpeed, bool disable = false)
        {
            if (disable)
            {
                 _footstepsAudio.volume = 0f;
                 _footstepsFastAudio.volume = 0f;
                return;
            }
            if (playerSpeed <= 1)
            {
                 _footstepsFastAudio.volume = 0f;
                 _footstepsAudio.volume = 1f;
            }
            else
            {
                _footstepsAudio.volume = 0f;
                _footstepsFastAudio.volume = 1f;
            }
        }
    }
}