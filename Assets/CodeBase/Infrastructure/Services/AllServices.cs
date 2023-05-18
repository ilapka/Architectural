namespace Infrastructure.Services
{
    public class AllServices
    {
        private static AllServices _instance;
        public static AllServices Container => _instance ??= new AllServices();

        public void RegisterSingle<TService>(TService implementation) where TService : IService
        {
            Implementation<TService>.ServiceInstance = implementation;
            
            //Note: 2 variant - dictionary by type,
            //Note: the disadvantages of this implementation is that the static class will exist for all application life
            //Note: so we can't dispose our services
        }

        public TService Single<TService>() where TService : IService
        {
            return Implementation<TService>.ServiceInstance;
        }
        
        private static class Implementation<TService> where TService : IService
        {
            public static TService ServiceInstance;
        }
    }
}