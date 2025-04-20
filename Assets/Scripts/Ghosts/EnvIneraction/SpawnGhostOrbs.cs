using Infrastructure;
using Infrastructure.Services;
using System;
using UnityEngine;

public class SpawnGhostOrbs : MonoBehaviour
{
    [SerializeField] GameObject _ghostOrbs;
    [SerializeField] GhostInfo _ghostInfo;

    private LevelSetUp _levelSetUp;

    private void Start()
    {
        if (_ghostInfo.SetedUp) SpawnOrbs();
        else
        {
            _ghostInfo.GhostSetedUp += SpawnOrbs;
        }
    }

    private void OnDestroy()
    {
        _ghostInfo.GhostSetedUp -= SpawnOrbs;
    }

    private void SpawnOrbs()
    {
        if (_ghostInfo.GhostData.GhostEvidences.Contains(GhostEvidence.GhostEvidencesTypes.GhostOrbs))
        {
            _levelSetUp = AllServices.Container.Single<LevelSetUp>();
            Instantiate(_ghostOrbs, _levelSetUp.CurrGhostRoomTransform.position, Quaternion.identity);
        }
    }
}
