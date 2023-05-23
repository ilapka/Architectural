using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(menuName = "StaticData/Monster", fileName = "MonsterData", order = 0)]
    public class MonsterStaticData : ScriptableObject
    {
        public MonsterTypeId MonsterTypeId;
        [Range(1, 100)]
        public int Hp;
        [Range(1f, 30f)]
        public float Damage;
        [Range(0.5f, 1f)] 
        public float EffectiveDistance = 0.5f;
        [Range(0.5f, 1f)]
        public float AttackCleavage = 0.5f;
        public float MoveSpeed;

        public int MinLoot;
        public int MaxLoot;
        
        public GameObject Prefab;
    }
}