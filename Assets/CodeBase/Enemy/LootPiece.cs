﻿using System.Collections;
using Data;
using Infrastructure.Services.PersistentProgress;
using TMPro;
using UnityEngine;

namespace Enemy
{
    public class LootPiece : MonoBehaviour, ISavedProgress
    {
        [SerializeField]
        private GameObject _skull;
        [SerializeField]
        private GameObject _pickupFxPrefab;
        [SerializeField]
        private TextMeshPro _lootText;
        [SerializeField]
        private GameObject _pickupPopup;
        
        private WorldData _worldData;
        private Loot _loot;
        
        private bool _isPicked;

        public void Construct(WorldData worldData)
        {
            _worldData = worldData;
        }

        public void Initialize(Loot loot)
        {
            _loot = loot;
            transform.position = loot.Position.AsUnityVector();
        }

        private void OnTriggerEnter(Collider other) =>
            PickUp();

        private void PickUp()
        {
            if(_isPicked)
                return;
            
            _isPicked = true;
            
            UpdateWorldData();
            HideSkull();
            PlayPickupFx();
            ShowText();
            StartCoroutine(StartDestroyTimer());
        }

        private void UpdateWorldData()
        {
            _worldData.LootData.Collect(_loot);
        }

        private void HideSkull()
        {
            _skull.SetActive(false);
        }

        private void PlayPickupFx()
        {
            Instantiate(_pickupFxPrefab, transform.position, Quaternion.identity);
        }

        private void ShowText()
        {
            _lootText.text = $"{_loot.Value}";
            _pickupPopup.SetActive(true);
        }

        private IEnumerator StartDestroyTimer()
        {               
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }

        public void LoadProgress(PlayerProgress progress)
        {
            Loot loot = progress.WorldData.LootData.GetLoot(id: _loot.Id);

            if(loot != null)
                Initialize(loot);
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            Loot loot = progress.WorldData.LootData.GetLoot(id: _loot.Id);
            
            if(loot != null)
                loot.Position = transform.position.AsVectorData();
        }
    }
}