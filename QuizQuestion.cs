namespace CybersecurityChatbot
{
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public string[] Options { get; set; } = new string[0];
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; } = string.Empty;

        public string GetFormattedQuestion()
        {
            if (Options == null || Options.Length == 0)
                return Question;

            string formatted = $"{Question}\n\n";
            for (int i = 0; i < Options.Length; i++)
            {
                char letter = (char)('A' + i);
                formatted += $"{letter}) {Options[i]}\n";
            }
            formatted += "\nType your answer (A, B, C, D or 1, 2, 3, 4):";
            return formatted;
        }
    }
}