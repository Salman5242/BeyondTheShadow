using UnityEngine;
using Items.Logic;
using GameFeatures;
using Utilities.Constants;
using Infrastructure;
using Infrastructure.Services;
using Ghosts.Mood;

namespace Items.ItemsLogic
{
    [RequireComponent(typeof(PhotoReward))]
    public class Crucifix : MonoBehaviour, IPickupable, IDisababled
    {
        public bool IsConsumed = false;
        public float CrucifixPreventHuntChance = 75f;

        [SerializeField]
        private RoomIdentifire _roomIdentifire;
        [SerializeField]
        private Renderer _crucifixRenderer;
        [SerializeField]
        private GameObject _crucifixMesh;
        [SerializeField]
        private Color _consumedEmmisionColor = new Color(50,0,0);

        private GameObjectivesService _gameObjectives;

        private PhotoReward _photoReward;
        private GhostInfo _ghostInfo;
        private AttackChecker _ghostAttackChecker;
        private LevelRooms.LevelRoomsEnum _ghostRoom;

        private LevelSetUp _levelSetUp;

        private bool _isDisabled = false;
        private bool _isInGhostRoom = false;


        void Start()
        {
            _photoReward = GetComponent<PhotoReward>();
            _photoReward.enabled = false;

            _roomIdentifire.OnRoomChanged += OnCurrentRoomChanged;
            _levelSetUp = AllServices.Container.Single<LevelSetUp>();

            SetUpInfo();
            if (_ghostRoom == LevelRooms.LevelRoomsEnum.NoRoom) _levelSetUp.OnLevelSetedUp += SetUpInfo;
        }

        private void SetUpInfo()
        {
            _ghostInfo = _levelSetUp.GhostInfo;
            _ghostRoom = _levelSetUp.CurrGhostRoom;
            _gameObjectives = AllServices.Container.Single<GameObjectivesService>();

            if (_ghostInfo == null) return;
            _ghostAttackChecker = _ghostInfo.GetComponent<AttackChecker>();
        }

        private void OnDestroy()
        {
            _roomIdentifire.OnRoomChanged -= OnCurrentRoomChanged;
        }

        public bool CanBeConsumed()
        {
            if(!IsConsumed && !_isDisabled) return true;

            return false;
        }

        public void ConsumeCrucifix()
        {
            IsConsumed = true;
            _photoReward.enabled = true;
            _gameObjectives.CrucifixPreventedHunt();

            _crucifixRenderer.material.EnableKeyword("_EMISSION");
            _crucifixRenderer.material.SetColor("_EmissionColor", _consumedEmmisionColor);

        }

        void OnCurrentRoomChanged()
        {
            if (_roomIdentifire.CurrRoom == _ghostInfo.GhostRoom)
            {
                _isInGhostRoom = true;
                _ghostAttackChecker.AddCrucifix(this);
            }
            else if(_isInGhostRoom == true)
            {
                _isInGhostRoom = false;
                _ghostAttackChecker.RemoveCrucifix(this);
            }
        }

        public void DisableItem()
        {
            _crucifixMesh.SetActive(false);
            _isDisabled = true;
        }

        public void EnableItem()
        {
            _crucifixMesh.SetActive(true);
            _isDisabled = false;
        }
    }
}