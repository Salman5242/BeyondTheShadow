using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Infrastructure.Services
{
    public class DataSaveLoader : IService
    {

        public StoredInfo _storedInfo = new StoredInfo();

        [ContextMenu("Load")]
        public void LoadInfo()
        {
            if (!File.Exists(Application.streamingAssetsPath + "/data.json"))
            {
                CreateNewFile();
            }
            else _storedInfo = JsonUtility.FromJson<StoredInfo>(File.ReadAllText(Application.streamingAssetsPath + "/data.json"));
        }
        [ContextMenu("Save")]
        public void SaveInfo()
        {
            File.WriteAllText(Application.streamingAssetsPath + "/data.json", JsonUtility.ToJson(_storedInfo));
        }

        public void SaveItemsAmount(int[] items)
        {
            _storedInfo.ItemsAmount = items;
            SaveInfo();
        }
        public void SaveMoney(float money)
        {
            _storedInfo.Money = money;
            SaveInfo();
        }
        public void AddMoney(float money)
        {
            _storedInfo.Money += money;
            SaveInfo();
        }
        public void RemoveItems(int[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                _storedInfo.ItemsAmount[i] -= items[i];
            }
            SaveInfo();
        }
        private void CreateNewFile()
        {
            var file = File.Create(Application.streamingAssetsPath + "/data.json");
            file.Close();

            _storedInfo.Money = 100f;
            for (int i = 0; i < _storedInfo.ItemsAmount.Length; i++)
            {
                _storedInfo.ItemsAmount[i] = 0;
            }

            File.WriteAllText(Application.streamingAssetsPath + "/data.json", JsonUtility.ToJson(_storedInfo));
        }


        [System.Serializable]
        public class StoredInfo
        {
            public float Money = 100f;
            public int[] ItemsAmount = new int[20];
        }
    }
}
