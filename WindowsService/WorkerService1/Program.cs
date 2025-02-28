using CliWrap;

using Serilog;

namespace WorkerService1
{
    public class Program
    {
        private static string myServiceName = "Test WorkerService1";

        public static void Main(string[] args)
        {
            if (args?.Length == 1)
            {
                CreateInstaller(args).Wait();
                return;
            }

            var builder = Host.CreateApplicationBuilder(args);

            var appSettingsConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            //https://github.com/serilog/serilog-settings-configuration

            Serilog.ILogger logger = new LoggerConfiguration()
               .ReadFrom.Configuration(appSettingsConfig)
               .CreateLogger();

            var config = appSettingsConfig.GetSection("Config").Get<Config>();

            builder.Services.AddSingleton(logger);
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<RequestCreator>();
            builder.Services.AddSingleton<RequestProcessor>();
            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }

        private static async Task CreateInstaller(string[] args)
        {
            try
            {
                string executablePath =
                    Path.Combine(AppContext.BaseDirectory, "WorkerService1.exe");

                if (args[0] is "/Install")
                {
                    await Cli.Wrap("sc")
                        .WithArguments(new[] { "create", myServiceName, $"binPath={executablePath}", "start=auto", "displayname=Test WorkerService1" })
                        .ExecuteAsync();
                    await Cli.Wrap("sc")
                       .WithArguments(new[] { "description", myServiceName, "Test WorkerService1" })
                       .ExecuteAsync();
                }
                else if (args[0] is "/Uninstall")
                {
                    try
                    {
                        await Cli.Wrap("sc")
                       .WithArguments(new[] { "stop", myServiceName })
                       .ExecuteAsync();
                    }
                    catch (Exception)
                    {
                    }
                    await Cli.Wrap("sc")
                        .WithArguments(new[] { "delete", myServiceName })
                        .ExecuteAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}