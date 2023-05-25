using System.Threading.Tasks;
using CameraLogic;
using Enemy;
using Hero;
using Infrastructure.Factory;
using Infrastructure.Services;
using Infrastructure.Services.PersistentProgress;
using StaticData;
using UI.Elements;
using UI.Services.Factory;
using UI.Services.Windows;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.States
{
    public class LoadLevelState : IPayloadedState<string>
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;
        private readonly IPersistentProgressService _progressService;
        private readonly IStaticDataService _staticData;
        private readonly IWindowService _windowService;
        private readonly IUIFactory _uiFactory;

        public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader,
            IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticData, IWindowService windowService,
            IUIFactory uiFactory)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticData;
            _windowService = windowService;
            _uiFactory = uiFactory;
        }

        public void Enter(string sceneName)
        {
            _windowService.FadeIn();
            
            _gameFactory.Cleanup();
            _gameFactory.WarmUp();
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
            _windowService.FadeOut();
        }

        private async void OnLoaded()
        {
            await InitUiRoot();
            await InitGameWorld();
            InformProgressReaders();
            _gameStateMachine.Enter<GameLoopState>();
        }

        private async Task InitUiRoot() =>
            await _uiFactory.CreateUIRoot();

        private async Task InitGameWorld()
        {
            LevelStaticData levelData = LevelStaticData();

            await InitSpawners(levelData);
            await InitLoot();
            GameObject hero = await InitHero(levelData);
            await InitHud(hero);
            SetCameraFollow(hero);
        }

        private LevelStaticData LevelStaticData()
        {
            string sceneKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelData = _staticData.ForLevel(sceneKey);
            return levelData;
        }

        private async Task InitLoot()
        {
            foreach (var loot in _progressService.Progress.WorldData.LootData.Loots)
            {
                LootPiece piece = await _gameFactory.CreateLoot();
                piece.Initialize(loot);
            }
        }

        private async Task InitSpawners(LevelStaticData levelData)
        {
            foreach (EnemySpawnerData spawnerData in levelData.EnemySpawners)
            {
                await _gameFactory.CreateSpawner(spawnerData.Position, spawnerData.Id, spawnerData.MonsterTypeId);
            }
        }

        private async Task<GameObject> InitHero(LevelStaticData levelData)
        {
            return await _gameFactory.CreateHero(at: levelData.InitialHeroPosition);
        }

        private async Task InitHud(GameObject hero)
        {
            GameObject hud = await _gameFactory.CreateHud();

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