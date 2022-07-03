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

        SqlTableDependency<T> _tableDependency;

        public DatabaseSubscription(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(string tableName)
        {
            _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SQL"),tableName);
            _tableDependency.OnChanged += (o, e) =>
            {

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
