using ChatApp.DL;
using ChatApp.Managers;
using ChatApp.Managers.Interfaces;
using ChatApp.Models.Settings;
using ChatApp.Repositories;
using ChatApp.Repositories.Common;
using ChatApp.Repositories.Interfaces;
using ChatApp.WebAPI.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.WebAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Конструктор класса первоначального запуска.
        /// </summary>
        /// <param name="configuration">Конфигурация</param>
        public Startup(IConfiguration configuration)
        {
            // Сохраняем конфигурацию для дальнейшего имспользования
            Configuration = configuration;
        }

        // Конфигурация
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        /// <summary>
        /// Конфигурировать сервисы.
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Получаем секцию настроек приложения из конфигурации
            var appSettingsSection = Configuration.GetSection("AppSettings");
            // Получаем настройки приложения
            var appSettings = appSettingsSection.Get<AppSettings>();

            // Добавляем и настраиваем контроллеры обработки JSON
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            //Добавляем и настраиваем контекст базы данных
            services.AddDbContext<ChatAppContext>(options =>
            {
                // Задаём использование MS SQL Server
                // И задаём строку подключения в БД из файла конфигурации
                options.UseSqlServer(Configuration.GetConnectionString("ChatAppDbContext"));
            });
            // Добавляем контроллеры
            services.AddControllers();
            // Добавляем SignalR
            services.AddSignalR();

            // Configure jwt authentication.
            // Получаем секретный ключ из файла конфигурации
            var secretKey = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            // Добавляем и настраиваем аутентификацию
            services.AddAuthentication(x =>
            {
                // Задаём схему футентификации - JWT Bearer
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //Задаём схему проверки - JWT Bearer
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // настраиваем JWT Bearar
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = true,
                    ValidIssuer = appSettings.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudiences = new List<string> { appSettings.JwtMobileAudience, appSettings.JwtWebAudience },
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Задаём используемые сервисы,
            // которые будут автоматически подстваыляться при вызовах конструкторов
            services.AddScoped<IUsersManager, UsersManager>();
            services.AddScoped<ITokensManager, TokensManager>();
            services.AddScoped<IConversationsManager, ConversationsManager>();

            services.AddScoped<IConversationRepliesRepository, ConversationRepliesRepository>();
            services.AddScoped<IConversationsRepository, ConversationsRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IConnectionsRepository, ConnectionsRepository>();
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Конфигурируем настройки приложения
            services.Configure<AppSettings>(appSettingsSection);

            // Добавляем Swagger генератор
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatApp.WebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatApp.WebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("chathub");
            });


        }
    }
}
