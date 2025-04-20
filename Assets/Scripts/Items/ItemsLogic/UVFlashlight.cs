using Items.Logic;
using UnityEngine;
using System.Collections;

namespace Items.ItemsLogic
{
    public class UVFlashlight : MonoBehaviour, IMainUsable, IDroppable
    {
        [SerializeField] private UVLight _uvLight;

        [SerializeField]
        private Light _light;


        [SerializeField]
        public Material _revealableMaterial;

        private AudioSource _audioSource;

        private bool _isEnabled = false;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        public void OnMainUse()
        {
            if (_isEnabled == false)
            {
                _uvLight.EnableUVLight();
                _light.enabled = true;
                _isEnabled = true;
            }
            else
            {
                _uvLight.DisableUVLight();
                _light.enabled = false;
                _isEnabled = false;
            }

            _audioSource.Play();
        }
        private void OnDisable()
        {
            _uvLight.DisableUVLight();
        }
        private void OnEnable()
        {
            if (_isEnabled) _uvLight.EnableUVLight();
        }

        public void DropItem()
        {
            if (_isEnabled) _uvLight.EnableUVLight();
        }
    }
}