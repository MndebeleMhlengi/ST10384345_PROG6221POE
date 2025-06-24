using CybersecurityChatbot;
using System;

public class NLPIntent
{
    public IntentType Type { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public DateTime? ReminderDate { get; set; }
    public int? TaskIndex { get; set; }
    public string QuizAnswer { get; set; } = string.Empty;
    public string OriginalInput { get; set; } = string.Empty;
}