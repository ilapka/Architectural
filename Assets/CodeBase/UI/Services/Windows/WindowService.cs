using System;
using UI.Elements;
using UI.Services.Factory;

namespace UI.Services.Windows
{ 
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;
        private LoadingCurtain _loadingCurtain;

        public WindowService(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            
            InitCurtain();
        }

        public void Open(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.Unknown:
                    break;
                case WindowId.Shop:
                    _uiFactory.CreateShop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(windowId), windowId, null);
            }    
        }

        public void FadeIn()
        {
            _loadingCurtain.Show();
        }

        public void FadeOut()
        {
            _loadingCurtain.Hide();
        }

        private void InitCurtain()
        {
            if(_loadingCurtain != null)
                return;
            
            _loadingCurtain = _uiFactory.CreateCurtainNonAsync();
        }
    }
}