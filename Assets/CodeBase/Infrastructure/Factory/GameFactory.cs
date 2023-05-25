using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Enemy;
using Infrastructure.AssetManagement;
using Infrastructure.Services;
using Infrastructure.Services.PersistentProgress;
using Logic;
using Logic.EnemySpawners;
using StaticData;
using UI.Elements;
using UI.Services.Windows;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssets _assets;
        private readonly IStaticDataService _staticData;
        private readonly IRandomService _randomService;
        private readonly IPersistentProgressService _progressService;
        private readonly IWindowService _windowService;
        public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
        public List<ISavedProgress> ProgressWriters { get; } = new List<ISavedProgress>();
        private GameObject HeroGameObject { get; set; }

        public GameFactory(IAssets assets, IStaticDataService staticData, IRandomService randomService,
            IPersistentProgressService progressService, IWindowService windowService)
        {
            _assets = assets;
            _staticData = staticData;
            _randomService = randomService;
            _progressService = progressService;
            _windowService = windowService;
        }

        public async Task WarmUp()
        {
            await _assets.Load<GameObject>(AssetsAddress.Loot);
            await _assets.Load<GameObject>(AssetsAddress.Spawner);
        }

        public async Task<GameObject> CreateHero(Vector3 at)
        {
            HeroGameObject = await InstantiateRegisteredAsync(AssetsAddress.HeroPath, at);
            return HeroGameObject;
        }

        public async Task<GameObject> CreateHud()
        {
            GameObject hud = await InstantiateRegisteredAsync(AssetsAddress.HudPath);
            hud.GetComponentInChildren<LootCounter>().Construct(_progressService.Progress.WorldData);

            foreach (OpenWindowButton opnOpenWindowButton in hud.GetComponentsInChildren<OpenWindowButton>())
            {
                opnOpenWindowButton.Construct(_windowService);
            }

            return hud;
        }

        public async Task<GameObject> CreateMonster(MonsterTypeId typeId, Transform parent)
        {
            MonsterStaticData monsterData = _staticData.ForMonster(typeId);
            
            GameObject monsterPrefab = await _assets.Load<GameObject>(monsterData.PrefabReference);
            GameObject monster = Object.Instantiate(monsterPrefab, parent.position, Quaternion.identity, parent);

            IHealth health = monster.GetComponent<IHealth>();
            health.Current = monsterData.Hp;
            health.Max = monsterData.Hp;

            monster.GetComponent<ActorUI>().Construct(health);
            monster.GetComponent<AgentMoveToHero>().Construct(HeroGameObject.transform);
            monster.GetComponent<NavMeshAgent>().speed = monsterData.MoveSpeed;

            Attack attack = monster.GetComponent<Attack>();
            attack.Construct(HeroGameObject.transform);
            attack.Damage = monsterData.Damage;
            attack.Cleavage = monsterData.AttackCleavage;
            attack.EffectiveDistance = monsterData.EffectiveDistance;

            monster.GetComponent<AgentRotateToHero>()?.Construct(HeroGameObject.transform);

            WorldData worldData = _progressService.Progress.WorldData;

            LootSpawner lootSpawner = monster.GetComponentInChildren<LootSpawner>();
            lootSpawner.Construct(this, _randomService, worldData);
            lootSpawner.SetLoot(monsterData.MinLoot, monsterData.MaxLoot);

            return monster;
        }

        public async Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId)
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetsAddress.Spawner);
            SpawnPoint spawner = InstantiateRegistered(prefab, at)
                .GetComponent<SpawnPoint>();

            spawner.Construct(this);
            spawner.Id = spawnerId;
            spawner.MonsterTypeId = monsterTypeId;
        }

        public async Task<LootPiece> CreateLoot()
        {
            GameObject prefab = await _assets.Load<GameObject>(AssetsAddress.Loot);
            LootPiece lootPiece = InstantiateRegistered(prefab)
                .GetComponent<LootPiece>();

            lootPiece.Construct(_progressService.Progress.WorldData);

            return lootPiece;
        }

        public void Cleanup()
        {
            ProgressReaders.Clear();
            ProgressWriters.Clear();
            
            _assets.CleanUp();
        }

        private GameObject InstantiateRegistered(GameObject prefab, Vector3 at)
        {
            GameObject gameObject = Object.Instantiate(prefab, at, Quaternion.identity);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }
        
        private GameObject InstantiateRegistered(GameObject prefab)
        {
            GameObject gameObject = Object.Instantiate(prefab);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateRegisteredAsync(string prefabPath, Vector3 at)
        {
            GameObject gameObject = await _assets.Instantiate(prefabPath, at);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private async Task<GameObject> InstantiateRegisteredAsync(string prefabPath)
        {
            GameObject gameObject = await _assets.Instantiate(prefabPath);
            RegisterProgressWatchers(gameObject);
            return gameObject;
        }

        private void RegisterProgressWatchers(GameObject gameObject)
        {
            foreach (ISavedProgressReader progressReader in gameObject.GetComponents<ISavedProgressReader>())
                Register(progressReader);
        }

        private void Register(ISavedProgressReader progressReader)
        {
            if (progressReader is ISavedProgress progressWriter)
            {
                ProgressWriters.Add(progressWriter);
            }

            ProgressReaders.Add(progressReader);
        }
    }
}