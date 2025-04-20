using UnityEngine;

namespace Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("Instance of " + typeof(T)).AddComponent<T>();
                    }
                }
                return _instance;
            }
        }


        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
        }
    }
}