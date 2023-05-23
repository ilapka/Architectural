﻿using CameraLogic;
using Enemy;
using Hero;
using Infrastructure.Factory;
using Infrastructure.Services.PersistentProgress;
using Logic;
using UI;
using UnityEngine;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private const string EnemySpawnerTag = "EnemySpawner";
        
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain curtain, IGameFactory gameFactory, IPersistentProgressService progressService)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
        }

        public void Enter(string sceneName)
        {
            _curtain.Show();
            _gameFactory.Cleanup();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            _curtain.Hide();
        }
        
        private void OnLoaded()
        {
            InitGameWorld();
            InformProgressReaders();
            _gameStateMachine.Enter<GameLoopState>();
        }

        private void InitGameWorld()
        {
            InitSpawners();
            GameObject hero = _gameFactory.CreateHero(at: GameObject.FindWithTag(InitialPointTag));
            InitHud(hero);
            InitLoot();
            SetCameraFollow(hero);
        }

        private void InitLoot()
        {
            foreach (var loot in _progressService.Progress.WorldData.LootData.Loots)
            {
                LootPiece piece = _gameFactory.CreateLoot();
                piece.Initialize(loot);
            }
        }

        private void InitSpawners()
        {
            GameObject[] spawners= GameObject.FindGameObjectsWithTag(EnemySpawnerTag);

            foreach (GameObject spawnerObject in spawners)
            {
                EnemySpawner spawner = spawnerObject.GetComponent<EnemySpawner>();
                _gameFactory.Register(spawner);
            }
        }

        private void InitHud(GameObject hero)
        {
            GameObject hud = _gameFactory.CreateHud();
            
            hud.GetComponentInChildren<ActorUI>()
                .Construct(hero.GetComponent<HeroHealth>());
        }

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
                progressReader.LoadProgress(_progressService.Progress);
        }

        private void SetCameraFollow(GameObject gameObject)
        {
            Camera.main
                .GetComponent<CameraFollow>()
                .Follow(gameObject);
        }
    }
}