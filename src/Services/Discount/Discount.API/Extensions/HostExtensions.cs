using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Dapper;
using System;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            { 
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try {
                    logger.LogInformation("migrating database");
                    using var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand { Connection=connection })
                        {
                            command.CommandText = @"create table if not exists Coupon(
                                                            Id serial primary key, 
                                                            ProductName varchar(24) not null,
                                                            Description Text,
                                                            Amount int
                                                    )";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into Coupon(productname, description, amount) values('iPhone X','iPhone Discount',150)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into Coupon(productname, description, amount) values('Samsung 10','Samsung Discount',100)";
                            command.ExecuteNonQuery();
                        }
                    }
                    logger.LogInformation("migrating database done");
                }
                catch (NpgsqlException ex) 
                {
                }
                return host;
            }

        }
    }
}
