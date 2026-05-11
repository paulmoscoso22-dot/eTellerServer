using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace eTeller.Auth;

public static class AuthServiceRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthServiceRegistration).Assembly));
        services.AddValidatorsFromAssembly(typeof(AuthServiceRegistration).Assembly);
        // TODO (Issue #30): aggiungere AutoMapper con profili quando saranno definiti
        // services.AddAutoMapper(cfg => { cfg.AddProfile(typeof(AuthMappingProfile)); }, typeof(AuthServiceRegistration).Assembly);

        return services;
    }
}
