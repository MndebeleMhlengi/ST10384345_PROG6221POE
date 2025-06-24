# ST10384345_PROG6221POE
Cybersecurity Awareness Chatbot
This is a WPF (Windows Presentation Foundation) C# application designed to serve as an interactive cybersecurity awareness chatbot. It provides users with information on various cybersecurity topics, helps manage personal cybersecurity-related tasks, and includes a mini-game quiz to test their knowledge. The chatbot incorporates basic Natural Language Processing (NLP) simulation for more responsive interactions.

Features
Interactive Chat Interface: A user-friendly graphical interface built with WPF.

Dynamic Greetings: Greets users by name and provides personalized welcome messages.

Cybersecurity Knowledge Base: Answers questions on various cybersecurity topics using keyword detection (e.g., phishing, malware, passwords, MFA, encryption, firewall, VPN, ransomware, scams, etc.).

Personal Task Assistant:

Add Tasks: Users can add new cybersecurity tasks with a title, description, and an optional reminder date/time through a multi-step conversational flow.

View Tasks: Displays all active and completed tasks.

Complete Tasks: Allows users to mark tasks as completed.

Delete Tasks: Enables users to remove tasks from their list.

Reminders: Notifies users about overdue tasks via a background timer.

Cybersecurity Quiz Mini-Game:

Tests user knowledge with a mixed set of multiple-choice and True/False questions.

Provides immediate feedback and explanations for each answer.

Tracks and displays a final score percentage at the end of the quiz.

Activity Log: Records significant user and system interactions (e.g., chatbot initialization, user input, task additions/completions/deletions, quiz starts/completions). Displays the most recent activities.

Error Handling: Includes basic error handling for common issues, such as file not found or invalid user input.

Keyboard Shortcuts:

Ctrl + L: Clear Chat

Ctrl + H: Display Help Message

F1: Start Quiz

Esc: Exit Application

Project Structure
The project follows a modular design, separating concerns into different manager classes for better organization and maintainability.

CybersecurityChatbot/
├── Properties/
├── audio/
│   └── greeting.wav          // Sound file for greeting
├── Images/
│   └── logo.png              // Application logo
├── ActivityLogger.cs         // Manages logging of chatbot activities.
├── App.config
├── App.xaml
├── App.xaml.cs
├── ChatEngine.cs             // The core orchestrator; processes user input and delegates to managers.
├── ChatHistoryManager.cs     // Manages the history of chat messages (user and system).
├── CyberTask.cs              // Defines the structure for a cybersecurity task.
├── IntentType.cs             // (If present) Defines types of user intents.
├── MainWindow.xaml           // XAML for the main application window (UI layout).
├── MainWindow.xaml.cs        // Code-behind for MainWindow; handles UI interactions and updates.
├── NLPProcessor.cs           // (If present) Advanced NLP logic.
├── NLPIntent.cs              // (If present) Defines specific NLP intents.
├── Program.cs                // Application entry point.
├── QuizManager.cs            // Manages the quiz game logic.
├── QuizQuestion.cs           // Defines the structure for a single quiz question.
├── QuizResult.cs             // (If present) Stores quiz results.
├── QuizScore.cs              // (If present) Manages quiz scores.
├── ResponseGenerator.cs      // Generates contextual responses based on recognized keywords.
├── SentimentDetector.cs      // (If present) Detects sentiment in user input.
├── TaskManager.cs            // Manages the creation, completion, and deletion of tasks.
├── User.cs                   // (If present) User profile management.
└── Utils.cs                  // Utility functions, e.g., for sound playback.

How to Run
Prerequisites:

Visual Studio (2019 or newer recommended)

.NET Framework (4.7.2 or newer) or .NET Core (3.1 or newer, depending on project target)

Clone the Repository (or open the project files):

git clone <your-repository-url>
cd CybersecurityChatbot

Open in Visual Studio:

Open the CybersecurityChatbot.sln file in Visual Studio.

Build the Solution:

Go to Build > Build Solution or press Ctrl + Shift + B.

Run the Application:

Press F5 or click Debug > Start Debugging.

The application window will appear. It will first prompt you for your name.

Usage
Type your questions or commands into the input box at the bottom.

Press Enter or click the Send button to submit your message.

Use the dedicated buttons (Clear Chat, View Tasks, Start Quiz, Add Task, Activity Log) for quick access to features.

Utilize keyboard shortcuts for efficiency.

Example Commands:
hello / hi

What is phishing?

Tell me about passwords

add task (then follow the prompts)

view tasks

complete task 2

delete task 1

start quiz

my quiz score

show activity log

help

exit / quit / bye

Customization
Quiz Questions: Modify QuizManager.cs to add, remove, or change quiz questions.

Cybersecurity Responses: Update ResponseGenerator.cs to expand the knowledge base and add more varied responses for different keywords.

Sound Effects: Change audio/greeting.wav or implement more sound effects in Utils.cs.

UI Elements: Adjust MainWindow.xaml to customize the layout, styling, and controls.

Activity Log Detail: Refine ActivityLogger.cs to log more specific details or change the display limit.

GitHub Link:https://github.com/MndebeleMhlengi/ST10384345_PROG6221POE.git
YouTube Link:https://youtu.be/Kursa3At0gU
