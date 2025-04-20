using System;
using System.Threading.Tasks;
using UI.Journal;
using UnityEngine;
using Utilities.Constants;

namespace Infrastructure.Services
{
    public class GameFactory : IService
    {
        private GameObject _mainHero = null;
        private Journal _journal = null;
        private GameObject _jumpscare = null;
        private GameObject _ghost = null;
        private readonly AssetProvider _assets;

        public GameFactory(AssetProvider assets)
        {
            _assets = assets;
        }

        public async Task <GameObject> CreateHero(GameObject at)
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.HeroPath, at.transform.position);
            _mainHero = gameObject;
            return gameObject;
        }

        public async Task<GameObject> CreateGhost(GameObject at)
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.GhostPath, at.transform.position);
            return gameObject;
        }
        public async Task<GameObject> CreateInputSystem()
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.InputSystemPath);
            return gameObject;
        }

        public async Task<GameObject> CreateAudioManager()
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.AudioManagerPath);
            return gameObject;
        }

        public async Task<GameObject> CreateTargetUI()
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.TargetUIPath);
           
            return gameObject;
        }

        public async Task<GameObject> CreateJournal()
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.JournalPath);
            _journal = gameObject.GetComponent<Journal>();
            return gameObject;
        }

        public async Task<GameObject> CreateJumpscare()
        {
            GameObject gameObject = await _assets.Instantiate(AssetPath.JumpscarePath);
            _jumpscare = gameObject;
            return gameObject;
        }


        public GameObject GetMainHero()
        {
            return _mainHero;
        }
        public Journal GetJournal()
        {
            return _journal;
        }
    }
}