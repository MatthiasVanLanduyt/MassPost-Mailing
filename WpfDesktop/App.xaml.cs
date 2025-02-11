using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Validator.Application.DependencyInjection;
using Validator.Application.Mailings.Contracts;
using Validator.Domain.MailingResponses.Services;
using Wpf.Ui;
using WpfDesktop.Contracts;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Contracts.Views;
using WpfDesktop.DependencyInjection;
using WpfDesktop.Models;
using WpfDesktop.Services;
using WpfDesktop.ViewModels;
using WpfDesktop.Views;
using WpfDesktop.Views.Pages;

namespace WpfDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public T? GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public App()
        {
        }
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
            _host = Host.CreateDefaultBuilder(e.Args)
                    .ConfigureAppConfiguration(c =>
                    {
                        c.SetBasePath(appLocation);
                        c.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    })
                    .ConfigureServices(ConfigureServices)
                    .Build();

            await _host.StartAsync();

        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Get the AppData path
            var appDataPath = GetAppDataPath();
            EnsureAppDataFolderExists(appDataPath);

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

            // First register your infrastructure and application services
            services.AddApplicationServices();    // Add this if you have application services
            services.AddInfrastructureServices(); // This

            // App Host
            services.AddHostedService<ApplicationHostService>();

            services.AddWpfServices(appDataPath);

            services.RegisterViewModels();
            
        }

        private string GetAppDataPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppConstants.AppName
            );
        }

        private void EnsureAppDataFolderExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw new InvalidOperationException($"Unable to create application data directory at {path}", ex);
            }
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0


            e.Handled = true;
            LogException(e.Exception);
            MessageBox.Show(e.Exception.Message, $"An unhandled exception just occurred:\n\n{e.Exception.Message}\n{e.Exception.StackTrace}", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void LogException(Exception ex)
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MassPostMailing",
                "error.log"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            string errorMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}\n\n";
            File.AppendAllText(logPath, errorMessage);
        }
    }

}
