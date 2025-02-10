using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Validator.Application.DependencyInjection;
using Wpf.Ui;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Contracts.Views;
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

            // Configuration
            services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));

            // First register your infrastructure and application services
            services.AddApplicationServices();    // Add this if you have application services
            services.AddInfrastructureServices(); // This

            // App Host
            services.AddHostedService<ApplicationHostService>();

            // Activation Handlers

            // Core Services
            //services.AddSingleton<IFileService, FileService>();

            // Services
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<Contracts.Services.IPageService, PageService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddSingleton<Contracts.Services.INavigationService, Services.NavigationService>();
            services.AddSingleton<ApplicationState>();
            services.AddSingleton<ISnackbarService, SnackbarService>();


            // Views and ViewModels
            services.AddTransient<IShellWindow, MainWindow>();
            services.AddTransient<MainWindowViewModel>();
            
            services.AddTransient<HomePage>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<UploadPage>();
            services.AddTransient<UploadViewModel>();
            services.AddTransient<ValidationPage>();
            services.AddTransient<ValidationViewModel>();
            services.AddTransient<ContactsPage>();
            services.AddTransient<ContactsViewModel>();


           
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0


            e.Handled = true;
            MessageBox.Show(e.Exception.Message, $"An unhandled exception just occurred:\n\n{e.Exception.Message}\n{e.Exception.StackTrace}", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

}
