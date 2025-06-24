using System;
using System.Windows;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Handle any unhandled exceptions
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // You can add any application-wide initialization here
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Log the exception and show a user-friendly message
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}",
                          "Cybersecurity Chatbot - Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);

            // Mark the exception as handled to prevent application crash
            e.Handled = true;
        }
    }
}