using System.Collections.Generic;
using System.Linq;
using Infrastructure.Services;
using UnityEngine;

namespace StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private const string StaticDataMonsters = "StaticData/Monsters";
        private const string StaticDataLevels = "StaticData/Levels";
        
        private Dictionary<MonsterTypeId, MonsterStaticData> _monsters;
        private Dictionary<string, LevelStaticData> _levels;

        public void LoadMonsters()
        {
            _monsters = Resources.LoadAll<MonsterStaticData>(StaticDataMonsters)
                .ToDictionary(x => x.MonsterTypeId, x => x);
            
            _levels = Resources.LoadAll<LevelStaticData>(StaticDataLevels)
                .ToDictionary(x => x.LevelKey, x => x);
        }

        public MonsterStaticData ForMonster(MonsterTypeId typeId) =>
            _monsters.TryGetValue(typeId, out MonsterStaticData staticData)
                ? staticData
                : null;

        public LevelStaticData ForLevel(string sceneKey) =>
            _levels.TryGetValue(sceneKey, out var sceneData)
                ? sceneData
                : null;
        
    }
}