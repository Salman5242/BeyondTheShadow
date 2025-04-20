using Infrastructure;
using Infrastructure.States;
using Infrastructure.States.GameStates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
{
    public static GameBootstrapper Instance;
    public GameStateMachine StateMachine;

    private void Awake() //boot
    {
        Instance = this;
        StateMachine = new GameStateMachine(new AllServices(), this);
        StateMachine.Enter<BootState>();

        DontDestroyOnLoad(this);
    }
}
