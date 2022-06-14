using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaInterface.ViewModels;
using Shared;
using Shared.Models;
using Shared.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaInterface
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                const int maximum = 1;

                var collection = new ServiceCollection();
                collection.AddSingleton<MainWindowViewModel>();
                collection.AddSingleton<IExecutionContext>(_ => new ExecutionContext(1000, maximum));
                collection.AddSingleton(new ExecutionConfiguration(TimeSpan.FromMilliseconds(1000), maximum, 0));

                var provider = collection.BuildServiceProvider();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = provider.GetRequiredService<MainWindowViewModel>(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}