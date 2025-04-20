using GameFeatures;
using Items.Logic;
using UnityEngine;
using System.Collections;
using Utilities.Constants;
using Infrastructure;
using Infrastructure.Services;

namespace Items.ItemsLogic
{
    public class Book : MonoBehaviour, IPickupable, IDisababled
    {
        private GhostInfo _ghostInfo;
        private LevelRooms.LevelRoomsEnum _ghostRoom;

        private LevelSetUp _levelSetUp;

        [SerializeField] 
        private Material[] _inscribedMaterials;

        [SerializeField] 
        private MeshRenderer _bookMeshRenderer;
        private Material[] _meshMaterials;


        [SerializeField]
        private RoomIdentifire _currRoom;
        [SerializeField] 
        private float _defaultInscribeChance = 2f;
        [SerializeField] 
        private float _increaseChancePerTick = 0.1f;
        [SerializeField] 
        private float _tickTime = 5f;

        [SerializeField]
        private float _curIncribeChance;

        [SerializeField] 
        private bool _isInscribed = false;
        [SerializeField]
        private bool _couldBeWritten = false;

        [SerializeField] private PhotoReward _photoReward;

        [SerializeField]
        private GameObject _bookMesh;
        private AudioSource _audioSource;

        private void Start()
        {
            _photoReward.enabled = false;
            _curIncribeChance = _defaultInscribeChance;

            _levelSetUp = AllServices.Container.Single<LevelSetUp>();
            _ghostRoom = _levelSetUp.CurrGhostRoom;

            if (_ghostRoom == LevelRooms.LevelRoomsEnum.NoRoom) _levelSetUp.OnLevelSetedUp += SetUpInfo;

            if (_couldBeWritten) StartCoroutine(nameof(WriteBook));

            _audioSource = GetComponent<AudioSource>();
        }
        IEnumerator WriteBook()
        {
            while (true)
            {
                yield return new WaitForSeconds(_tickTime);

                if (_currRoom.CurrRoom == _ghostRoom)
                {
                    if (RandomizeInscription(_curIncribeChance))
                    {
                        ChangeMaterial();
                        _photoReward.enabled = true;
                        _isInscribed = true;
                        StopCoroutine(nameof(WriteBook));
                    }
                    else
                    {
                        _curIncribeChance += _increaseChancePerTick;
                    }
                }
            }
        }
        private void ChangeMaterial()
        {
             _meshMaterials = _bookMeshRenderer.materials;
             _meshMaterials[1] = _inscribedMaterials[Random.Range(0, _inscribedMaterials.Length)];
             _bookMeshRenderer.materials = _meshMaterials;
            _audioSource.Play();
            _isInscribed = true;
        }
        
        private bool RandomizeInscription(float chance)
        {
            float number = Random.Range(0f, 100f);
            if (number <= chance) return true;
            else return false;
        }
        private void SetUpInfo()
        {
            _ghostInfo = _levelSetUp.GhostInfo;
            _ghostRoom = _levelSetUp.CurrGhostRoom;
            if (_ghostInfo.GhostData.GhostEvidences.Contains(GhostEvidence.GhostEvidencesTypes.GhostWriting)) _couldBeWritten = true;
        }

        public void EnableItem()
        {
            _bookMesh.SetActive(true);
            if (!_isInscribed && _couldBeWritten) StartCoroutine(nameof(WriteBook));
        }
        public void DisableItem()
        {
            _bookMesh.SetActive(false);
            StopAllCoroutines();
        }
    }
}