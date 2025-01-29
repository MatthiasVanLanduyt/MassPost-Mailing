using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Validator.Application.DependencyInjection;
using Validator.Desktop;

namespace Validator.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Desktop.Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Add services from application layer
            services.AddApplicationServices();
            services.AddInfrastructureServices();

            // Add WPF-specific services
            services.AddSingleton<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }

}
