using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VersandTracks0r.Data;
using VersandTracks0r.Models;

namespace VersandTracks0r.Services
{
    public class BackgroundTracker : IHostedService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IEnumerable<IShipmentTracker> trackers;
        private readonly GeoCoder geoCoder = new GeoCoder();
        private readonly AppSettings appSettings = new AppSettings();

        public Task StartAsync(CancellationToken cancellationToken)
        {

            Task.Run(Tick);
            
            return Task.CompletedTask;
        }

        public BackgroundTracker(IEnumerable<IShipmentTracker> trackers, IServiceScopeFactory scopeFactory)
        {
            this.trackers = trackers;
            this.scopeFactory = scopeFactory;
        }

        private void Tick()
        {
            Thread.Sleep(2000);
            
            while (true)
            {
                Console.WriteLine("=====================");
                Console.WriteLine("TICK");
                Console.WriteLine("=====================");

                using var scope = this.scopeFactory.CreateScope();
                using var ctx = scope.ServiceProvider.GetService<AppDbContext>();
                //ctx.Database.ExecuteSqlRaw("PRAGMA foreign_keys=OFF;");
                //ctx.Database.ExecuteSqlRaw("PRAGMA ignore_check_constraints=true;");

                // TODO: updated und status keine custom getter sondern direkt db sonst kann man hier nicht filtern mit where

                var shipments = ctx.Shipment
                    .Where(s => !s.IsDeleted)
                    .Include(s => s.History).ToList();

                var rwl = new ReaderWriterLockSlim();
                var tasks = new List<Task>();
                foreach (var shipment in shipments)
                {
                    var task = Task.Run(() =>
                    {
                        PackageTracker.track(shipment,trackers,appSettings,geoCoder);
                        rwl.EnterWriteLock();
                        ctx.SaveChanges();
                        rwl.ExitWriteLock();
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                Thread.Sleep(5000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
