using System;
using System.Collections.Generic;

namespace Infrastructure.Services.IAP
{
    public interface IIAPService : IService
    {
        event Action Initialized;
        bool IsInitialized { get; }
        void Initialize();
        List<ProductDescription> Products();
        void StartPurchase(string productId);
    }
}