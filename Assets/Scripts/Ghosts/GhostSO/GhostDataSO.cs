using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "ScriptableObjects/GhostData", order = 1)]
public class GhostDataSO : ScriptableObject
{
    public float GhostNormalSpeed = 2;
    public float GhostAttackSpeed = 2;
    public List<GhostEvidence.GhostEvidencesTypes> GhostEvidences;

    [Tooltip("The lower this number,the more time ghost will spend in his room")]
    public int PatrolRandomMultiplier = 5;

    public float StartGhostAnger = 0;
    public float GhostAngerPlusPerSec = 0.070f;
    public float GhostAngerInGhostRoomPlusPerSec = 0.150f;
    public float MaxGhostAnger = 110f;
    public float StartLateGhostAnger = 70f;

    public float PlayerSanityMinusPerSecond = 0.055f;
    public float PlayerSanityMinusInGhostRoomPerSecond = 0.12f;

    public float PlayerSanityMinusPerGhostInterection = 1f;
    public float PlayerSanityMinusPerGhostGhostEvent = 5f;
    public float PlayerSanityMinusPerHunt = 10f;

    public float MinSanityToAttack = 60f;
    public float MaxDistanceToPlayerAggr = 4f;
    [Tooltip("Higher the number, higher the chance for a ghost to attack")]
    public float AttackChanceCoef = 0.2f;
    public float MinAttackCD = 40f;
    [Tooltip("Higher the number, higher the chance for a ghost to attack")]
    public float  GhostEventChanceCoef = 0.2f;

    public float DefaultInterectionChance = 5f;
    public float InterectionsCoef = 0.2f;

    public float GhostFindInerectionsCD = 2f;
    public float GhostBetweenInterectionsCD = 4f;

    public float ItemThrowRadiusOfInterection = 1.5f;
    public float DoorsTouchRadiusInterection = 2.5f;
    public float ClickableTouchRadiusInterection = 2.5f;

    public float ItemThrowPower = 5f;

    public float MinMeshActiveTime = 0.12f;
    public float MaxMeshActiveTime = 0.18f;

    public float MinMeshDisabledTime = 0.55f;
    public float MaxMeshDisabledTime = 0.9f;

    public GameObject[] UniqueAbilities;

    [TextArea(5, 10)]
    public string GhostDescription;

}