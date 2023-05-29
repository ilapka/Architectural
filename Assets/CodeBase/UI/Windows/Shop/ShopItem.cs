﻿using Infrastructure.AssetManagement;
using Infrastructure.Services.IAP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Shop
{
    public class ShopItem : MonoBehaviour
    {
        public Button BuyItemButton;
        public TextMeshProUGUI PriceText;
        public TextMeshProUGUI QuantityText;
        public TextMeshProUGUI AvailableitemsLeftText;
        public Image Icon;
        
        private IIAPService _iapService;
        private IAssets _assets;
        private ProductDescription _productDescription;

        public void Construct(IIAPService iapService, IAssets assets, ProductDescription productDescription)
        {
            _iapService = iapService;
            _assets = assets;
            _productDescription = productDescription;
        }

        public async void Initialize()
        {
            BuyItemButton.onClick.AddListener(OnBuyItemClick);
            
            PriceText.text = _productDescription.Config.Price;
            QuantityText.text = _productDescription.Config.Quantity.ToString();
            AvailableitemsLeftText.text = _productDescription.AvailablePurchasesLeft.ToString();
            Icon.sprite = await _assets.Load<Sprite>(_productDescription.Config.Icon);
        }

        private void OnBuyItemClick()
        {
            _iapService.StartPurchase(_productDescription.Id);
        }
    }
}