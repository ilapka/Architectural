using CameraLogic;
using Enemy;
using Hero;
using Infrastructure.Factory;
using Infrastructure.Services;
using Infrastructure.Services.PersistentProgress;
using StaticData;
using UI.Elements;
using UI.Services.Factory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _curtain;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;
        private readonly IUIFactory _uiFactory;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain curtain,
            IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticData,
            IUIFactory uiFactory)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _curtain = curtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticData;
            _uiFactory = uiFactory;
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
            InitUiRoot();
            InitGameWorld();
            InformProgressReaders();
            _gameStateMachine.Enter<GameLoopState>();
        }

        private void InitUiRoot() =>
            _uiFactory.CreateUIRoot();

        private void InitGameWorld()
        {
            LevelStaticData levelData = LevelStaticData();

            InitSpawners(levelData);
            GameObject hero = InitHero(levelData);
            InitHud(hero);
            InitLoot();
            SetCameraFollow(hero);
        }

        private LevelStaticData LevelStaticData()
        {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _staticData.ForLevel(sceneKey);
            return levelData;
        }

        private void InitLoot()
        {
            foreach (var loot in _progressService.Progress.WorldData.LootData.Loots)
            {
                LootPiece piece = _gameFactory.CreateLoot();
                piece.Initialize(loot);
            }
        }

        private void InitSpawners(LevelStaticData levelData)
        {
            foreach (EnemySpawnerData spawnerData in levelData.EnemySpawners)
            {
                _gameFactory.CreateSpawner(spawnerData.Position, spawnerData.Id, spawnerData.MonsterTypeId);
            }
        }

        private GameObject InitHero(LevelStaticData levelData)
        {
            return _gameFactory.CreateHero(at: levelData.InitialHeroPosition);
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