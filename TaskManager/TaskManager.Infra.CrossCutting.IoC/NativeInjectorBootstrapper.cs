using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Infra.Data.Context;
using TaskManager.Infra.Data.Repository;

namespace TaskManager.Infra.CrossCutting.IoC
{
    //Registra as dependências entre as interfaces e as suas implementações
    public static class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Application

            services.AddScoped<IAssignmentAppService, AssignmentAppService>();

            //Infra-Data

            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<TaskManagerContext>();
        }
    }
}
