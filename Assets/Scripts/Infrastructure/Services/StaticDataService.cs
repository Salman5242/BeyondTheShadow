using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infrastructure.Services
{
    public class StaticDataService : IService
    {
        private const string GhostDataPath = "Ghost/TypesSO";
        private const string GameObjectivesPath = "Gameplay/ObjectivesSO";

        private Dictionary<string, GhostDataSO> _ghosts;

        private Dictionary<string, GameObjectiveSO> _gameObjectives;

        public void Load()
        {
            _ghosts = Resources
            .LoadAll<GhostDataSO>(GhostDataPath)
            .ToDictionary(x => x.name, x => x);

            _gameObjectives = Resources
            .LoadAll<GameObjectiveSO>(GameObjectivesPath)
            .ToDictionary(x => x.name, x => x);
        }

        public Dictionary<string, GameObjectiveSO> GetAllObjectives()
        {
            return _gameObjectives;
        }

        public GhostDataSO GetRandomGhost()
        {
           return ForMonster(_ghosts.ElementAt(Random.Range(0, _ghosts.Count)).Key);
            //return ForMonster(_ghosts.ElementAt(1).Key);
        }

        public GhostDataSO ForMonster(string name) =>
         _ghosts.TryGetValue(name, out GhostDataSO staticData)
          ? staticData
          : null;

    }
}