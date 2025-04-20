using Items.Logic;
using UnityEngine;
using Infrastructure.Services;
using Infrastructure;

namespace Items.ItemsLogic
{
    public class SanityPills : MonoBehaviour, IMainUsable
    {
        [SerializeField]
        private float _sanityToAdd = 30;

        private Consumable _consumable;
        private SanityHandler _playerSanity;
        private LevelSetUp _levelSetUp;
        private void Start()
        {
            _levelSetUp = AllServices.Container.Single<LevelSetUp>();
            _consumable = GetComponent<Consumable>();
            if (_levelSetUp.MainPlayer == null) _levelSetUp.OnLevelSetedUp += SetUp;
            else SetUp();
        }

        private void OnDestroy()
        {
            _levelSetUp.OnLevelSetedUp -= SetUp;
        }

        public void OnMainUse()
        {
            if (_playerSanity != null)
            {
                _playerSanity.AddSanity(_sanityToAdd);
                _consumable.Consume();
            }
        }

        private void SetUp()
        {
            _playerSanity = _levelSetUp.MainPlayer.GetComponent<SanityHandler>();
        }
    }
}