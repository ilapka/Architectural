using System.Threading.Tasks;
using Data;
using Infrastructure.Factory;
using Infrastructure.Services;
using Logic;
using UnityEngine;

namespace Enemy
{
    public class LootSpawner : MonoBehaviour
    {
        [SerializeField]
        private UniqueId _uniqueId;
        
        public EnemyDeath EnemyDeath;
        
        private IGameFactory _factory;
        private IRandomService _random;
        private WorldData _worldData;
        
        private int _lootMin;
        private int _lootMax;

        public void Construct(IGameFactory factory, IRandomService randomService, WorldData worldData)
        {
            _factory = factory;
            _random = randomService;
            _worldData = worldData;
        }
        
        private void Start()
        {
            EnemyDeath.Died += SpawnLoot;
        }

        private async void SpawnLoot()
        {
            LootPiece loot = await _factory.CreateLoot();

            var lootItem = GenerateLoot();
            
            loot.Initialize(lootItem);
        }

        private Loot GenerateLoot()
        {
            var lootItem = new Loot()
            {
                Id = _uniqueId.Id,
                Value = _random.Next(_lootMin, _lootMax),
                Position = transform.position.AsVectorData()
            };
            
            _worldData.LootData.AddLoot(lootItem);
            
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
                EnemyDeath.Died -= SpawnLoot;
        }
    }
}   