using Data;
using Enemy;
using Infrastructure.Factory;
using Infrastructure.Services.PersistentProgress;
using StaticData;
using UnityEngine;

namespace Logic.EnemySpawners
{
    public class SpawnPoint : MonoBehaviour, ISavedProgress
    {
        public MonsterTypeId MonsterTypeId;
        
        private IGameFactory _factory;
        private EnemyDeath _enemyDeath;
        public string Id { get; set; }
        
        private bool _isSlain;

        public void Construct(IGameFactory factory)
        {
            _factory = factory;
        }
        
        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.ClearedSpawners.Contains(Id))
            {
                _isSlain = true;
            }
            else
            {
                Spawn();
            }
        }

        private async void Spawn()
        {
            GameObject monster = await _factory.CreateMonster(MonsterTypeId, transform);
            _enemyDeath = monster.GetComponent<EnemyDeath>();
            _enemyDeath.Died += Slay;
        }

        private void Slay()
        {
            if (_enemyDeath != null)
            {
                _enemyDeath.Died -= Slay;
            }
            
            _isSlain = true;
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if(_isSlain)
            {
                progress.KillData.ClearedSpawners.Add(Id);
            }
        }
    }
}