using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Validator.Application.Mailings.Contracts;
using Validator.Domain.MailingResponses.Services;
using Wpf.Ui;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Contracts.Views;
using WpfDesktop.Services;
using WpfDesktop.ViewModels;
using WpfDesktop.Views.Pages;
using WpfDesktop.Views;
using Windows.Storage;
using Microsoft.Extensions.Options;
using WpfDesktop.Models;

namespace WpfDesktop.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddWpfServices(this IServiceCollection services, string appDataPath)
        {
            // Services
            services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<ISettingsService>(provider => new SettingsService(appDataPath));
            services.AddSingleton<IContactService>(provider => new ContactService(appDataPath, provider.GetRequiredService<IOptions<AppConfig>>()));
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<Contracts.Services.INavigationService, Services.NavigationService>();
            services.AddSingleton<ApplicationState>();
            services.AddSingleton<ISnackbarService, SnackbarService>();

            services.AddSingleton<IMailingResponseParser>(sp =>
            {
                var json = File.ReadAllText("statuscodes.json");
                return new XmlMailingResponseParser(json);
            });

            return services;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
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
            // Register other application services
            return services;
        }
    }
    
}
