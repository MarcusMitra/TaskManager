using Microsoft.EntityFrameworkCore;
using TaskManager.Infra.Data.Context;

namespace TaskManagerAPI.Configurations
{
    //Diz que usarei o SQLite e registra o TaskManagerContext(DBcontext)
    public static class DatabaseConfig
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<TaskManagerContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
