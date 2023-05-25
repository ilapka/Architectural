using System.Collections.Generic;
using System.Threading.Tasks;
using Enemy;
using Infrastructure.Services;
using Infrastructure.Services.PersistentProgress;
using StaticData;
using UnityEngine;

namespace Infrastructure.Factory
{
    public interface IGameFactory : IService
    {
        List<ISavedProgressReader> ProgressReaders { get; }
        List<ISavedProgress> ProgressWriters { get; }
        Task<GameObject> CreateHero(Vector3 at);
        Task<GameObject> CreateHud();
        Task<GameObject> CreateMonster(MonsterTypeId typeId, Transform parent);
        Task CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId);
        Task<LootPiece> CreateLoot();
        void Cleanup();
        Task WarmUp();
    }
}