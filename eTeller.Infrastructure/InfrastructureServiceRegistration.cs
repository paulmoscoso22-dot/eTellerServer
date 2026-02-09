using eTeller.Application.Contracts;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccount;
using eTeller.Application.Mappings;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<eTellerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectionStringEteller")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IBaseSimpleRepository<>), typeof(BaseSimpleRepository<>));

            services.AddMediatR(cfg =>
             cfg.RegisterServicesFromAssembly(typeof(GetAccountQueryHandle).Assembly));

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(typeof(MappingProfile));
            },
            typeof(MappingProfile).Assembly);

            return services;
        }
    }
}
