using GameFeatures;
using UnityEngine;
using Utilities.Constants;

public class LevelInfo : MonoBehaviour
{
    public LevelSizeConst.LevelSize LevelSize;
    public Room[] AllLevelRooms;
    public Transform[] PatrolPoints;
    public DoorDraggable[] MainDoors;
    public LightButton[] LightButtons;
}
