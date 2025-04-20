using GameFeatures;
using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using UnityEngine;
using Utilities.Constants;

public class SanityHandler : MonoBehaviour
{
    [SerializeField]
    private RoomIdentifire _currPlayerRoom;
    [SerializeField]
    private float _maxSanityValue;
    [SerializeField,Tooltip("How much sanity is taken from the player when he is in the house")]
    private float _houseSecondSanityMinus;  
    [SerializeField,Tooltip("How much sanity is taken from the player when he is in the ghost room")]
    private float _ghostRoomSecondSanityMinus;

    private WaitForSeconds WaitOneSecond = new WaitForSeconds(1f);
    private GhostInfo _ghostInfo;
    private LevelSetUp _levelSetUp;
    private GameObjectivesService _gameObjectives;

    private LevelRooms.LevelRoomsEnum _currGhostRoom = LevelRooms.LevelRoomsEnum.NoRoom;
    private bool _isSubscribedToOnLevelSetedUp = false;
    private float _sanityModifier = 1f;

    public float Sanity;


    private void Start()
    {
        if(_levelSetUp != null) _levelSetUp.OnLevelSetedUp -= SetUp;

        _gameObjectives = AllServices.Container.Single<GameObjectivesService>();
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();

        Sanity = _maxSanityValue;
        if (_levelSetUp.IsInitialized) { SetUp(); _isSubscribedToOnLevelSetedUp = false; }
        else {_levelSetUp.OnLevelSetedUp += SetUp; _isSubscribedToOnLevelSetedUp = true; }

    }

    private void OnDestroy()
    {
        if(_isSubscribedToOnLevelSetedUp)
        _levelSetUp.OnLevelSetedUp -= SetUp;
    }

    public void TakeSanity(float ammountToTake)
    {
        Sanity -= (ammountToTake * _sanityModifier);
        Sanity = Mathf.Clamp(Sanity, 0, _maxSanityValue);

    }

    public void AddSanity(float ammountToAdd)
    {
        Sanity += (ammountToAdd / _sanityModifier);
        Sanity = Mathf.Clamp(Sanity, 0, _maxSanityValue);
    }

    private IEnumerator DropSanityWithTime()
    {
        while (true)
        {
            DropPlayerSanity();
            yield return WaitOneSecond;
        }
    }

    private void DropPlayerSanity()
    {
        if (_currPlayerRoom.CurrRoom == LevelRooms.LevelRoomsEnum.NoRoom) return;
        else if (_currPlayerRoom.CurrRoom == _currGhostRoom) TakeSanity(_ghostRoomSecondSanityMinus);
        else TakeSanity(_houseSecondSanityMinus);

        if(Sanity < 20) _gameObjectives.AvgSanityMinus20();

    }

    private void SetUp()
    {
        _currGhostRoom = _levelSetUp.CurrGhostRoom;
        _ghostInfo = _levelSetUp.GhostInfo;

        _sanityModifier = _levelSetUp.SelectedDifficulty.SanityWasteModifier;

        _houseSecondSanityMinus = _ghostInfo.GhostData.PlayerSanityMinusPerSecond;
        _ghostRoomSecondSanityMinus = _ghostInfo.GhostData.PlayerSanityMinusInGhostRoomPerSecond;
        StartCoroutine(DropSanityWithTime());
    }

}
