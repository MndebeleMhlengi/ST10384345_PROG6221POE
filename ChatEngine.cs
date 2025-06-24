// ChatEngine.cs (The Orchestrator)
using System;
using System.Collections.Generic;
using System.Linq; // Required for .Any(), .ToList()

namespace CybersecurityChatbot
{
    // The central chatbot engine that coordinates interactions among different functional managers.
    public class ChatEngine
    {
        // Manager instances for different functionalities.
        private readonly ChatHistoryManager chatHistoryManager;
        private readonly TaskManager taskManager;
        private readonly QuizManager quizManager;
        private readonly ActivityLogger activityLogger;
        private readonly ResponseGenerator responseGenerator;

        // Current conversational state for multi-turn interactions.
        private ChatState currentChatState = ChatState.Idle;
        private CyberTask pendingTask; // Temporarily stores task details during multi-step addition.
        private string userName = "Guest"; // Store the user's name

        // Defines possible conversational states.
        private enum ChatState
        {
            Idle,                           // Default state, waiting for general commands.
            InQuiz,                         // User is currently taking a quiz.
            AddingTask_WaitingForTitle,     // Waiting for the title of a new task.
            AddingTask_WaitingForDescription, // Waiting for the description of a new task.
            AddingTask_WaitingForDate,      // Waiting for the reminder date/time of a new task.
            CompletingTask                  // User is prompted to provide a task number to complete.
        }

        // Constructor initializes all sub-managers and registers their callbacks.
        public ChatEngine()
        {
            // Initialize ChatHistoryManager first as other managers report messages to it.
            chatHistoryManager = new ChatHistoryManager();

            // Pass chatHistoryManager.AddSystemMessage as a callback to other managers
            // This allows them to report messages that will appear in the main chat display.
            taskManager = new TaskManager(chatHistoryManager.AddSystemMessage);
            quizManager = new QuizManager(chatHistoryManager.AddSystemMessage);
            activityLogger = new ActivityLogger(chatHistoryManager.AddSystemMessage);
            responseGenerator = new ResponseGenerator(chatHistoryManager.AddSystemMessage);

            activityLogger.LogActivity("Chatbot engine initialized.");
        }

