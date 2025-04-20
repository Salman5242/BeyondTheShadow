using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTriggerPoint : MonoBehaviour
{
    [SerializeField]
    private Transform _playerHead;

    private Vector3 _triggerPointTransform;
    private float _offsetX, _offsetY, _offsetZ;


    private void Start()
    {
        _offsetX = _playerHead.position.x - transform.position.x;
        _offsetY = _playerHead.position.y - transform.position.y;
        _offsetZ = _playerHead.position.z - transform.position.z;
    }


    private void Update()
    {
        _triggerPointTransform = _playerHead.transform.position;
        _triggerPointTransform.x -= _offsetX;
        _triggerPointTransform.y -= _offsetY;
        _triggerPointTransform.z -= _offsetZ;
        transform.position = _triggerPointTransform;
    }
}
