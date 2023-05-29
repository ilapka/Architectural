using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.AssetManagement;
using Infrastructure.Services.IAP;
using Infrastructure.Services.PersistentProgress;
using UnityEngine;

namespace UI.Windows.Shop
{
    public class ShopItemsContainer : MonoBehaviour
    {
        private const string ShopItemAddress = "ShopItem";
        
        public GameObject[] ShopUnavailableObjects;
        public Transform Root;
        
        private IIAPService _iapService;
        private IPersistentProgressService _progressService;
        private IAssets _assets;
        
        private readonly List<GameObject> _shopItems = new();

        public void Construct(IIAPService iapService, IPersistentProgressService progressService, IAssets assets)
        {
            _iapService = iapService;
            _progressService = progressService;
            _assets = assets;
        }

        public void Initialize()
        {
            RefreshAvailableItems();
        }

        public void Subscribe()
        {
            _iapService.Initialized += RefreshAvailableItems;
            _progressService.Progress.PurchaseData.Changed += RefreshAvailableItems;
        }

        public void CleanUp()
        {
            _iapService.Initialized -= RefreshAvailableItems;
            _progressService.Progress.PurchaseData.Changed -= RefreshAvailableItems;
        }

        private async void RefreshAvailableItems()
        {
            UpdateShopUnavailableObjects();

            if(!_iapService.IsInitialized)
                return;

            ClearShopItems();

            await FillShopItems();
        }

        private void UpdateShopUnavailableObjects()
        {
            foreach (GameObject shopUnavailableObject in ShopUnavailableObjects)
                shopUnavailableObject.SetActive(!_iapService.IsInitialized);
        }

        private void ClearShopItems()
        {
            foreach (GameObject shopItemObject in _shopItems)
            {
                Destroy(shopItemObject.gameObject);
            }

            _shopItems.Clear();
        }

        private async Task FillShopItems()
        {
            foreach (ProductDescription productDescription in _iapService.Products())
            {
                GameObject shopItemObject = await _assets.Instantiate(ShopItemAddress, Root);
                ShopItem shopItem = shopItemObject.GetComponent<ShopItem>();

                shopItem.Construct(_iapService, _assets, productDescription);
                shopItem.Initialize();

                _shopItems.Add(shopItemObject);
            }
        }
    }
}