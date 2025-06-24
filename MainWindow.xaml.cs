// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq; // For LINQ methods like .Any()
using System.Threading.Tasks; // For Task.Run
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // For KeyEventArgs
using System.Windows.Media.Imaging; // For BitmapImage
using System.IO; // For File.Exists
using System.Windows.Threading; // For DispatcherTimer

namespace CybersecurityChatbot
{
    /// <summary>
    /// Main window for the Cybersecurity Chatbot application.
    /// Handles user interface interactions and orchestrates communication with the ChatEngine.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields
        private ChatEngine chatEngine;
        private DispatcherTimer reminderTimer; // Periodically checks for overdue tasks.
        private string userName = "Guest"; // Stores the user's name.
        private Utils utils; // Optional: for sound playback or other utility functions.

        // Constants for file paths and default values.
        private const string DEFAULT_LOGO_PATH = @"Images\logo.png"; // Path to the application logo.
        private const string WELCOME_TITLE = "Welcome";
        #endregion

        #region Constructor and Initialization
        public MainWindow()
        {
            InitializeComponent(); // Initializes WPF components from XAML.
            // DO NOT call InitializeApplicationAsync() directly here.
            // We need to wait for the MainWindow to be loaded before prompting for user name.
            this.Loaded += MainWindow_Loaded; // Subscribe to the Loaded event
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe to prevent multiple calls if the event somehow fires again
            this.Loaded -= MainWindow_Loaded;
            await InitializeApplicationAsync(); // Now call the async initialization
        }

        private async Task InitializeApplicationAsync() // Changed to Task return type for await
        {
            try
            {
                chatEngine = new ChatEngine(); // Instantiate the core chatbot engine.
                utils = new Utils(); // Instantiate utility class (if you keep Utils.cs).

                // Perform UI-related initialization on the UI thread using Dispatcher.InvokeAsync.
                await Dispatcher.InvokeAsync(() =>
                {
                    PromptForUserName(); // Prompts the user for their name.
                    UpdateUserGreeting(); // Updates the greeting in the UI.
                    ShowWelcomeMessages(); // Displays initial welcome messages from the bot.
                    PlayGreetingSound(); // Plays a welcome sound (if implemented).
                    DisplayLogo(); // Displays the application logo.
                    UpdateStatus("Ready - Ask me about cybersecurity!"); // Sets initial status bar text.
                    userInputTextBox?.Focus(); // Sets focus to the input text box.
                });

                SetupReminderTimer(); // Configures and starts the background reminder timer.
            }
            catch (Exception ex)
            {
                HandleError("Failed to initialize application", ex);
            }
        }

        // Configures and starts the DispatcherTimer for checking reminders.
        private void SetupReminderTimer()
        {
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromSeconds(30); // Checks every 30 seconds.
            reminderTimer.Tick += ReminderTimer_Tick; // Assigns the event handler.
            reminderTimer.Start();
        }

        // Event handler for the reminder timer's Tick event.
        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            // Retrieve tasks that are active, have a reminder date, and haven't been notified yet.
            var tasksToCheck = chatEngine.GetTasksForReminderCheck();

