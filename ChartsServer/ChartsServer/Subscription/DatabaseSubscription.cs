using ChartsServer.Hubs;
using ChartsServer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using TableDependency.SqlClient;
using System.Linq;
using System.Collections.Generic;

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

            // Veritabanında yapılan herhangi bir değişikliği bu metot sayesinde görüntüleyebiliriz
            _tableDependency.OnChanged += async (o, e) =>
            {
                

                SatisDbContext context = new SatisDbContext();

                var data = (from personel in context.Personellers
                            join satis in context.Satislars
                            on personel.Id equals satis.PersonelId
                            select new
                            {
                                personel,
                                satis
                            }).ToList();

                List<object> datas = new List<object>();
                var personelIsimleri = data.Select(x => x.personel.Adi).Distinct().ToList();

                personelIsimleri.ForEach(x =>
                {
                    datas.Add(new
                    {
                        name = x,
                        data = data.Where(y => y.personel.Adi == x).Select(s => s.satis.Fiyat).ToList()
                    });
                });
                await _hubContext.Clients.All.SendAsync("receiveMessage", datas);
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
