using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class AnimationControl : MonoBehaviour
    {
        private Animator _anim;

        private const string IsCrouching = "CrouchTrigger";
        private const string Crouch = "IsCrouch";
        private string _standingTrans ="StandingTransition";
        private string _sittingTrans = "SittingTransition";
        private void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public bool SitDown()
        {
            if (!_anim.GetAnimatorTransitionInfo(0).IsUserName(_standingTrans))
            {
                _anim.SetTrigger(IsCrouching);
                _anim.SetBool(Crouch, true);
                return true;
            }
            else return false;
        }

        public bool StandUp()
        {
            if (!_anim.GetAnimatorTransitionInfo(0).IsUserName(_sittingTrans))
            {
                _anim.SetBool(Crouch, false);
                return true;
            }
            else
            {
                Debug.Log("Fuck");
                return false;
            }
        }
    }
}