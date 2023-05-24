using TMPro;

namespace UI.Windows
{
    public class ShopWindow : WindowBase
    {
        public TextMeshProUGUI PlayerMoneyText;

        protected override void Initialize() =>
            RefreshPlayerMoneyText();

        protected override void SubscribeUpdates() =>
            Progress.WorldData.LootData.Changed += RefreshPlayerMoneyText;

        protected override void Cleanup()
        {
            base.Cleanup();
            Progress.WorldData.LootData.Changed -= RefreshPlayerMoneyText;
        }

        private void RefreshPlayerMoneyText() =>
            PlayerMoneyText.text = Progress.WorldData.LootData.Collected.ToString();
    }
}