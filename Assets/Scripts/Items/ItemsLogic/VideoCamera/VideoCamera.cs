using Items.Logic;
using UnityEngine;
using System.Collections;
using System;

namespace Items.ItemsLogic
{
    public class VideoCamera : MonoBehaviour, IMainUsable, IDroppable
{
        
        [SerializeField] private Camera _camera;
        [SerializeField] private Camera _cameraGreen;
        [SerializeField] private MeshRenderer _screenMesh;
        
        
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private Material _renderTextureMat;

        private bool _normalMode = true;

        private void Start()
        {
            SetUpCamera();
            DisableCamera();
        }
        public void OnMainUse()
        {
            if (_normalMode)
            {
                SetGhostOrbMode();
            }
            else
            {
                SetNormalMode();
            }
        }


        private void SetNormalMode()
        {
            _renderTextureMat.color = Color.white;
            _cameraGreen.gameObject.SetActive(false);
            _camera.gameObject.SetActive(true);
            _normalMode = true;
        }
        private void SetGhostOrbMode()
        {
            _renderTextureMat.color = Color.green;
            _camera.gameObject.SetActive(false);
            _cameraGreen.gameObject.SetActive(true);
            _normalMode = false;
        }
        private void DisableCamera()
        {
            _camera.gameObject.SetActive(false);
            _cameraGreen.gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            if (_normalMode) SetNormalMode();
            else SetGhostOrbMode();
        }

        public void DropItem()
        {
            DisableCamera();
        }

        private void SetUpCamera()
        {
            _camera.targetTexture = _renderTexture;
            _cameraGreen.targetTexture = _renderTexture;
            _screenMesh.material = _renderTextureMat;
        }
    }
}
