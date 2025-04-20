using Infrastructure;
using Infrastructure.Services;
using UnityEngine;

public class GhostJumpscare : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;

    private GameFlowService _gameFlow;
    private const string Jumpscare = "Jumpscare";
    void Start()
    {
        _gameFlow = AllServices.Container.Single<GameFlowService>();
        _gameFlow.GameOverAction += CallJumpScare;
    }

    private void OnDestroy()
    {
        _gameFlow.GameOverAction -= CallJumpScare;
    }

    private void CallJumpScare()
    {
        _anim.SetTrigger(Jumpscare);
    }
}
