using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Web API приложение
/// </summary>
namespace ChatApp.WebAPI
{
    /// <summary>
    /// Стартовый класс приложения
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Стартовый метод приложения.
        /// </summary>
        /// <param name="args">Аргументы из команднлой строки.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Создать построителя хоста.
        /// </summary>
        /// <param name="args">Параметры из командной строки</param>
        /// <returns>Построитель хоста</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // использовать в качеств естартового класса Startup
                    webBuilder.UseStartup<Startup>();
                });
    }
}
