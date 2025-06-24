namespace CybersecurityChatbot
{
    public class QuizScore
    {
        public int Correct { get; set; }
        public int Total { get; set; }

        public double Percentage
        {
            get
            {
                return Total > 0 ? Math.Round((double)Correct / Total * 100, 1) : 0;
            }
        }
    }
}