        // --- Main Input Processing Method ---
        // This is the primary method called by the UI (MainWindow) when the user sends a message.
        public string ProcessUserInput(string input)
        {
            // First, log the user's input to both chat history and activity log.
            chatHistoryManager.AddUserMessage(input);
            activityLogger.LogActivity($"User input: \"{input}\"");

            string lowerInput = input.ToLower().Trim();
            string botResponse = string.Empty;

            // 1. Prioritize handling based on the current conversational state (multi-turn interactions).
            botResponse = HandleStateBasedInput(lowerInput);
            if (!string.IsNullOrEmpty(botResponse))
            {
                // If the input was consumed by a state handler, return its response.
                return botResponse;
            }

            // 2. If not in a multi-turn state, identify and handle direct commands/intents.
            if (lowerInput == "exit" || lowerInput == "quit" || lowerInput == "bye")
            {
                // Return a special exit command that MainWindow can detect
                botResponse = "EXIT_COMMAND";
                chatHistoryManager.AddSystemMessage($"Goodbye, {userName}! Stay safe online!");
                return botResponse;
            }
            else if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
            {
                string[] thankResponses = {
                    $"You're welcome, {userName}! Happy to help with cybersecurity!",
                    $"No problem, {userName}! Is there anything else you'd like to know about online safety?",
                    $"You're very welcome, {userName}! Feel free to ask me more questions anytime.",
                    $"Glad I could help, {userName}! What else would you like to learn about cybersecurity?"
                };
                botResponse = thankResponses[new Random().Next(thankResponses.Length)];
            }
            else if (lowerInput == "help")
            {
                botResponse = responseGenerator.GetHelpMessage();
            }
            else if (lowerInput.Contains("start quiz"))
            {
                quizManager.StartNewQuiz();
                botResponse = chatHistoryManager.GetLatestEntryContent(); // Get initial quiz question/message.
            }
            else if (lowerInput.Contains("add task"))
            {
                // Initiate the multi-step process for adding a new task.
                currentChatState = ChatState.AddingTask_WaitingForTitle;
                pendingTask = new CyberTask(); // Initialize a new CyberTask object to store details.
                botResponse = "What is the title for this task?";
            }
            else if (lowerInput.Contains("complete task"))
            {
                int taskIndex = ExtractTaskIndexFromInput(lowerInput);
                if (taskIndex != -1)
                {
                    taskManager.CompleteTask(taskIndex);
                    botResponse = chatHistoryManager.GetLatestEntryContent(); // Get response from TaskManager.
                }
                else
                {
                    // If no number provided, transition to state to ask for it.
                    currentChatState = ChatState.CompletingTask;
                    botResponse = "Which task number would you like to complete? (e.g., 'complete 1')";
                }
            }
            else if (lowerInput.Contains("view tasks") || lowerInput.Contains("show tasks"))
            {
                taskManager.ShowAllTasks();
                botResponse = chatHistoryManager.GetLatestEntryContent(); // Get response from TaskManager.
            }
            else if (lowerInput.Contains("show activity log") || lowerInput.Contains("view activity log"))
            {
                activityLogger.ShowActivityLog();
                botResponse = chatHistoryManager.GetLatestEntryContent(); // Get response from ActivityLogger.
            }
            else if (lowerInput.Contains("how many tasks"))
            {
                botResponse = $"You currently have {taskManager.GetActiveTaskCount()} active tasks.";
            }
            else if (lowerInput.Contains("my quiz score") || lowerInput.Contains("what's my score"))
            {
                int score = quizManager.GetQuizScore();
                botResponse = (score != -1) ? $"Your last quiz score was {score}%." : "A quiz is not currently in progress or hasn't been completed yet.";
            }
            else if (responseGenerator.IsCybersecurityQuestion(lowerInput))
            {
                botResponse = responseGenerator.GetCybersecurityResponse(lowerInput);
            }
            else if (lowerInput.Contains("hello") || lowerInput.Contains("hi"))
            {
                string[] greetings = {
                    $"Hello there, {userName}! How can I help you with cybersecurity today?",
                    $"Hi {userName}! Ready to learn something new about online safety?",
                    $"Greetings, {userName}! What's on your mind regarding cybersecurity?"
                };
                botResponse = greetings[new Random().Next(greetings.Length)];
            }
            else
            {
                botResponse = "I'm still learning! Can you please rephrase that, or ask about a specific cybersecurity topic? Type 'help' for options.";
            }

            // Messages generated directly by ChatEngine are also added to history.
            // For messages generated by managers, the callback takes care of it.
            // If botResponse was generated directly here, we need to add it to history.
            if (string.IsNullOrEmpty(botResponse) || !chatHistoryManager.GetLatestEntryContent().Contains(botResponse))
            {
                chatHistoryManager.AddSystemMessage(botResponse); // Ensure the response is in history.
            }

            return botResponse;
        }

        // --- Multi-turn State Handler ---
        // This method manages the flow of multi-step interactions like adding a task or taking a quiz.
        private string HandleStateBasedInput(string input)
        {
            string response = null;
            switch (currentChatState)
            {
                case ChatState.InQuiz:
                    quizManager.ProcessQuizAnswer(input);
                    response = chatHistoryManager.GetLatestEntryContent(); // Get the latest message generated by QuizManager.
                    if (!quizManager.IsQuizActive()) // Check if the quiz just ended.
                    {
                        currentChatState = ChatState.Idle; // Reset to idle if quiz is over.
                    }
                    break;

                case ChatState.AddingTask_WaitingForTitle:
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        pendingTask.Title = input.Trim();
                        currentChatState = ChatState.AddingTask_WaitingForDescription;
                        response = "Got it. Now, please provide a short description for this task.";
                    }
                    else
                    {
                        response = "The task title cannot be empty. Please try again.";
                        // Stay in this state until a valid title is provided.
                    }
                    break;

                case ChatState.AddingTask_WaitingForDescription:
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        pendingTask.Description = input.Trim();
                        currentChatState = ChatState.AddingTask_WaitingForDate;
                        response = "Okay. Do you want to set a reminder date and time? (e.g., '2025-12-31 18:00' or 'no')";
                    }
                    else
                    {
                        response = "The task description cannot be empty. Please try again.";
                        // Stay in this state until a valid description is provided.
                    }
                    break;

