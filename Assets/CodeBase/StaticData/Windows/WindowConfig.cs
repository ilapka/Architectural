using System;
using UI.Services.Windows;
using UI.Windows;
using UI.Windows.Shop;

namespace StaticData.Windows
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId WindowId;
        public WindowBase Prefab;
    }
}