using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty", menuName = "ScriptableObjects/Difficulty", order = 1)]
public class DifficultySO : ScriptableObject
{
    [Tooltip("Time in seconds at the beginning of the game, when ghost can't hunt")]
    public float GameGracePeriod;
    [Tooltip("Time in seconds at the beginning of attack, when ghost can't kill player")]
    public float HuntSafeTime;
    [Tooltip("(Higher the number - Faster player will lost his sanity")]
    public float SanityWasteModifier;
    public float HuntDurationModifier;

    public float RewardCoef;

    public float TimeToWarmHouse;
    [Tooltip("How much money does player return after death")]
    public float InsurancePercent;
    [TextArea(5, 10)]
    public string Description;

}