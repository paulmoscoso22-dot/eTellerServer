using eTeller.Application.Behaviours;
using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Auth;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Contracts.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Application.Contracts.Vigilanza;
using eTeller.Application.Features.ContiCorrenti.Commands.Carica;
using eTeller.Application.Features.Manager.Commands.Users.InsertUser;
using eTeller.Application.Features.User.Services;
using eTeller.Application.Mappings;
using eTeller.Application.Mappings.Prelievo;
using eTeller.Application.Validators;
using eTeller.Domain.Services;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories;
using eTeller.Infrastructure.Repositories.StoreProcedures;
using eTeller.Infrastructure.Repositories.StoreProcedures.Currency;
using eTeller.Infrastructure.Repositories.StoreProcedures.Manager;
using eTeller.Infrastructure.Repositories.StoreProcedures.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Infrastructure.Repositories.StoreProcedures.Vigilanza;
using eTeller.Infrastructure.Services;
using eTeller.Infrastructure.Services.Auth;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<eTellerDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectionStringEteller")));
            services.AddHostedService<DatabaseInitializer>();
            services.AddMemoryCache();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IBaseSimpleRepository<>), typeof(BaseSimpleRepository<>));
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            // Register Authentication Service
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Register JWT Token Service
            services.AddScoped<ITokenService, JwtTokenService>();

            // Register Password History Repository
            services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

            // Register validators
            services.AddScoped<IValidator<CaricaContiCorrentiCommand>, CaricaContiCorrentiValidator>();
            services.AddScoped<IValidator<PrelievoViewVm>, CaricaRequestValidator>();
            services.AddScoped<IValidator<InsertUserCommand>, InsertUserCommandValidator>();

            // Register repositories
            services.AddScoped<Application.Contracts.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository, ErrorCodeRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IVigilanzaRepository>(provider =>
                new VigilanzaSpRepository(
                    provider.GetRequiredService<eTellerDbContext>(),
                    provider.GetRequiredService<ILoggerFactory>().CreateLogger<VigilanzaSpRepository>()));

            // Register Domain Services
            services.AddScoped<IForexDomainService, ForexDomainService>();
            services.AddScoped<IIS107DomainService, IS107DomainService>();

            services.AddMediatR(cfg =>
             cfg.RegisterServicesFromAssembly(typeof(Application.Features.StoreProcedures.Account.Queries.GetAccount.GetAccountQueryHandler).Assembly));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddValidatorsFromAssembly(typeof(ValidationBehaviour<,>).Assembly);


            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(typeof(MappingProfile));
            },
            typeof(MappingProfile).Assembly);

            return services;
        }
    }
}
