using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot
{
    internal class Chatbot
    {
        public string UserName { get; set; }

        private readonly string _voiceGreetingFile = "greeting.wav";
        private readonly string _asciiArt = @"
    _   _   _   _   _   _   _   _   _
   / \ / \ / \ / \ / \ / \ / \ / \ / \
  | C | y | b | e | r | s | e | c | u |
   \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/ \_/
    _   _   _   _   _   _
   / \ / \ / \ / \ / \ / \
  | A | w | a | r | e | n |
   \_/ \_/ \_/ \_/ \_/ \_/
        _   _   _
       / \ / \ / \
      | B | o | t |
       \_/ \_/ \_/
";

        public async Task PlayVoiceGreeting()
        {
            try
            {
                string soundPath = @"""C:\Users\lab_services_student\Desktop\greeting.wav""";
                if (File.Exists(soundPath))
                {
                    using (SoundPlayer player = new SoundPlayer("audio/greeting.wav"))
                    {
                        player.Load();
                        player.PlaySync();
                        await Task.Delay(100);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Voice greeting file not found.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
            }
        }

        public void ShowImage()
        {
            string imagePath = @"C:\Users\lab_services_student\Desktop\CybersecurityChatbot\image and audio\OIP.jpg";

            if (File.Exists(imagePath))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = imagePath,
                        UseShellExecute = true // Opens with default image viewer
                    });
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error opening image: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Image file not found.");
                Console.ResetColor();
            }
        }


        public void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(_asciiArt);
            Console.ResetColor();
        }

        public void GreetUser()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("What is your name? ");
            Console.ResetColor();
            UserName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(UserName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Name cannot be empty. Please enter your name:");
                Console.ResetColor();
                UserName = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            TypeText($"Hello, {UserName}! Welcome to the Cybersecurity Awareness Bot.");
            Console.ResetColor();
        }

        public void StartChatting()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nAsk me a cybersecurity-related question (or type 'exit' to quit):");
                Console.ResetColor();
                Console.Write("> ");
                string userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter a valid question.");
                    Console.ResetColor();
                    continue;
                }

                if (userInput.ToLower().Trim() == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Thank you for chatting! Stay safe online.");
                    Console.ResetColor();
                    break;
                }

                string response = GetResponse(userInput);
                Console.ForegroundColor = ConsoleColor.Blue;
                TypeText($"Bot: {response}");
                Console.ResetColor();

                if (userInput.ToLower().Contains("password"))
                {
                    OfferPasswordTips();
                }
            }
        }

        private string GetResponse(string query)
        {
            query = query.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                return "I didn't quite understand that. Could you rephrase?";
            }

            if (query.Contains("how are you"))
            {
                return "I'm doing well, thank you for asking!";
            }
            else if (query.Contains("what's your purpose") || query.Contains("what is your purpose"))
            {
                return "My purpose is to provide you with information and raise awareness about cybersecurity topics.";
            }
            else if (query.Contains("what can i ask you about"))
            {
                return "You can ask me about topics like password safety, phishing, safe browsing, malware, social engineering, and general online security tips.";
            }
            else if (query.Contains("password safety") || query.Contains("strong password"))
            {
                return "For strong password safety, use a combination of uppercase and lowercase letters, numbers, and symbols. Avoid using personal information and aim for a password that is at least 12 characters long. Consider using a password manager and enable multi-factor authentication (MFA).";
            }
            else if (query.Contains("phishing"))
            {
                return "Phishing is a type of online fraud where attackers try to trick you into revealing personal information, such as passwords or credit card details, often through deceptive emails or websites. Be cautious of unsolicited messages and always verify the sender's authenticity. Look for suspicious links and never provide sensitive info via email.";
            }
            else if (query.Contains("safe browsing") || query.Contains("browse safely"))
            {
                return "To browse safely, keep your web browser and antivirus software up to date. Be wary of suspicious links and websites, and avoid downloading files from untrusted sources. Use HTTPS for secure connections when entering sensitive information. Avoid public Wi-Fi for sensitive tasks.";
            }
            else if (query.Contains("malware"))
            {
                return "Malware is malicious software designed to damage or disrupt systems. Avoid downloading files from untrusted sources, use antivirus software, and be cautious of email attachments. Regularly update your software to patch vulnerabilities.";
            }
            else if (query.Contains("social engineering"))
            {
                return "Social engineering is the manipulation of people to gain access to sensitive information. Be cautious of unsolicited requests for personal information, and verify the identity of anyone asking for such details. Never share passwords or sensitive data with unknown parties.";
            }
            else if (query.Contains("data privacy"))
            {
                return "Data privacy is crucial. Be mindful of the information you share online, and understand the privacy policies of the services you use. Use strong passwords, enable MFA, and limit the amount of personal data you provide. Keep your software updated.";
            }
            else if (query.Contains("mobile security"))
            {
                return "For mobile security, use a strong passcode or biometric authentication. Only download apps from official app stores. Keep your device's operating system updated and be cautious of public Wi-Fi. Review app permissions regularly.";
            }
            else if (query.Contains("ransomware"))
            {
                return "Ransomware is a type of malware that encrypts your files and demands payment for their release. Regularly back up your data, avoid clicking on suspicious links or attachments, and keep your software updated to prevent ransomware attacks.";
            }
            else
            {
                return "I didn't quite understand that. Could you rephrase your question or ask about password safety, phishing, safe browsing, malware, social engineering, or data privacy?";
            }
        }
        private void OfferPasswordTips()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nWould you like to learn about password safety tips? (yes/no)");
            Console.ResetColor();
            Console.Write("> ");
            string answer = Console.ReadLine()?.ToLower();

            if (answer == "yes")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                TypeText("\nHere are some password safety tips:");
                TypeText("1. Use strong, unique passwords for each account.");
                TypeText("2. Avoid using personal info like birthdays or names.");
                TypeText("3. Use a password manager to store passwords securely.");
                TypeText("4. Enable Multi-Factor Authentication (MFA) wherever possible.");
                Console.ResetColor();
            }
        }

        private void TypeText(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";

            Chatbot bot = new Chatbot();

            await bot.PlayVoiceGreeting();
            bot.DisplayAsciiArt();
            bot.GreetUser();
            bot.StartChatting();
        }
    }
}
