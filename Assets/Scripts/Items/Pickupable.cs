using Items.Logic;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Rigidbody),typeof(AudioSource))]

    public class Pickupable : MonoBehaviour, IPickupable
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
            _audioSource = GetComponent<AudioSource>();
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
}