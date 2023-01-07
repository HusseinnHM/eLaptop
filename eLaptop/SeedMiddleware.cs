using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


public static class SeedMiddleware
{
    public static IApplicationBuilder Migration<TContext>(this IApplicationBuilder builder,
        bool deleteDbIfFailure = false) where TContext : DbContext
    {
        var serviceProvider = builder.ServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var context = serviceProvider.GetRequiredService<TContext>();
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Migrate is done");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed while applies migrations");
            if (deleteDbIfFailure)
            {
                logger.LogInformation("Start delete db and re-applies migrations");
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }

        return builder;
    }

    public static IApplicationBuilder Migration<TContext>(this IApplicationBuilder builder,
        Func<TContext, IServiceProvider, Task> seed, bool deleteDbIfFailure = false) where TContext : DbContext
    {
        var serviceProvider = builder.ServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var context = serviceProvider.GetRequiredService<TContext>();
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Migrate is done");

            seed(context, serviceProvider);
            logger.LogInformation("Seed is done");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed while applies migrations/seed data");

            if (deleteDbIfFailure)
            {
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                seed(context, serviceProvider);
            }
        }

        return builder;
    }

    public static IApplicationBuilder Migration<TContext>(this IApplicationBuilder builder,
        Action<TContext, IServiceProvider> seed, bool deleteDbIfFailure = false) where TContext : DbContext
    {
        var serviceProvider = builder.ServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var context = serviceProvider.GetRequiredService<TContext>();
        using var db = context.Database.BeginTransaction();
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Migrate is done");

            seed(context, serviceProvider);
            logger.LogInformation("Seed is done");
            db.Commit();
        }
        catch (Exception e)
        {
            db.Rollback();
            logger.LogError(e, "Failed while applies migrations/seed data");
            if (deleteDbIfFailure)
            {
                context.Database.EnsureCreated();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                seed(context, serviceProvider);
            }
        }

        return builder;
    }

    public static IApplicationBuilder Seed<TContext>(this IApplicationBuilder builder,
        Func<TContext, IServiceProvider, Task> seed) where TContext : DbContext
    {
        var serviceProvider = builder.ServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var context = serviceProvider.GetRequiredService<TContext>();
        try
        {
            seed(context, serviceProvider);
            logger.LogInformation("Seed is done");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed while seed data");
        }

        return builder;
    }

    public static IApplicationBuilder Seed<TContext>(this IApplicationBuilder builder,
        Action<TContext, IServiceProvider> seed) where TContext : DbContext
    {
        var serviceProvider = builder.ServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
        var context = serviceProvider.GetRequiredService<TContext>();
        try
        {
            seed(context, serviceProvider);
            logger.LogInformation("Seed is done");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed while seed data");
        }

        return builder;
    }

    private static IServiceProvider ServiceProvider(this IApplicationBuilder builder) => builder
        .ApplicationServices
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope()
        .ServiceProvider;
}