using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ghosts
{
    public class LineOfSight : MonoBehaviour
    {
        private const float MaxDistance = 100f;
        [SerializeField]
        private Transform _rayStartTransform;
        [SerializeField]
        private LayerMask _layerMask;

        private RaycastHit _ray;
        private Vector3 _rayDirection;
        private PlayerCheckResult _checkResult = new PlayerCheckResult();

        public PlayerCheckResult CheckForPlayer(Transform playerPoint , Transform heroTransform)
        {
            _rayDirection = (playerPoint.position - _rayStartTransform.position);
            
            if(Physics.Raycast(_rayStartTransform.position, _rayDirection, out _ray, MaxDistance, _layerMask))
            {
                Debug.DrawRay(_rayStartTransform.position, _rayDirection, Color.red, 1f);
                if ( _ray.transform == heroTransform)
                {
                    _checkResult.IsPlayerVisible = true;
                    _checkResult.DistanceToPlayer = Vector3.Distance(playerPoint.position, transform.position);
                }
                else
                {
                    _checkResult.IsPlayerVisible = false;
                }
            }
            return _checkResult;
        }
    }

    public struct PlayerCheckResult
    {
        public bool IsPlayerVisible;
        public float DistanceToPlayer;
    }
}