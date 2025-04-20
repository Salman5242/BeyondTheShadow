using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsInventory : MonoBehaviour
{
    private DataSaveLoader _dataSaveLoader;

    public int[] _purchasedItemsAmount;

    private void Start()
    {
        _dataSaveLoader = AllServices.Container.Single<DataSaveLoader>();
        _purchasedItemsAmount = _dataSaveLoader._storedInfo.ItemsAmount;
    }
    public void AddItem(int itemId)
    {
        _purchasedItemsAmount[itemId]++;

        _dataSaveLoader.SaveItemsAmount(_purchasedItemsAmount);
    }
}
