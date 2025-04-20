using UnityEngine;

namespace Items.ItemsLogic
{
    public class InteractionScript : MonoBehaviour
    {
        public int EmfLvl = 1;
        public const float TimeBeforeDestroy = 15f;

        private void Start()
        {
            Destroy(this.gameObject, TimeBeforeDestroy);
        }
    }
}