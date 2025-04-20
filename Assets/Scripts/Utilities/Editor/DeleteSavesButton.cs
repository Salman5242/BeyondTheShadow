using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class DeleteSavesButton
    {
        [MenuItem("Tools/Clear Saves")]
        public static void ClearSaves()
        {
            File.Delete(Application.streamingAssetsPath + "/data.json");
        }
    }
}