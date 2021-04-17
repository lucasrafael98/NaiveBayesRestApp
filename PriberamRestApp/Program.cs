using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriberamRestApp.Classification;

namespace PriberamRestApp
{
    public class Program
    {
        static void OnProcessExit(object sender, EventArgs e)
        {
            Classifier.Instance.SaveClassifierState();
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
