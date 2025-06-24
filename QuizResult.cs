namespace CybersecurityChatbot
{
    public class QuizResult
    {
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; }
        public string CorrectAnswerText { get; set; } = string.Empty; 
    }
}