using System;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    public class SentimentDetector
    {
        public string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower().Trim();

            // Positive sentiments
            if (Regex.IsMatch(lowerInput, @"\b(thank you|thanks|appreciate|helpful|great|awesome|excellent)\b"))
            {
                return "😊 You're very welcome! I'm glad I could help you with cybersecurity. Is there anything else you'd like to learn about?";
            }

            // Frustrated/confused sentiments
            if (Regex.IsMatch(lowerInput, @"\b(confused|frustrated|don't understand|help|lost|stuck)\b"))
            {
                return "😕 I understand this can be confusing! Cybersecurity can seem overwhelming, but I'm here to help. What specific topic would you like me to explain more clearly?";
            }

            // Worried/concerned sentiments
            if (Regex.IsMatch(lowerInput, @"\b(worried|scared|concerned|afraid|anxious|hacked|compromised)\b"))
            {
                return "😰 I understand your concern about cybersecurity. It's good that you're being cautious! Let me help you understand the risks and how to protect yourself. What's your main worry?";
            }

            // Greeting sentiments
            if (Regex.IsMatch(lowerInput, @"\b(hello|hi|hey|good morning|good afternoon|good evening)\b"))
            {
                return "👋 Hello! I'm your cybersecurity assistant. I'm here to help you stay safe online. What would you like to learn about today?";
            }

            return null; // No sentiment detected
        }
    }
}