using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WildflyViewLog.Services;
using WildflyViewLog.ViewModels;
using WildflyViewLog.Views;

namespace WildflyViewLog
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Configurar el contenedor IoC
                var services = new ServiceCollection();
                ConfigureServices(services, desktop);

                _serviceProvider = services.BuildServiceProvider();

                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                var vm = _serviceProvider.GetRequiredService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services, IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Registrar servicios
            //services.AddSingleton<IFilePickerService>(provider =>
            //{
            //    var storageProvider = desktop.MainWindow?.StorageProvider;
            //    if (storageProvider == null)
            //        throw new InvalidOperationException("StorageProvider is not available.");

            //    return new FilePickerService(storageProvider);
            //});

            // Registrar ViewModels
            services.AddSingleton<FilePickerService>();
            services.AddSingleton<MainWindowViewModel>();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}