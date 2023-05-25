using System.Threading.Tasks;
using Infrastructure.Services;
using UI.Elements;

namespace UI.Services.Factory
{
    public interface IUIFactory : IService
    {
        void CreateShop();
        Task CreateUIRoot();
        LoadingCurtain CreateCurtainNonAsync();
    }
}