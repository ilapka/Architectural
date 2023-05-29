using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Infrastructure.Services.PersistentProgress;
using UnityEngine.Purchasing;

namespace Infrastructure.Services.IAP
{
    public class IAPService : IIAPService
    {
        private readonly IAPProvider _iapProvider;
        private readonly IPersistentProgressService _progressService;
        
        public event Action Initialized;

        public IAPService(IAPProvider iapProvider, IPersistentProgressService progressService)
        {
            _iapProvider = iapProvider;
            _progressService = progressService;
        }

        public void Initialize()
        {
            _iapProvider.Initialized += () => Initialized?.Invoke();
            _iapProvider.Initialize(this);
        }

        public List<ProductDescription> Products()
        {
            return ProductDescriptions().ToList();
        }

        public void StartPurchase(string productId)
        {
            _iapProvider.StartPurchase(productId);
        }

        public PurchaseProcessingResult ProcessPurchase(Product purchasedProduct)
        {
            ProductConfig productConfig = _iapProvider.Configs[purchasedProduct.definition.id];

            switch (productConfig.ItemType)
            {
                case ItemType.Currency:
                    _progressService.Progress.WorldData.LootData.Add(productConfig.Quantity);
                    _progressService.Progress.PurchaseData.AddPurchase(purchasedProduct.definition.id);
                    break;
            }

            return PurchaseProcessingResult.Complete;
        }

        private IEnumerable<ProductDescription> ProductDescriptions()
        {
            PurchaseData purchaseData = _progressService.Progress.PurchaseData;

            foreach (string productId in _iapProvider.Products.Keys)
            {
                ProductConfig config = _iapProvider.Configs[productId];
                Product product = _iapProvider.Products[productId];

                BoughtIAP boughtIAP = purchaseData.BoughtIAPs.Find(x => x.IAPid == productId);

                if(IsProductBoughtOut(boughtIAP, config))
                {
                    continue;
                }

                yield return new ProductDescription()
                {
                    Id = productId,
                    Config = config,
                    Product = product,
                    AvailablePurchasesLeft = boughtIAP != null
                        ? config.MaxPurchaseCount - boughtIAP.Count
                        : config.MaxPurchaseCount,
                };
            }
        }

        private static bool IsProductBoughtOut(BoughtIAP boughtIAP, ProductConfig config)
        {
            return boughtIAP != null && boughtIAP.Count >= config.MaxPurchaseCount;
        }

        public bool IsInitialized => _iapProvider.IsInitialized;
    }
}