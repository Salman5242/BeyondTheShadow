using Ghosts.Mood;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmudgeObj : MonoBehaviour
{
    [SerializeField]
    private LayerMask _enemyLayer;
    [SerializeField]
    private float TriggerDistance = 1.7f;
    [SerializeField]
    private float _timeToDissapear = 3f;
    [SerializeField, Tooltip("Should this object dissapear in given time?")]
    private bool _shouldDissapear = true;

    private void OnEnable()
    {
        if (_shouldDissapear) Destroy(this.gameObject, _timeToDissapear);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, TriggerDistance, _enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            GhostMood ghostMood = hitCollider.GetComponent<GhostMood>();
            if (ghostMood) ghostMood.SmudgeEffect();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GhostMood ghostMood = other.GetComponent<GhostMood>();

        if (ghostMood) {ghostMood.SmudgeEffect(); }
    }
}
