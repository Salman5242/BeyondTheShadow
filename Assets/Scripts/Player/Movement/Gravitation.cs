using UnityEngine;

namespace Player.Movement
{
    public class Gravitation : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;

        [SerializeField] private float fallSpeed = -9.81f;

        private Vector3 velocity;

        void FixedUpdate()
        {
            velocity.y = fallSpeed;
            controller.Move(velocity * Time.fixedDeltaTime);
        }
    }
}