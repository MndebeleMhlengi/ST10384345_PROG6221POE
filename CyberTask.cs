// CyberTask.cs
using System;

namespace CybersecurityChatbot
{
    // Represents a cybersecurity task with details for management and reminders.
    public class CyberTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; } // Nullable for optional reminders
        public bool IsCompleted { get; set; } = false; // Tracks if the task is finished
        public bool IsNotified { get; set; } = false; // Prevents repeated notifications for the same overdue task

        // Provides a formatted string representation of the task for display in the UI.
        public override string ToString()
        {
            string dateInfo = ReminderDate.HasValue ? $" (Due: {ReminderDate.Value:yyyy-MM-dd HH:mm})" : "";
            string status = IsCompleted ? "[COMPLETED]" : "[ACTIVE]";
            return $"{status} {Title}{dateInfo} - {Description}";
        }
    }
}