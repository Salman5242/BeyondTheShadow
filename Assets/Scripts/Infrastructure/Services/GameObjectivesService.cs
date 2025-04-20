using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Infrastructure.Services
{
    public class GameObjectivesService :  IService
    {
        public Action OnObjectiveCompleted;
        public List<GameObjectiveSO> CurrObjectives = new List<GameObjectiveSO>(3);

        private const int RandomObjectivesCount = 3;
        private const string EMFLevelSTR = "EMFLevel";
        private const string GhostEventSTR = "GhostEvent";
        private const string GhostPhotoSTR = "GhostPhoto";
        private const string SmudgeStickSTR = "SmudgeStick";
        private const string SanityLvlSTR = "SanityLvl";
        private const string EscapeGhostHuntSTR = "EscapeGhostHunt";
        private const string CrucifixPreventedHuntSTR = "CrucifixPreventedHunt";


        private Dictionary<string, GameObjectiveSO> _allObjectives;
        public GameObjectivesService(StaticDataService _staticData)
        {
            _allObjectives = _staticData.GetAllObjectives();
            for(int i = 0; i< RandomObjectivesCount; i++)
                CurrObjectives.Add(null);
        }

        public void ClearAllData()
        {
            foreach (KeyValuePair<string, GameObjectiveSO> objective in _allObjectives)
               objective.Value.IsDone = false;
        }

        public void SetUpObjectives()
        {
            List<int>RandomNums = RandomGenerator.GenerateRandom(RandomObjectivesCount, 0, _allObjectives.Count);
            for(int i = 0; i< RandomObjectivesCount; i++)
            {
                CurrObjectives[i] = _allObjectives.ElementAt(RandomNums[i]).Value;
            }
        }
        public void EMFLevelFound(int emfLvl)
        {
            if (emfLvl > 2)
            {
                CompleteObjective(EMFLevelSTR);
            }
        }

        public void PhotoTaken(string photoName)
        {
            if(photoName == "Ghost")
                CompleteObjective(GhostPhotoSTR);
        }

        public void GhostEventWitnessed()
        {
            CompleteObjective(GhostEventSTR);
        }

        public void GhostSmudged()
        {
            CompleteObjective(SmudgeStickSTR);
        }

        public void EscapeGhostDuringHunt()
        {
            CompleteObjective(EscapeGhostHuntSTR);
        }

        public void AvgSanityMinus20()
        {
            CompleteObjective(SanityLvlSTR);
        }

        public void CrucifixPreventedHunt()
        {
            CompleteObjective(CrucifixPreventedHuntSTR);
        }

        private bool CompleteObjective(string ObjectiveName)
        {
            if (!_allObjectives[ObjectiveName].IsDone)
            {
                _allObjectives[ObjectiveName].IsDone = true;
                ObjectivesRefreshed();
                return true;
            }
            else return false;
        }
        private void ObjectivesRefreshed()
        {
            OnObjectiveCompleted?.Invoke();
        }
    }
}