// ActivityLogger.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbot
{
    // Logs user and system activities for review and troubleshooting.
    public class ActivityLogger
    {
        private List<string> activityLog = new List<string>();
        private Action<string> addSystemMessageCallback; // Delegate to send messages back to ChatEngine/ChatHistoryManager

        // Constructor requires a callback to report messages back to the chat history.
        public ActivityLogger(Action<string> addSystemMessageCallback)
        {
            this.addSystemMessageCallback = addSystemMessageCallback ?? throw new ArgumentNullException(nameof(addSystemMessageCallback));
        }

        // Adds a new activity entry with a timestamp to the log.
        public void LogActivity(string activity)
        {
            activityLog.Add($"{DateTime.Now:yyyy-MM-dd HH:mm}: {activity}");
            if (activityLog.Count > 50) // Keep log size manageable (e.g., last 50 activities)
            {
                activityLog.RemoveAt(0); // Remove the oldest entry
            }
        }

        // Displays the current activity log in the chat.
        public void ShowActivityLog()
        {
            if (activityLog.Any())
            {
                StringBuilder sb = new StringBuilder("Recent activities:\n");
                foreach (var activity in activityLog)
                {
                    sb.AppendLine($"- {activity}");
                }
                addSystemMessageCallback(sb.ToString());
            }
            else
            {
                addSystemMessageCallback("No activities logged yet.");
            }
        }

        // Gets the total count of logged activities.
        public int GetActivityCount()
        {
            return activityLog.Count;
        }
    }
}