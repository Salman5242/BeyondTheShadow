using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy = 15f;
    void Start()
    {
        Destroy(this.gameObject, _timeToDestroy);
    }

}
