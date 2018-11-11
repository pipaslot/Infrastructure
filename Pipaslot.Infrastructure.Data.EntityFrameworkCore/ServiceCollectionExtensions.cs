using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Pipaslot.Infrastructure.Data.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(optionsAction);
            services.AddSingleton<IEntityFrameworkDbContextFactory>(_ =>
            {
                var options = new DbContextOptionsBuilder<TDbContext>();
                optionsAction(options);
                return new EntityFrameworkDbContextFactory<TDbContext>(options.Options);
            });
            services.AddSingleton<IUnitOfWorkRegistry, UnitOfWorkRegistry>();
            services.AddSingleton<IUnitOfWorkFactory, EntityFrameworkUnitOfWorkFactory<TDbContext>>();
            return services;
        }
}
}
