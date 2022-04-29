using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security;
using Dapper;
using DbUp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Triton.WebApi
{
    public class Program
    {
        private static IConfiguration _config;
        public Program(IConfiguration config) { _config = config; }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build()
                .GetSection("ConnectionStrings").AsEnumerable();

            foreach (var conn in config)
            {
                if (conn.Value != null)
                {
                    var key = conn.Key.Split(':')[1];
                    var database = conn.Value;

                    // Database migration tool
                    // DatabaseMigration(database, key);
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        private static void DatabaseMigration(string connectionString, string database)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var path = Path.Combine(Environment.CurrentDirectory, $"Scripts\\{database}");
            if(Directory.Exists(path))
            {
                var upgrader =
                    DeployChanges.To
                        .SqlDatabase(connectionString)
                        .WithScriptsFromFileSystem(path)
                        .LogToTrace()
                        .Build();

                var result = upgrader.PerformUpgrade();

                //if (!result.Successful)
                //{
                //    var p = result.Error;
                //}
                //else
                //{
                //    string p = "Success!";
                //}
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
