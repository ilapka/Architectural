using Data;
using Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace Logic
{
    public class EnemySpawner : MonoBehaviour, ISavedProgress
    {
        public MonsterTypeId MonsterTypeId;
        private string _id;

        public bool IsSlain;

        private void Awake()
        {
            _id = GetComponent<UniqueId>().Id;
        }

        public void LoadProgress(PlayerProgress progress)
        {
            if (progress.KillData.ClearedSpawners.Contains(_id))
            {
                IsSlain = true;
            }
            else
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            if(IsSlain)
            {
                progress.KillData.ClearedSpawners.Add(_id);
            }
        }
    }
}