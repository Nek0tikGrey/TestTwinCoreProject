using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestTwinCoreProject.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace TestTwinCoreProject.Utility
{
    public class TimerDeletingHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private string connectionString;

        public TimerDeletingHostedService( IConfiguration Configuration)
        {
            this.connectionString = Configuration.GetConnectionString("DefaultConnection");
        }
        private void Delete()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "DELETE from AspNetUsers where IsDeleted=1 and DeletedDate <= GETDATE()-DAY(30)";
                db.Execute(sqlQuery);
            }
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Delete();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
