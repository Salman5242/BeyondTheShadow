using Items.Logic;
using UnityEngine;
using System.Collections;

namespace Items.ItemsLogic
{
    public class UVGlowstick : MonoBehaviour, IMainUsable, IDroppable
    {
        [SerializeField] private UVLight _uvLight;

        [SerializeField]
        private Light _light;

        [SerializeField]
        private Material _glowOnMaterial;

        [SerializeField]
        private MeshRenderer _glowBody;

        private bool _isEnabled = false;

        public void OnMainUse()
        {
            if (_isEnabled == false)
            {
                _light.enabled = true;
                _isEnabled = true;
                _uvLight.EnableUVLight();
                _glowBody.material = _glowOnMaterial;
            }
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