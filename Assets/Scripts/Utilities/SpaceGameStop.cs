using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SpaceGameStop : MonoBehaviour
{
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            EditorApplication.isPaused = true;
        }
    }
#endif
}