                case ChatState.AddingTask_WaitingForDate:
                    DateTime? reminderDate = null;
                    if (input.ToLower() != "no")
                    {
                        if (DateTime.TryParse(input, out DateTime parsedDate))
                        {
                            reminderDate = parsedDate;
                        }
                        else
                        {
                            response = "I couldn't understand that date. Please try a format like 'YYYY-MM-DD HH:MM' or type 'no' for no reminder.";
                            // Stay in this state if the date is invalid.
                            return response;
                        }
                    }

                    // All task details collected, add the task via TaskManager.
                    taskManager.AddTask(pendingTask.Title, pendingTask.Description, reminderDate);
                    response = chatHistoryManager.GetLatestEntryContent(); // Get the "Task added successfully" message.
                    currentChatState = ChatState.Idle; // Reset state to idle.
                    pendingTask = null; // Clear pending task data.
                    break;

                case ChatState.CompletingTask:
                    if (int.TryParse(input, out int taskIndex))
                    {
                        taskManager.CompleteTask(taskIndex);
                        response = chatHistoryManager.GetLatestEntryContent(); // Get completion confirmation.
                    }
                    else
                    {
                        response = "Please provide a valid task number to complete.";
                    }
                    currentChatState = ChatState.Idle; // Reset state after attempting completion.
                    break;

                default:
                    // If no specific state handles the input, return null to fall through to general intent processing.
                    return null;
            }
            // Add the state-generated response to the chat history (if it's not already added by a manager's callback)
            if (!string.IsNullOrEmpty(response) && !chatHistoryManager.GetLatestEntryContent().Contains(response))
            {
                chatHistoryManager.AddSystemMessage(response);
            }
            return response;
        }

        // --- Helper Methods ---
        // These can be extracted to an NLPHelper class if more complex parsing is needed.
        private int ExtractTaskIndexFromInput(string input)
        {
            string[] parts = input.Split(' ');
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(parts[i], out int index))
                {
                    return index;
                }
            }
            return -1; // Indicates no valid index found.
        }

        // --- Public Methods for UI Interaction (Delegating to Managers) ---
        // These methods are called directly by MainWindow.xaml.cs to interact with the chatbot's state.

        public string GetChatHistory()
        {
            return chatHistoryManager.GetChatHistory();
        }

        public void ClearChat()
        {
            chatHistoryManager.ClearHistory();
            quizManager.ResetQuizState(); // Reset quiz state when chat is cleared.
            currentChatState = ChatState.Idle; // Reset overall chat state.
            pendingTask = null; // Clear any pending task data.
            activityLogger.LogActivity("Chat cleared."); // Log the clear action.
        }

        public void ShowAllTasks()
        {
            taskManager.ShowAllTasks();
        }

        public int GetActiveTaskCount()
        {
            return taskManager.GetActiveTaskCount();
        }

        public void StartQuiz()
        {
            if (quizManager.IsQuizActive())
            {
                chatHistoryManager.AddSystemMessage("A quiz is already in progress. Please answer the current question or clear the chat to start a new one.");
                return;
            }
            quizManager.StartNewQuiz();
            currentChatState = ChatState.InQuiz; // Set state when quiz begins.
        }

        public int GetQuizScore()
        {
            return quizManager.GetQuizScore();
        }

        public void ShowActivityLog()
        {
            activityLogger.ShowActivityLog();
        }

        public int GetActivityCount()
        {
            return activityLogger.GetActivityCount();
        }

        // Provides access to tasks for the MainWindow's reminder timer.
        public List<CyberTask> GetTasksForReminderCheck()
        {
            return taskManager.GetTasksForReminderCheck();
        }

        // PUBLIC METHOD - Expose AddSystemMessage to the UI
        // This allows MainWindow to add system messages directly to the chat history
        public void AddSystemMessage(string message)
        {
            chatHistoryManager.AddSystemMessage(message);
        }

        // Set the user's name
        public void SetUserName(string name)
        {
            userName = string.IsNullOrWhiteSpace(name) ? "Guest" : name.Trim();
            activityLogger.LogActivity($"User name set to: {userName}");
        }

        // Get the user's name
        public string GetUserName()
        {
            return userName;
        }
    }
}