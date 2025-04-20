using Items.Logic;
using System;
using System.Collections;
using UnityEngine;

namespace Items.ItemsLogic
{
    public class SmudgeStick : MonoBehaviour, IMainUsable
    {
        [SerializeField]
        private SmudgeObj _smudgeObj;
        [SerializeField]
        private ParticleSystem _smokeParts;
        [SerializeField]
        private GameObject _sticksParent;
        [SerializeField]
        private float _smudgeEffectTime = 4f;

        private const float TimeBetweenSmudgeObjects = 0.5f;
        private bool _isUsed = false;
        private bool _isSmudging = false;

        private WaitForSeconds _waitSpawnTime;

        private void Start()
        {
            _waitSpawnTime = new WaitForSeconds(TimeBetweenSmudgeObjects);   
        }

        public void OnMainUse()
        {
            if (!_isUsed)
            {
                ChangeSmudgeEffect(true);
                _isUsed = true;
            }
        }

        private void StopSmudging()
        {
            ChangeSmudgeEffect(false);
        }
        private IEnumerator Smudging()
        {
            while (true)
            {
                if (_isSmudging)
                {
                    Instantiate(_smudgeObj.gameObject, transform.position, Quaternion.identity);
                    yield return _waitSpawnTime;
                }
            }
           
        }

        private void ChangeSmudgeEffect(bool isSmudging)
        {
            _isSmudging = isSmudging;
            _smudgeObj.gameObject.SetActive(isSmudging);
            _sticksParent.SetActive(isSmudging);
            if (isSmudging)
            {
                _smokeParts.Play();
                StartCoroutine(nameof(Smudging));
                Invoke(nameof(StopSmudging), _smudgeEffectTime);
            }
            else
            {
                _smokeParts.Stop();
                StopCoroutine(nameof(Smudging));
            }
        }
    }
}