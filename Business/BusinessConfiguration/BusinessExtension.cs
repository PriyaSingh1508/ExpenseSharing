using Business.AppService;
using DataAccess.DataAccessConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessConfiguration
{
    public static class BusinessExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, string connectionString,IConfiguration  jwt)
        {
            services.RegisterDataContext(connectionString,jwt);
            services.AddScoped<IAccountService,AccountService>();
            services.AddScoped<IGroupService,GroupService>();
            services.AddScoped<IExpenseService,ExpenseService>();
            return services;
        }
    }
}
