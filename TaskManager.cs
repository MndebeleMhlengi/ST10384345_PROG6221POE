// TaskManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbot
{
    // Manages the creation, completion, and retrieval of cybersecurity tasks.
    public class TaskManager
    {
        private List<CyberTask> tasks = new List<CyberTask>();
        private Action<string> addSystemMessageCallback; // Delegate to send messages back to ChatEngine/ChatHistoryManager

        // Constructor requires a callback to report messages back to the chat history.
        public TaskManager(Action<string> addSystemMessageCallback)
        {
            this.addSystemMessageCallback = addSystemMessageCallback ?? throw new ArgumentNullException(nameof(addSystemMessageCallback));
        }

        // Adds a new cybersecurity task to the list.
        public void AddTask(string title, string description, DateTime? reminderDate)
        {
            CyberTask newTask = new CyberTask
            {
                Title = title,
                Description = description,
                ReminderDate = reminderDate,
                IsCompleted = false,
                IsNotified = false // Initialize as not notified
            };
            tasks.Add(newTask);
            string reminderInfo = reminderDate.HasValue ? $" with a reminder for {reminderDate.Value:yyyy-MM-dd HH:mm}" : "";
            addSystemMessageCallback($"Task '{title}' added successfully{reminderInfo}.");
        }

        // Marks a task as completed based on its 1-based index in the *active* tasks list.
        public void CompleteTask(int taskNumber)
        {
            // Get only active tasks to ensure the user's index matches the displayed list.
            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();

            if (taskNumber > 0 && taskNumber <= activeTasks.Count)
            {
                CyberTask taskToComplete = activeTasks[taskNumber - 1]; // Adjust to 0-based index
                taskToComplete.IsCompleted = true; // Mark the actual task object as completed
                addSystemMessageCallback($"Task '{taskToComplete.Title}' marked as complete. Well done!");
            }
            else
            {
                addSystemMessageCallback("Invalid task number. Please provide a valid number from the active tasks list.");
            }
        }

        // Displays all active (uncompleted) tasks in the chat.
        public void ShowAllTasks()
        {
            var activeTasks = tasks.Where(t => !t.IsCompleted).ToList();
            if (activeTasks.Any())
            {
                StringBuilder sb = new StringBuilder("Your current active tasks:\n");
                for (int i = 0; i < activeTasks.Count; i++)
                {
                    sb.AppendLine($"- {i + 1}. {activeTasks[i].ToString()}"); // Use ToString() of CyberTask for full details
                }
                addSystemMessageCallback(sb.ToString());
            }
            else
            {
                addSystemMessageCallback("You have no active tasks currently. Great job!");
            }
        }

        // Gets the total count of active (uncompleted) tasks.
        public int GetActiveTaskCount()
        {
            return tasks.Count(t => !t.IsCompleted);
        }

        // Retrieves a list of active tasks with reminders that have not yet been notified.
        // This is primarily for the MainWindow's reminder timer.
        public List<CyberTask> GetTasksForReminderCheck()
        {
            return tasks.Where(t => !t.IsCompleted && t.ReminderDate.HasValue && !t.IsNotified).ToList();
        }
    }
}