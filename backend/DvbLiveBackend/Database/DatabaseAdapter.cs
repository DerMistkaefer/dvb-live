using DerMistkaefer.DvbLive.Backend.Database.Api;
using DerMistkaefer.DvbLive.Backend.Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.Backend.Database
{
    internal class DatabaseAdapter : IDatabaseAdapter
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseAdapter(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            DbOperation(context => context.Database.MigrateAsync()).Wait();
        }

        public Task ClearDatabase() => DbOperation(async context =>
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE StopPoints").ConfigureAwait(false);
        });

        public Task InsertStopPoint(StopPoints entity) => DbOperation(async context =>
        {
            await context.StopPoints.AddAsync(entity).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        });

        public Task<List<StopPoints>> GetAllStopPoints() => DbOperation(async context =>
        {
            return await context.StopPoints.ToListAsync().ConfigureAwait(false);
        });

        private async Task DbOperation(Func<DvbDbContext, Task> databaseOperation)
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<DvbDbContext>();
            await databaseOperation(dbContext).ConfigureAwait(false);
        }

        private async Task<TReturn> DbOperation<TReturn>(Func<DvbDbContext, Task<TReturn>> databaseOperation)
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<DvbDbContext>();
            return await databaseOperation(dbContext).ConfigureAwait(false);
        }
    }
}
