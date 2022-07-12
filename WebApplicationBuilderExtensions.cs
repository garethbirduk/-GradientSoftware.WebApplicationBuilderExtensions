using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace Gradient.WebApplicationBuilderExtensions
{
    /// <summary>
    /// Helper extensions for building the app
    /// </summary>
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplication LogSections(this WebApplication app, params string[] sections)
        {
            foreach (var section in app.Configuration.GetChildren().Where(x => sections.Contains(x.Key)))
                LogSection(section);
            return app;
        }

        public static void LogSection(IConfigurationSection section)
        {
            foreach (var item in section.AsEnumerable().Where(x => x.Value != null))
                Console.WriteLine($"{item.Key}:{item.Value}");
        }

        /// <summary>
        /// Configure the Web Application
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication ConfigureApp(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.UseHttpLogging();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            return app;
        }

        /// <summary>
        /// Add standard configuration options
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder BuildConfiguration(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
            builder.Configuration.AddEnvironmentVariables();
            return builder;
        }

        /// <summary>
        /// Configure the builder environment
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder ConfigureEnvironment(this WebApplicationBuilder builder)
        {
            builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
            builder.Environment.EnvironmentName = Environments.Development;
            return builder;
        }

        /// <summary>
        /// Configure the logging
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.AddJsonConsole();
            return builder;
        }

        /// <summary>
        /// Configure the services
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddMemoryCache();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddLogging();
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOriginsPolicy",
                    builder => builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
            });

            return builder;
        }
    }
}
    