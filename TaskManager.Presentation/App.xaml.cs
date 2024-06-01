using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TaskManager.Business;
using TaskManager.Data;

namespace TaskManager.Presentation
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<TaskService>();
            services.AddTransient<UserService>();
            services.AddTransient<TaskRepository>();
            services.AddTransient<UserRepository>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
    }
}