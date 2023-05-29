using Infrastructure.AssetManagement;
using Infrastructure.Services.Ads;
using Infrastructure.Services.IAP;
using Infrastructure.Services.PersistentProgress;
using TMPro;

namespace UI.Windows.Shop
{
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI PlayerMoneyText;
        public RewardedAdItem AdItem;
        public ShopItemsContainer ShopItemsContainer;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService,
            IIAPService iapService, IAssets assets)
        {
            base.Construct(progressService);
            AdItem.Construct(adsService, progressService);
            ShopItemsContainer.Construct(iapService, progressService, assets);
        }
        
        protected override void Initialize()
        {
            AdItem.Initialize();
            ShopItemsContainer.Initialize();
            RefreshPlayerMoneyText();
        }

        protected override void SubscribeUpdates()
        {
            AdItem.Subscribe();
            ShopItemsContainer.Subscribe();
            Progress.WorldData.LootData.Changed += RefreshPlayerMoneyText;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AdItem.Cleanup();
            ShopItemsContainer.CleanUp();
            Progress.WorldData.LootData.Changed -= RefreshPlayerMoneyText;
        }

        private void RefreshPlayerMoneyText() =>
            PlayerMoneyText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}