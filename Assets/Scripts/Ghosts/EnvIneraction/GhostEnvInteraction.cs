using Items;
using Items.ItemsLogic;
using Items.Logic;
using System.Collections;
using UnityEngine;
using Utilities;
namespace Ghosts.EnvIneraction
{
    public class GhostEnvInteraction : MonoBehaviour
    {
        public LayerMask GhostInterectWithLayer;

        [SerializeField]
        private InteractionScript _interaction;

        [SerializeField]
        private GhostInfo _ghostInfo;

        //public LayerMask GhostInterectWithLayer;
        [SerializeField, Tooltip("How close player should be to interection, to lost sanity")]
        private float _interecMaxDistanceToPlayerSanity = 4f;

        [SerializeField]
        private float _ghostInteractionCDMultiplayer = 0.02f;

        [SerializeField]
        private AudioClip _doorInterectionSound;

        [SerializeField]
        private AudioClip _mainDoorClosedSound;

        private int _maxEMFLevel = 4;
        // private const float MaxInteractionRadius = 8f;
        private float _defaultInteractionChance;
        private float _explodeLightChance = 8f;
        private float _interactionCoef;

        private float _itemsThrowInteractionRadius;
        private float _doorsTouchInteractionRadius;
        private float _clickableInteractionRadius;

        private float _maxInteractionRadius = 0;

        private float _itemsThrowPower;

        private int _hitColliderSize = 0;
        private Vector3 _playerPos;
        private Vector3 _interectionPos;
        private bool _interected = false;

        private float _leaveHandprintChance = 40f;
        private bool _canLeaveHandprint = false;

        private float _ghostInterectCD = 0f;

        private WaitForSeconds WaitForCheckCD;

        private Collider[] hitColliders = new Collider[100];

        private bool _interectedWithPickupable = false;
        private bool _interectedWithClickable = false;
        private bool _interectedWithDoor = false;

        private void Start()
        {
            if (_ghostInfo.SetedUp) SetUpInfo();
            else  _ghostInfo.GhostSetedUp += SetUpInfo; 
            StartCoroutine(ItemInteraction());
        }

        public void CloseMainDoors()
        {
            foreach(DoorDraggable door in _ghostInfo.MainDoors)
            {
                AudioHelper.PlayClipAtPoint(_mainDoorClosedSound, door.transform.position, 0.8f);
                door.LockTheDoor();
            }
        }

        public void OpenMainDoors()
        {
            foreach (DoorDraggable door in _ghostInfo.MainDoors)
            {
                door.UnlockTheDoor();
            }
        }
        public void TurnOffLights()
        {
            foreach (LightButton button in _ghostInfo.LightButtons)
            {
                button.GhostAttackOffLight();
            }
        }
        public void TurnOnLight()
        {
            foreach (LightButton button in _ghostInfo.LightButtons)
            {
                button.EndGhostAttack();
            }
        }

        public void ChangeLightExplodeChance(float explodeChance)
        {
            _explodeLightChance = explodeChance;
        }
        private IEnumerator ItemInteraction()
        {
            while (true)
            {
                _interected = false;
                //Can be optimized
                InteractSphereCheck();
                if (!_interected) yield return new WaitForSeconds(CalculateInterectCD());
                else yield return WaitForCheckCD;
            }
        }
        private float CalculateInterectCD() => Mathf.Max(0f, _ghostInterectCD - (_ghostInfo.FinalGhostAnger * _ghostInteractionCDMultiplayer));


        public void InteractWithEverything()
        {
            _hitColliderSize = Physics.OverlapSphereNonAlloc(transform.position, _maxInteractionRadius, hitColliders, GhostInterectWithLayer);

            for (int i = 0; i < _hitColliderSize; i++)
            {
                CheckForPickupable(hitColliders[i], true);
                CheckForClickable(hitColliders[i], true);
                CheckForDoors(hitColliders[i], true);
            }
        }
        private void InteractSphereCheck()
        {
            _hitColliderSize = Physics.OverlapSphereNonAlloc(transform.position, _maxInteractionRadius, hitColliders, GhostInterectWithLayer);

            ResetInterectedWith();
            for (int i = 0; i < _hitColliderSize; i++)
            {
                if (!_interectedWithPickupable) if (CheckForPickupable(hitColliders[i])) return;
                if (!_interectedWithClickable) if (CheckForClickable(hitColliders[i])) return;
                if (!_interectedWithDoor) if (CheckForDoors(hitColliders[i])) return;
            }
        }

        private void ResetInterectedWith()
        {
            _interectedWithPickupable = false;
            _interectedWithClickable = false;
            _interectedWithDoor = false;
        }
        private bool CheckForPickupable(Collider hitCollider, bool ShouldInterect = false)
        {
            Pickupable pickupable = hitCollider.GetComponent<Pickupable>();
            if (pickupable != null && Vector3.Distance(transform.position, hitCollider.transform.position) <= _itemsThrowInteractionRadius)
            {
                _interectedWithPickupable = true;
                if (CalculateInteractionChance() == true || ShouldInterect)
                {
                    _interected = true;
                    InteractWithPickupable(pickupable);
                    return true;
                }
            }
            return false;
        }
        private void InteractWithPickupable(Pickupable item)
        {
            if (item != null)
            {
                InstantiateInteraction(item.transform.position);
                DropItemRb(item);
            }
        }

