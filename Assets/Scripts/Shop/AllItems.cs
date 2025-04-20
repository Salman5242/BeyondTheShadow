using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllItems", menuName = "ScriptableObjects/AllItems", order = 2)]
public class AllItems : ScriptableObject
{
    public ItemDataSO[] ItemsDataSO;
}
