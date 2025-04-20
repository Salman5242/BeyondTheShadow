using Ghosts.EnvIneraction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniUnique : MonoBehaviour
{
    [SerializeField]
    private float _explodeLightChance = 12f;
    private GhostEnvInteraction _envInteraction;
    private void Start()
    {
        _envInteraction = GetComponentInParent<GhostEnvInteraction>();
        _envInteraction.ChangeLightExplodeChance(_explodeLightChance);
    }
}
