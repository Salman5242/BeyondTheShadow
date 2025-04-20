using Infrastructure;
using Infrastructure.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckInventory : MonoBehaviour
{
    [SerializeField] 
    private ItemsList _itemsList;

    [SerializeField] 
    private GameObject[] _items;

    [SerializeField]
    private int[] _addedItems;

    private LevelSetUp _levelSetUp;

    private void Start()
    {
        _levelSetUp = AllServices.Container.Single<LevelSetUp>();

        _addedItems = new int[_levelSetUp.AddedItems.Length];

        GetAddedItems();
        SetUpTruckInventory();
    }

    private void SetUpTruckInventory()
    {
        int curItem = 0;
        for (int i = 0; i < _addedItems.Length; i++)           
        {
            for (int j = 0; j < _itemsList.ItemsInfo[i].MaxAmount; j++)
            {
                if (j > _addedItems[i] - 1) Destroy(_items[curItem]);
                curItem++;
            }
        }
    }

    private void GetAddedItems()
    {
        for (int i = 0; i < _levelSetUp.AddedItems.Length; i++)
        {
            _addedItems[i] = _levelSetUp.AddedItems[i] + _itemsList.ItemsInfo[i].MinAmount;
        }
    }
}
