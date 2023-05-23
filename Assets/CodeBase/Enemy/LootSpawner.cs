using Data;
using Infrastructure.Factory;
using Infrastructure.Services;
using UnityEngine;

namespace Enemy
{
    public class LootSpawner : MonoBehaviour
    {
        public EnemyDeath EnemyDeath;
        
        private IGameFactory _factory;
        private IRandomService _random;
        
        private int _lootMin;
        private int _lootMax;

        public void Construct(IGameFactory factory, IRandomService randomService)
        {
            _factory = factory;
            _random = randomService;
        }
        
        private void Start()
        {
            EnemyDeath.Happened += SpawnLoot;
        }

        private void SpawnLoot()
        {
            LootPiece loot = _factory.CreateLoot();
            loot.transform.position = transform.position;

            var lootItem = GenerateLoot();
            
            loot.Initialize(lootItem);
        }

        private Loot GenerateLoot()
        {
            var lootItem = new Loot()
            {
                Value = _random.Next(_lootMin, _lootMax),
            };
            
            return lootItem;
        }

        public void SetLoot(int min, int max)
        {
            _lootMin = min;
            _lootMax = max;
        }
        
        private void OnDestroy()
        {
            if (EnemyDeath != null)
                EnemyDeath.Happened -= SpawnLoot;
        }
    }
}   