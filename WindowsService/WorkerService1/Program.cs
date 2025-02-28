using Serilog;

namespace WorkerService1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var appSettingsConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            //https://github.com/serilog/serilog-settings-configuration

            //Serilog.ILogger logger = new LoggerConfiguration()
            //   .ReadFrom.Configuration(appSettingsConfig)
            //   .CreateLogger();

            Serilog.ILogger logger = new LoggerConfiguration()
               .ReadFrom.Configuration(appSettingsConfig)
               .CreateLogger();

            var config = appSettingsConfig.GetSection("Config").Get<Config>();

            //var config = new Config();
            //config.Bind(config);

            builder.Services.AddSingleton(logger);
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<RequestCreator>();
            builder.Services.AddSingleton<RequestProcessor>();
            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}