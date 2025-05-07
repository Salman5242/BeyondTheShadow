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

        public static readonly Dictionary<SceneNames.LevelNames, LevelSize> LevelSizes
            = new Dictionary<SceneNames.LevelNames, LevelSize>
        {
            {SceneNames.LevelNames.Turkwood, LevelSize.Tiny},
            {SceneNames.LevelNames.Warehouse, LevelSize.Medium}
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