            foreach (var task in tasksToCheck)
            {
                if (task.ReminderDate.HasValue && task.ReminderDate.Value <= DateTime.Now)
                {
                    // Add a reminder message to the chat history via ChatEngine.
                    chatEngine.AddSystemMessage($"\n🔔 REMINDER: Your task '{task.Title}' is due! " +
                                                $"{(!string.IsNullOrEmpty(task.Description) ? $"({task.Description})" : "")}\n");
                    task.IsNotified = true; // Mark the task as notified to prevent repetitive alerts.
                    UpdateChatDisplay(); // Refresh the chat display to show the reminder.
                }
            }
        }

        // Handles the window closing event to stop the reminder timer gracefully.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            reminderTimer?.Stop(); // Stops the timer to release resources.
            // Persistence logic (saving tasks, history, etc.) would go here.
        }
        #endregion

        #region User Greeting and Welcome
        // Prompts the user for their name using a custom input dialog
        private void PromptForUserName()
        {
            try
            {
                // Ensure the main window is visible before showing the input dialog with Owner set.
                // This is now implicitly handled by calling this method after MainWindow_Loaded.
                string inputName = ShowInputDialog("Hi there! What's your name?", WELCOME_TITLE, "Guest");

                if (string.IsNullOrWhiteSpace(inputName))
                {
                    MessageBox.Show("No name provided. You will be referred to as 'Guest'.", WELCOME_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                    userName = "Guest";
                }
                else
                {
                    userName = inputName.Trim();
                }

                // Set the username in the ChatEngine
                chatEngine.SetUserName(userName);
            }
            catch (Exception ex)
            {
                HandleError("Failed to get user name", ex);
                userName = "Guest";
                chatEngine.SetUserName(userName);
            }
        }

        // Custom input dialog method to replace VB.NET InputBox
        private string ShowInputDialog(string prompt, string title, string defaultValue)
        {
            Window inputDialog = new Window()
            {
                Width = 400,
                Height = 200,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this, // 'this' (MainWindow) should now be visible when this is called.
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel() { Margin = new Thickness(20) };

            TextBlock promptText = new TextBlock()
            {
                Text = prompt,
                Margin = new Thickness(0, 0, 0, 15),
                TextWrapping = TextWrapping.Wrap
            };

            TextBox inputBox = new TextBox()
            {
                Text = defaultValue,
                Margin = new Thickness(0, 0, 0, 15),
                Padding = new Thickness(5)
            };

            StackPanel buttonPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button okButton = new Button()
            {
                Content = "OK",
                Width = 75,
                Height = 25,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };

            Button cancelButton = new Button()
            {
                Content = "Cancel",
                Width = 75,
                Height = 25,
                IsCancel = true
            };

            okButton.Click += (s, e) => inputDialog.DialogResult = true;
            cancelButton.Click += (s, e) => inputDialog.DialogResult = false;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(promptText);
            panel.Children.Add(inputBox);
            panel.Children.Add(buttonPanel);

            inputDialog.Content = panel;
            inputBox.Focus();

            bool? result = inputDialog.ShowDialog(); // This is where the error occurred
            return result == true ? inputBox.Text : defaultValue;
        }

        // Updates the greeting TextBlock in the UI with the user's name.
        private void UpdateUserGreeting()
        {
            if (userGreetingTextBlock != null)
            {
                userGreetingTextBlock.Text = $"Hello, {userName}!";
            }
        }

        // Displays initial welcome messages from the chatbot into the chat history.
        private void ShowWelcomeMessages()
        {
            chatEngine.ProcessUserInput("hello"); // Simulate user saying "hello" to get a dynamic greeting.
            chatEngine.AddSystemMessage($"I'm the Cybersecurity Guardian, ready to help you learn and stay safe online, {userName}!");
            chatEngine.AddSystemMessage("Type 'help' to see what I can do, ask me about cybersecurity topics, or type 'exit' to quit.");
            UpdateChatDisplay();
        }

        // Plays a greeting sound if the Utils class and sound file are available.
        private void PlayGreetingSound()
        {
            try
            {
                utils?.PlayGreeting(); // Calls PlayGreeting method from Utils instance.
            }
            catch (Exception ex)
            {
                // Silently handle sound playback errors
                Console.WriteLine($"Could not play greeting sound: {ex.Message}");
            }
        }

        // Displays the application logo from the specified path.
        private void DisplayLogo()
        {
            try
            {
                // Constructs the full path to the logo file.
                string logoFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_LOGO_PATH);
                if (File.Exists(logoFullPath))
                {
                    // Sets the Image control's source. UriKind.Absolute is used as Path.Combine returns absolute.
                    logoImage.Source = new BitmapImage(new Uri(logoFullPath, UriKind.Absolute));
                }
                else
                {
                    Console.WriteLine($"Logo file not found: {logoFullPath}");
                    // Optionally, provide a placeholder image or hide the control.
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to display logo", ex);
            }
        }
        #endregion

        #region Event Handlers (UI Interactions)

        // Handles the click event for the Send button.
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInputAndDisplay();
        }

        // Handles key down events in the user input text box (specifically for Enter key).
        private async void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessUserInputAndDisplay();
            }
        }

        // Centralized method to process user input and update the UI.
        private async Task ProcessUserInputAndDisplay()
        {
            string userInput = userInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                chatEngine.AddSystemMessage("Please type something before sending.");
                UpdateChatDisplay();
                return;
            }

            userInputTextBox.Clear(); // Clear the input field immediately after sending.

            // Process the user input on a background thread to keep the UI responsive.
            // The ChatEngine handles adding messages to its internal history.
            string botResponse = await Task.Run(() => chatEngine.ProcessUserInput(userInput));

            // Check if the user wants to exit
            if (botResponse == "EXIT_COMMAND")
            {
                UpdateChatDisplay(); // Show the goodbye message first
                await Task.Delay(1500); // Brief delay to show the goodbye message
                ExitApplication();
                return;
            }

            UpdateChatDisplay(); // Refresh the chat display with new messages.
        }

        // Method to handle application exit
        private void ExitApplication()
        {
            try
            {
                reminderTimer?.Stop();
                chatEngine.AddSystemMessage("Application closing...");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                HandleError("Error during application exit", ex);
                Environment.Exit(0); // Force exit if normal shutdown fails
            }
        }

        // Handles the click event for the Clear Chat button.
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            chatEngine.ClearChat(); // Clear chat history via ChatEngine.
            UpdateChatDisplay(); // Refresh UI.
            UpdateStatus("Chat cleared. Ready."); // Update status bar.
        }

        // Handles the click event for the Start Quiz button.
        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            chatEngine.StartQuiz(); // Start quiz via ChatEngine.
            UpdateChatDisplay(); // Refresh UI.
            UpdateStatus("Quiz started!"); // Update status bar.
        }

        // Handles the click event for the View Tasks button.
        private void ViewTasksButton_Click(object sender, RoutedEventArgs e)
        {
            chatEngine.ShowAllTasks(); // Display tasks via ChatEngine.
            UpdateChatDisplay(); // Refresh UI.
            UpdateStatus($"Displaying tasks. Active: {chatEngine.GetActiveTaskCount()}"); // Update status bar.
        }

        // Handles the click event for the Add Task button.
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Simulate typing "add task" to initiate the multi-step task addition in ChatEngine.
            userInputTextBox.Text = "add task";
            _ = ProcessUserInputAndDisplay(); // Process this simulated input (fire and forget).
        }

        // Handles the click event for the Activity Log button.
        private void ShowActivityLogButton_Click(object sender, RoutedEventArgs e)
        {
            chatEngine.ShowActivityLog(); // Display activity log via ChatEngine.
            UpdateChatDisplay(); // Refresh UI.
            UpdateStatus($"Displaying activity log. Entries: {chatEngine.GetActivityCount()}"); // Update status bar.
        }

        // Handles global key down events for shortcuts (Ctrl+L, Ctrl+H, F1).
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.L)
            {
                ClearButton_Click(this, new RoutedEventArgs());
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
            {
                // Simulate user typing 'help' to trigger the help response.
                userInputTextBox.Text = "help";
                _ = ProcessUserInputAndDisplay(); // Fire and forget
            }
            else if (e.Key == Key.F1)
            {
                StartQuizButton_Click(this, new RoutedEventArgs());
            }
            else if (e.Key == Key.Escape)
            {
                // Allow Escape key to exit as well
                userInputTextBox.Text = "exit";
                _ = ProcessUserInputAndDisplay();
            }
        }

        // Manages placeholder text visibility in the user input text box.
        private void UserInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (placeholderText != null)
            {
                placeholderText.Visibility = string.IsNullOrEmpty(userInputTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        #endregion

        #region Other UI Helper Methods
        // Updates the main chat display TextBlock with the current chat history.
        private void UpdateChatDisplay()
        {
            if (chatDisplayTextBlock != null)
            {
                chatDisplayTextBlock.Text = chatEngine.GetChatHistory();

                // Scroll to bottom to show latest messages
                if (chatScrollViewer != null)
                {
                    chatScrollViewer.ScrollToBottom();
                }
            }
            UpdateStatusWithStats(); // Also update the status bar.
        }

        // Updates the status bar with a general message.
        private void UpdateStatus(string statusMessage)
        {
            if (statusLabel != null)
            {
                statusLabel.Content = statusMessage;
            }
        }

        // Updates the status bar with dynamic statistics from the chatbot engine.
        private void UpdateStatusWithStats()
        {
            int activeTasks = chatEngine.GetActiveTaskCount();
            int activities = chatEngine.GetActivityCount();
            int lastQuizScore = chatEngine.GetQuizScore();

            string status = $"User: {userName} | Tasks: {activeTasks} active | Activities: {activities}";
            if (lastQuizScore != -1) // Only show quiz score if a quiz was completed.
                status += $" | Last Quiz: {lastQuizScore}%";

            UpdateStatus(status);
        }

        // Handles and logs application errors, displaying a message box to the user.
        private void HandleError(string message, Exception ex)
        {
            Console.WriteLine($"ERROR: {message} - {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            MessageBox.Show($"{message}.\n\nDetails: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateStatus($"Error: {message}");
        }
        #endregion
    }
}