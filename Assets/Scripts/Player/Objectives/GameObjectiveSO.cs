using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objectives", menuName = "ScriptableObjects/Objectives", order = 1)]
public class GameObjectiveSO : ScriptableObject
{
    public float ObjectiveReward;
    public bool IsDone = false;

    [TextArea(3, 8)]
    public string ObjectiveDescription;
}
