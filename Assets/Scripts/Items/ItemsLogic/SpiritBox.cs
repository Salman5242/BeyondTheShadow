using GameFeatures;
using Infrastructure;
using Infrastructure.Services;
using Items.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Constants;

public class SpiritBox : MonoBehaviour, IMainUsable, IDisababled, IDroppable
{
    [SerializeField]
    private GameObject _mesh;
    [SerializeField]
    private AudioSource _radioFrequency;
    [SerializeField]
    private AudioClip[] _ghostVoiceClips;
    [SerializeField]
    private float _ghostVoiceVolume = 0.3f;

    [SerializeField]
    private RoomIdentifire _currRoom;
    [SerializeField]
    private float _defaultChanceToSay = 1f;
    [SerializeField]
    private float _increaseChancePerTick = 0.1f;
    [SerializeField]
    private float _tickTime = 3f;
    [SerializeField]
    private float _inHandModifier = 15f;

    private LevelSetUp _levelSetUp;
    private GhostInfo _ghostInfo;
    private LevelRooms.LevelRoomsEnum _ghostRoom;

    private float _curChanceToSay = 0f;

    private bool _isEnabled = false;
    private bool _isInHands = false;
    private bool _hasSpiritBoxEvidence = false;

    private void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();
        _ghostRoom = _levelSetUp.CurrGhostRoom;

        if (_ghostRoom == LevelRooms.LevelRoomsEnum.NoRoom) _levelSetUp.OnLevelSetedUp += SetUpInfo;

        _curChanceToSay = _defaultChanceToSay;
    }
    public void OnMainUse()
    {
        if (_isEnabled == false) EnableSpiritBox();
        else DisableSpiritBox();
    }

    IEnumerator DetectGhostVoice()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tickTime);
            if (_currRoom.CurrRoom == _ghostRoom)
            {
                if (CalculateChanceToSay(_curChanceToSay, _isInHands, _inHandModifier))
                {
                    _radioFrequency.PlayOneShot(_ghostVoiceClips[Random.Range(0, _ghostVoiceClips.Length)], _ghostVoiceVolume);
                    _curChanceToSay = _defaultChanceToSay;
                }
                else _curChanceToSay += _increaseChancePerTick;
            }
        }
    }

    private bool CalculateChanceToSay(float chance, bool inHand, float modifier)
    {
        float number = Random.Range(0f, 100f);
        if (inHand) chance *= modifier;
        if (number <= chance) return true;
        else return false;
    }
    private void SetUpInfo()
    {
        _ghostInfo = _levelSetUp.GhostInfo;
        _ghostRoom = _levelSetUp.CurrGhostRoom;
        if (_ghostInfo.GhostData.GhostEvidences.Contains(GhostEvidence.GhostEvidencesTypes.SpiritBox)) _hasSpiritBoxEvidence = true;
    }
    private void EnableSpiritBox()
    {
        _isEnabled = true;
        _radioFrequency.Play();
        if (_hasSpiritBoxEvidence) StartCoroutine(nameof(DetectGhostVoice));
    }
    private void DisableSpiritBox()
    {
        _isEnabled = false;
        _radioFrequency.Pause();
        StopCoroutine(nameof(DetectGhostVoice));
    }
    public void DisableItem()
    {
        _mesh.SetActive(false);
        _radioFrequency.Pause();
        StopCoroutine(nameof(DetectGhostVoice));
    }
    public void EnableItem()
    {
        _isInHands = true;
        _mesh.SetActive(true);
        if (_isEnabled) EnableSpiritBox();
    }

    public void DropItem()
    {
        _isInHands = false;
    }
}
