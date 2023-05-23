using System.Collections.Generic;
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
        GameObject CreateHero(GameObject at);
        GameObject CreateHud();
        GameObject CreateMonster(MonsterTypeId typeId, Transform parent);
        void CreateSpawner(Vector3 at, string spawnerId, MonsterTypeId monsterTypeId);
        LootPiece CreateLoot();
        void Cleanup();
    }
}