        private bool CheckForClickable(Collider hitCollider, bool ShouldInterect = false)
        {
            IClickable clickable = hitCollider.GetComponent<IClickable>();
            if (clickable != null && Vector3.Distance(transform.position, hitCollider.transform.position) <= _clickableInteractionRadius)
            {
                _interectedWithClickable = true;
                if (CalculateInteractionChance() == true || ShouldInterect)
                {
                    _interected = true;
                    InteractWithClickable(clickable);
                    return true;
                }
            }
            return false;
        }
        private void InteractWithClickable(IClickable clickable)
        {
            if (clickable != null)
            {
                InstantiateInteraction(clickable.gameObject.transform.position);
                if (clickable.gameObject.GetComponent<LightButton>())
                {
                    if (RandomGenerator.CalculateChance(_explodeLightChance))
                    {
                        clickable.gameObject.GetComponent<LightButton>().ExplodeLight();
                        return;
                    }
                }
                clickable.OnClick();
            }
        }

        private bool CheckForDoors(Collider hitCollider, bool ShouldInterect = false)
        {
            DoorDraggable door = hitCollider.GetComponent<DoorDraggable>();
            if (door != null && Vector3.Distance(transform.position, hitCollider.transform.position) <= _doorsTouchInteractionRadius)
            {
                _interectedWithDoor = true;
                if (CalculateInteractionChance() == true || ShouldInterect)
                {
                    _interected = true;
                    InteractWithDoors(door);

                    if (_canLeaveHandprint && door.InteractionTransform != null)
                    {
                        if (RandomGenerator.CalculateChance(_leaveHandprintChance))
                        {
                            door.GetComponent<IPrintsUV>().LeavePrintsUV();
                        }
                    }

                    return true;
                }
            }
            return false;
        }
        private void InteractWithDoors(DoorDraggable door)
        {
            if (door != null)
            {
                AudioHelper.PlayClipAtPoint(_doorInterectionSound, door.transform.position, 0.8f);

                InstantiateInteraction(door.InteractionTransform.position);
                door.GhostDrugDoor();
            }
        }


        private bool CalculateInteractionChance()
        { 
            float chance = _defaultInteractionChance + (_ghostInfo.FinalGhostAnger * _interactionCoef);
            bool procked = RandomGenerator.CalculateChance(chance);

            return procked;
        }


        private void InstantiateInteraction(Vector3 pos)
        {
            _interected = true;
            InteractionScript obj = Instantiate(_interaction, pos, Quaternion.identity);

            CheckForPlayerSanity(obj);
            SetObjectEmfLvl(ref obj);
        }

        private void CheckForPlayerSanity(InteractionScript obj)
        {
            if (_ghostInfo.PlayerPoint == null) return;
            _playerPos = _ghostInfo.PlayerPoint.position;
            _interectionPos = obj.transform.position;
            _playerPos.y = 0f; _interectionPos.y = 0f;
            if (Vector3.Distance(_playerPos, _interectionPos) <= _interecMaxDistanceToPlayerSanity)
            {
                _ghostInfo.PlayerSanity.TakeSanity(_ghostInfo.GhostData.PlayerSanityMinusPerGhostInterection);
            }
        }

        private void SetObjectEmfLvl(ref InteractionScript obj)
        {
            obj.EmfLvl = Random.Range(2, _maxEMFLevel);
        }
        private void SetUpInfo()
        {
            _maxInteractionRadius = 0f;
            _maxEMFLevel = 5;
            if (_ghostInfo.GhostData.GhostEvidences.Contains(GhostEvidence.GhostEvidencesTypes.EMF5)) _maxEMFLevel = 6;
            if (_ghostInfo.GhostData.GhostEvidences.Contains(GhostEvidence.GhostEvidencesTypes.Fingerprints)) _canLeaveHandprint = true;
            WaitForCheckCD = new WaitForSeconds(_ghostInfo.GhostData.GhostFindInerectionsCD);
            _ghostInterectCD = _ghostInfo.GhostData.GhostBetweenInterectionsCD;

            _itemsThrowPower = _ghostInfo.GhostData.ItemThrowPower;
            _itemsThrowInteractionRadius = _ghostInfo.GhostData.ItemThrowRadiusOfInterection;
            _maxInteractionRadius = Mathf.Max(_itemsThrowInteractionRadius, _maxInteractionRadius);

            _doorsTouchInteractionRadius = _ghostInfo.GhostData.DoorsTouchRadiusInterection;
            _maxInteractionRadius = Mathf.Max(_doorsTouchInteractionRadius, _maxInteractionRadius);

            _clickableInteractionRadius = _ghostInfo.GhostData.ClickableTouchRadiusInterection;
            _maxInteractionRadius = Mathf.Max(_clickableInteractionRadius, _maxInteractionRadius);

            _defaultInteractionChance = _ghostInfo.GhostData.DefaultInterectionChance;
            _interactionCoef = _ghostInfo.GhostData.InterectionsCoef;
        }
        private void DropItemRb(IPickupable item)
        {
            Rigidbody itemRB = item.gameObject.GetComponent<Rigidbody>();
            Vector3 randomItemDir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));

            itemRB.AddForce(randomItemDir * _itemsThrowPower, ForceMode.VelocityChange);
        }
    }
}