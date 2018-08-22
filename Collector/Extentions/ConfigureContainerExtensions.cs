using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Authorization;
using Collector.DAO.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Collector.Extentions
{
    public static class ConfigureContainerExtensions
    {
        public static void AddRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        public static void AddTransientServices(this IServiceCollection serviceCollection)
        {
            //serviceCollection.AddTransient<IPersonService, PersonService>();
            serviceCollection.AddTransient<ITokenService, TokenService>();
        }
    }
}
