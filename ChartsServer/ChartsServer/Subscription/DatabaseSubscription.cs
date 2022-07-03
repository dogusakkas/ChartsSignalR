using ChartsServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using TableDependency.SqlClient;

namespace ChartsServer.Subscription
{
    public interface IDatabaseSubscription
    {
        void Configure(string tableName);
    }
    public class DatabaseSubscription<T> : IDatabaseSubscription where T : class, new()
    {
        // Appsettings.json verisini okuyabilmemiz için gerekli fonksiyon
        IConfiguration _configuration;
        IHubContext<SatisHub> _hubContext;

        SqlTableDependency<T> _tableDependency;

        public DatabaseSubscription(IConfiguration configuration, IHubContext<SatisHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }

        public void Configure(string tableName)
        {
            _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SQL"), tableName);

            _tableDependency.OnChanged += async (o, e) =>
            {
                await _hubContext.Clients.All.SendAsync("receiveMessage", "Merhaba");
            };
            _tableDependency.OnError += (o, e) =>
            {

            };
            _tableDependency.Start();
        }

        // Configure son bulacağı zaman çalışacak fonksiyon
        ~DatabaseSubscription()
        {
            _tableDependency.Stop();
        }
    }
}
