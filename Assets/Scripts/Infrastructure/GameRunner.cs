using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRunner : MonoBehaviour
{
    public GameBootstrapper BootstrapperPrefab;
    private void Awake()
    {
        var bootstrapper = FindObjectOfType<GameBootstrapper>();

        if (bootstrapper != null) return;

        bootstrapper = Instantiate(BootstrapperPrefab);
    }
}
