using TaskManager.Infra.CrossCutting.IoC;

namespace TaskManagerAPI.Configurations
{
    //Registra os serviços da aplicação, chamando o NativeInjectorBootstrapper
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            NativeInjectorBootstrapper.RegisterServices(services);
        }
    }
}
