using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemsCollisionSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip _hitSound;
    [SerializeField]
    private float _hitVolume = 0.1f;

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 1f;
        _audioSource.maxDistance = 8f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float speed = _rigidbody.velocity.magnitude;
        if (speed > 1f) _audioSource.PlayOneShot(_hitSound, _hitVolume * speed);
    }
}
