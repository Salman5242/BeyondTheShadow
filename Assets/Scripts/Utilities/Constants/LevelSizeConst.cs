using System;
using System.Collections.Generic;

namespace Utilities.Constants
{
    public static class LevelSizeConst
    {
        public enum LevelSize { Tiny, Small, Medium, Big };
        public enum LevelNames { Turkwood, Warehouse };
        public enum MapMinHuntDurationEnum
        {
            Tiny = 8,
            Small = 14,
            Medium = 22,
            Big = 30,
        }

        public enum MapMaxHuntDurationEnum
        {
            Tiny = 13,
            Small = 25,
            Medium = 35,
            Big = 50,
        }

        public static readonly Dictionary<SceneNames.LevelNames, int> LevelSizes = new()
    {
        { SceneNames.LevelNames.Turkwood, 2 },
        { SceneNames.LevelNames.Warehouse, 3 },
        { SceneNames.LevelNames.Abandoned, 4 }
    };

        public static int MapMinHuntDuration(string levelSize)
        {
            return (int)Enum.Parse(typeof(MapMinHuntDurationEnum), levelSize);
        }

        public static int MapMaxHuntDuration(string levelSize)
        {
            return (int)Enum.Parse(typeof(MapMaxHuntDurationEnum), levelSize);
        }
    }
}