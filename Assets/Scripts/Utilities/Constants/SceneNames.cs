using System.Collections.Generic;

namespace Utilities.Constants
{
    public static class SceneNames
    {
        public const string BootScene = "Boot";
        public const string LobbyScene = "Lobby";

        public enum LevelNames { Turkwood, Warehouse, Abandoned }

        public static readonly Dictionary<LevelNames, string> LevelNameToString = new()
        {
            { LevelNames.Turkwood, "Turkwood" },
            { LevelNames.Warehouse, "Warehouse" },
            { LevelNames.Abandoned, "Abandoned Factory" } 
        };
    }
}
