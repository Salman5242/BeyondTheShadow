using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _ghostMesh;

    [SerializeField]
    private GhostInfo _ghostInfo;

    [SerializeField]
    private PhotoReward _photoReward;

    private float _minMeshActiveTime;
    private float _maxMeshActiveTime;

    private float _minMeshDisabledTime;  
    private float _maxMeshDisabledTime;


    private bool _isMeshActive = false;
    private bool _isAttacking = false;

    private void Start()
    {
        _minMeshActiveTime = _ghostInfo.GhostData.MinMeshActiveTime;
        _maxMeshActiveTime = _ghostInfo.GhostData.MaxMeshActiveTime;
        _minMeshDisabledTime = _ghostInfo.GhostData.MinMeshDisabledTime;
        _maxMeshDisabledTime = _ghostInfo.GhostData.MaxMeshDisabledTime;
    }
    private void Update()
    {
        if (_isMeshActive && !_photoReward.enabled) _photoReward.enabled = true;
        if (!_isMeshActive && _photoReward.enabled) _photoReward.enabled = false;
    }
    private void OnEnable()
    {
        _isMeshActive = false;
        _ghostMesh.SetActive(false);
    }

    private void OnDisable()
    {
        _ghostMesh.SetActive(true);
        _isMeshActive = true;
    }

    public void MeshDisappear()
    {
        _ghostMesh.SetActive(false);
        _isMeshActive = false;
    }
    public void DisableMesh()
    {
        if (!enabled) return;

        _isAttacking = false;
        _ghostMesh.SetActive(false);
        _isMeshActive = false;
    }

    public void GhostEventMesh()
    {
        if (!enabled) return;

        _isAttacking = false;
        _isMeshActive = true;
        _ghostMesh.SetActive(true);
    }

    public void AttackStateMesh()
    {
        if (!enabled) return;

        _isAttacking = true;
        StartCoroutine(AttackStateMeshCoroutine());
    }


    private IEnumerator AttackStateMeshCoroutine()
    {
        while (true)
        {
            if (!_isAttacking) break;

            SwitchMeshState();

            if (_isMeshActive) yield return new WaitForSeconds(Random.Range(_minMeshActiveTime, _maxMeshActiveTime));
            else yield return new WaitForSeconds(Random.Range(_minMeshDisabledTime, _maxMeshDisabledTime));
        }

        yield return null;
    }

    private void SwitchMeshState()
    {
        if (_isMeshActive)
        {
            _ghostMesh.SetActive(false);
            _isMeshActive = false;
        }
        else
        {
            _ghostMesh.SetActive(true);
            _isMeshActive = true;
        }
    }
}
