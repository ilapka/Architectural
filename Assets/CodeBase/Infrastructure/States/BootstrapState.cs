using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Infrastructure.Services;
using Infrastructure.Services.Ads;
using Infrastructure.Services.Input;
using Infrastructure.Services.PersistentProgress;
using Infrastructure.Services.SaveLoad;
using StaticData;
using UI.Services.Factory;
using UI.Services.Windows;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = services;
            
            RegisterServices();
        }

        public void Enter()
        {
            _sceneLoader.Load(Initial, onLoaded: EnterLoadProgress);
        }

        private void EnterLoadProgress()
        {
            _stateMachine.Enter<LoadProgressState>();
        }

        private void RegisterServices()
        {
            RegisterStaticData();
            RegisterAdsService();

            _services.RegisterSingle<IAssets>(new AssetProvider());
            _services.RegisterSingle<IGameStateMachine>(_stateMachine);
            _services.RegisterSingle(InputService());
            _services.RegisterSingle<IRandomService>(new UnityRandomService());
            _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
    
            _services.RegisterSingle<IUIFactory>(new UIFactory(
                _services.Single<IAssets>(),
                _services.Single<IStaticDataService>(), 
                _services.Single<IPersistentProgressService>(), 
                _services.Single<IAdsService>()
                ));
            
            _services.RegisterSingle<IWindowService>(new WindowService(_services.Single<IUIFactory>()));

            _services.RegisterSingle<IGameFactory>(new GameFactory(
                _services.Single<IAssets>(),
                _services.Single<IStaticDataService>(),
                _services.Single<IRandomService>(),
                _services.Single<IPersistentProgressService>(),
                _services.Single<IWindowService>()
                ));

            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(_services.Single<IPersistentProgressService>(), _services.Single<IGameFactory>()));
        }

        private void RegisterAdsService()
        {
            IAdsService adsService = new AdsService();
            adsService.Initialize();
            _services.RegisterSingle(adsService);
        }

        private void RegisterStaticData()
        {
            IStaticDataService staticData = new StaticDataService();
            staticData.LoadMonsters();
            _services.RegisterSingle(staticData);
        }

        public void Exit()
        {
            
        }
        
        private static IInputService InputService()
        {
            if (Application.isEditor)
            {
                return new StandaloneInputService();
            }

            return new MobileInputService();
        }
    }
}