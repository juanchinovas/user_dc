using Application.Users.Dtos;
using Application.Users.Managers;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Reflection;
using FluentValidation;
using Api.Services;
using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ISaveable<User>, AddUserManager>();
        services.AddScoped<IQuerableFilterable<User, UserFilter>, QueryUserManager>();
        services.AddTransient<IFileProcessor, UserInfoBatchCsvFileProcessor>();
        services.AddScoped<IAuthService, AuthService>(); 
        services.AddSingleton<ICacheService>((_) =>
            new MemoryCacheService(
                new MemoryCache(
                    new MemoryCacheOptions
                    {
                        SizeLimit = 1024
                    }
                )
            )
        );


        return services;
    }

    public static void AddApplicationMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomRequestMiddleware>();
    }

    public static void ConfigureApplicationCustomApiBehaviorOptions(this IMvcBuilder builder)
    {
        builder.ConfigureApiBehaviorOptions(options =>
        {
            var builtInFactory = options.InvalidModelStateResponseFactory;
            options.InvalidModelStateResponseFactory = context =>
            {
                if (CustomBadRequestHandler.IsRequestStateInvalid(context) && builtInFactory is not null)
                {
                    return builtInFactory(context);
                }

                return new BadRequestObjectResult(CustomBadRequestHandler.ConstructAppResponse(context));
            };
        });
        
    }
}