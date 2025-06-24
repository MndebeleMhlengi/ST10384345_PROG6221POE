using System;

namespace CybersecurityChatbot
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string FavoriteCybersecurityTopic { get; set; } = string.Empty;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public int QuizzesTaken { get; set; } = 0;
        public int TasksCompleted { get; set; } = 0;
    }
}