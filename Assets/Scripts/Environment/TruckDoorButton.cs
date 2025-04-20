using UnityEngine;
using Items.Logic;
using Infrastructure.Services;
using Infrastructure;
using System.Collections;

public class TruckDoorButton : MonoBehaviour, IClickable
{
    [SerializeField] private Animator _anim;

    [SerializeField] private AudioSource _doorAudio;
    [SerializeField] private AudioSource _engineAudio;

    private GameFlowService _gameFlowService;
    private bool _isDoorsOpened = false;
    private bool _canBeOpened = true;
    private float _delayForEnd = 3f;
    private float _closingDoorsTime = 4f;


    private void Start()
    {
        _gameFlowService = AllServices.Container.Single<GameFlowService>();
    }
    public void OnClick()
    {
        if (_canBeOpened && !_isDoorsOpened && !_anim.GetCurrentAnimatorStateInfo(0).IsName("ClosingDoors"))
        {
            PlayDoorsSound();
            StopCoroutine(nameof(LevelEnd));
            _anim.SetTrigger("OpenDoors");
            _isDoorsOpened = true;
        }
        else if (_isDoorsOpened && !_anim.GetCurrentAnimatorStateInfo(0).IsName("OpeningDoors"))
        {
            PlayDoorsSound();
            StartCoroutine(nameof(LevelEnd));
            Invoke(nameof(PlayEngineSound), 2f);
            _anim.SetTrigger("CloseDoors");
            _isDoorsOpened = false;
        }      
    }

    IEnumerator LevelEnd()
    { 
        yield return new WaitForSeconds(_delayForEnd + _closingDoorsTime);
        _canBeOpened = false;
        LevelEndEvent();
    }
    public void LevelEndEvent()
    {
        _gameFlowService.WinAction?.Invoke();
    }
    private void PlayDoorsSound()
    {
        _doorAudio.Play();
    }
    private void PlayEngineSound()
    {
        _engineAudio.Play();
    }
}
