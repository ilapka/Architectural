using Data;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LootCounter : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _counter;
        
        private WorldData _worldData;

        public void Construct(WorldData worldData)
        {
            _worldData = worldData;
            _worldData.LootData.Changed += UpdateCounter;
        }

        private void Start()
        {
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            _counter.text = $"{_worldData.LootData.Collected}";
        }

        private void OnDestroy()
        {
            if (_worldData != null)
                _worldData.LootData.Changed -= UpdateCounter;
        }
    }
}