using Infrastructure.Services.Ads;
using Infrastructure.Services.PersistentProgress;
using TMPro;

namespace UI.Windows.Shop
{
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI PlayerMoneyText;
        public RewardedAdItem AdItem;

        public void Construct(IAdsService adsService, IPersistentProgressService progressService)
        {
            base.Construct(progressService);
            AdItem.Construct(adsService, progressService);
        }
        
        protected override void Initialize()
        {
            AdItem.Initialize();
            RefreshPlayerMoneyText();
        }

        protected override void SubscribeUpdates()
        {
            AdItem.Subscribe();
            Progress.WorldData.LootData.Changed += RefreshPlayerMoneyText;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AdItem.Cleanup();
            Progress.WorldData.LootData.Changed -= RefreshPlayerMoneyText;
        }

        private void RefreshPlayerMoneyText() =>
            PlayerMoneyText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}