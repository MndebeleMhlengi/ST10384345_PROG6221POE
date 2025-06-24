// ChatHistoryManager.cs
using System;
using System.Collections.Generic;
using System.Linq; // Required for .Select() and .Last()

namespace CybersecurityChatbot
{
    // Manages the storage and retrieval of all chat messages.
    public class ChatHistoryManager
    {
        private List<string> chatHistory = new List<string>();

        // Adds a system (chatbot) message to the history.
        public void AddSystemMessage(string message)
        {
            chatHistory.Add($"System: {message}");
        }

        // Adds a user's message to the history.
        public void AddUserMessage(string message)
        {
            chatHistory.Add($"User: {message}");
        }

        // Clears all entries from the chat history.
        public void ClearHistory()
        {
            chatHistory.Clear();
            AddSystemMessage("Chat history cleared."); // Log this action in the history itself
        }

        // Retrieves the entire chat history, formatted for display in the UI.
        public string GetChatHistory()
        {
            // Replaces internal prefixes ("System:", "User:") with display-friendly ones ("Bot:", "You:")
            // This line was fixed from previous syntax errors.
            return string.Join(Environment.NewLine, chatHistory.Select(line =>
                line.StartsWith("System: ") ? "Bot: " + line.Substring("System: ".Length) :
                line.StartsWith("User: ") ? "You: " + line.Substring("User: ".Length) : line));
        }

        // Retrieves the content of the most recently added message (stripping internal prefixes).
        public string GetLatestEntryContent()
        {
            if (chatHistory.Any())
            {
                string lastEntry = chatHistory.Last();
                if (lastEntry.StartsWith("System: "))
                    return lastEntry.Substring("System: ".Length);
                else if (lastEntry.StartsWith("User: "))
                    return lastEntry.Substring("User: ".Length);
                return lastEntry; // Fallback
            }
            return string.Empty; // No history
        }
    }
}